using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace Autodrive.UIListeners
{
    /// <summary>
    /// This class can listen to UI events in the PTW TBA Scanning software. The design of this is to use the PTW software to run a task list,
    /// and you can use a LINACController to change LINAC parameters as new tasks are required. Run the TBA software and then use find
    /// </summary>
    public class TbaScanApp
    {
        public delegate void ApplicatorChangeHandler(string applicatorId, TbaPopup popup);

        public delegate void EnergyChangeHandler(string energyId, TbaPopup popup);

        public delegate void FieldSizeChangeHandler(double x, double y, TbaPopup popup);

        public delegate void PopupOpsCompletionHandler(TbaPopup popup);

        public delegate void PopupHandler(TbaPopup popup);

        private readonly int _processId;

        private readonly Timer t = new Timer(1000);

        private TbaScanApp(int processId)
        {
            _processId = processId;
        }

        public List<IntPtr> Children { get; set; }

        public bool ByPassOutOfBoundsPopups { get; set; } = true;

        public static TbaScanApp Find()
        {
            try
            {
                IntPtr hWnd = IntPtr.Zero;
                int tbaAppId = Process.GetProcessesByName("tbaScan").First().Id;
                var tbaApp = new TbaScanApp(tbaAppId);
                return tbaApp;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void UpdateChildren()
        {
            Children = WinAPI.EnumerateProcessWindowHandles(_processId).ToList();
        }

        public void ListenForPopup(double msInterval = 1000)
        {
            t.Elapsed += t_Elapsed;
            t.Start();
        }

        private void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            Children = WinAPI.EnumerateProcessWindowHandles(_processId).ToList();
            var captions = Children.Select(c => new { Ptr = c, Caption = WinAPI.GetWindowCaption(c), Instructions = WinAPI.GetAllChildrenWindowHandles(c, 10).Select(WinAPI.GetWindowCaption).ToList() }).ToList();
            var outOfLimits = captions.FirstOrDefault(c => c.Caption == "tbaScan" && c.Instructions.Count == 4 && c.Instructions.Last().Contains("Some measurement points are out of limits."));
            if (outOfLimits != null)
            {
                t.Stop();
                t.Elapsed -= t_Elapsed;

                var pop = new TbaPopup(outOfLimits.Ptr, true);

                if (ByPassOutOfBoundsPopups)
                {
                    //We will hanlde here
                    pop.PressOk();
                    t.Start();
                    t.Elapsed += t_Elapsed;
                }
                else
                {
                    //Handle elsewhere
                    OnPopupRaised(pop);
                }
            }

            IntPtr popup = Children.FirstOrDefault(c => WinAPI.GetWindowCaption(c).Contains("PTWtbaScan20"));
            if (popup != IntPtr.Zero)
            {
                var pop = new TbaPopup(popup);
                t.Stop();
                t.Elapsed -= t_Elapsed;
                ParseInstructions(pop);
            }
        }

        private void ParseInstructions(TbaPopup popup)
        {
            if (popup.Instructions.Contains("-field size") && !popup.Instructions.Contains("-wedge/ applicator"))
            {
                popup.ResetEvent.Reset();
                //-field size to 10.00 cm x10.00 cm
                string[] lines = popup.Instructions.Split('\n');
                string fovLine = lines.First(l => l.Contains("-field size to "));
                fovLine = fovLine.Replace("-field size to ", "").Replace("cm", "").Replace("x", "");
                List<double> numbers =
                    fovLine.Split(' ').Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => double.Parse(n)).ToList();
                OnFieldSizeChange(numbers[0], numbers[1], popup);
                //Don't move on until this is complete
                popup.ResetEvent.WaitOne();
            }
            if (popup.Instructions.Contains("-wedge/ applicator"))
            {
                popup.ResetEvent.Reset();
                //-field size to 10.00 cm x10.00 cm
                string[] lines = popup.Instructions.Split('\n');
                string fovLine = lines.First(l => l.Contains("-wedge/ applicator to"));
                string applicator = fovLine.Replace("-wedge/ applicator to ", "").Trim();
                OnApplicatorChange(applicator, popup);
                popup.ResetEvent.WaitOne();
            }
            if (popup.Instructions.Contains("-energy to"))
            {
                //-field size to 10.00 cm x10.00 cm
                string[] lines = popup.Instructions.Split('\n');
                string energy = lines.First(l => l.Contains("-energy to "));
                energy = energy.Replace("-energy to", "");
                energy = energy.Trim();
                OnEnergyChange(energy, popup);
                popup.ResetEvent.WaitOne();
            }

            OpPopupOpsCompletion(popup);
        }

        public event PopupHandler PopupRaised;

        public void OnPopupRaised(TbaPopup popup)
        {
            PopupRaised?.Invoke(popup);
        }

        public event PopupOpsCompletionHandler PopupOpsCompleted;

        public void OpPopupOpsCompletion(TbaPopup popup)
        {
            PopupOpsCompleted?.Invoke(popup);
        }

        public event FieldSizeChangeHandler FieldSizeChange;

        public void OnFieldSizeChange(double x, double y, TbaPopup popup)
        {
            FieldSizeChange?.Invoke(x, y, popup);
            if (FieldSizeChange == null) { popup.ResetEvent.Set(); }
        }

        public event ApplicatorChangeHandler ApplicatorChange;

        public void OnApplicatorChange(string applicatorId, TbaPopup popup)
        {
            ApplicatorChange?.Invoke(applicatorId, popup);
            if (ApplicatorChange == null) { popup.ResetEvent.Set(); }
        }

        public event EnergyChangeHandler EnergyChange;

        public void OnEnergyChange(string energyId, TbaPopup popup)
        {
            EnergyChange?.Invoke(energyId, popup);
            if (EnergyChange == null) { popup.ResetEvent.Set(); }
        }
    }
}