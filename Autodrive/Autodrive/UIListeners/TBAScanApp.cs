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
        public delegate void ApplicatorChangeHandler(string applicatorId);

        public delegate void EnergyChangeHandler(string energyId);

        public delegate void FieldSizeChangeHandler(double x, double y);

        public delegate void PopupHandler(TbaPopup popup);

        private readonly int _processId;

        private readonly Timer t = new Timer(1000);

        private TbaScanApp(int processId)
        {
            _processId = processId;
        }

        public List<IntPtr> Children { get; set; }

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
            if (popup.Instructions.Contains("-field size"))
            {
                //-field size to 10.00 cm x10.00 cm
                string[] lines = popup.Instructions.Split('\n');
                string fovLine = lines.First(l => l.Contains("-field size to "));
                fovLine = fovLine.Replace("-field size to ", "").Replace("cm", "").Replace("x", "");
                List<double> numbers =
                    fovLine.Split(' ').Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => double.Parse(n)).ToList();
                OnFieldSizeChange(numbers[0], numbers[1]);
            }
            if (popup.Instructions.Contains("-wedge/ applicator"))
            {
                //-field size to 10.00 cm x10.00 cm
                string[] lines = popup.Instructions.Split('\n');
                string fovLine = lines.First(l => l.Contains("-wedge/ applicator to"));
                string applicator = fovLine.Replace("-wedge/ applicator to ", "").Trim();
                OnApplicatorChange(applicator);
            }
            if (popup.Instructions.Contains("-energy to"))
            {
                //-field size to 10.00 cm x10.00 cm
                string[] lines = popup.Instructions.Split('\n');
                string energy = lines.First(l => l.Contains("-energy to "));
                energy = energy.Replace("-energy to", "");
                energy = energy.Trim();
                OnEnergyChange(energy);
            }
        }

        public event PopupHandler PopupRaised;

        public void OnPopupRaised(TbaPopup popup)
        {
            PopupRaised?.Invoke(popup);
        }

        public event FieldSizeChangeHandler FieldSizeChange;

        public void OnFieldSizeChange(double x, double y)
        {
            FieldSizeChange?.Invoke(x, y);
        }

        public event ApplicatorChangeHandler ApplicatorChange;

        public void OnApplicatorChange(string applicatorId)
        {
            ApplicatorChange?.Invoke(applicatorId);
        }

        public event EnergyChangeHandler EnergyChange;

        public void OnEnergyChange(string energyId)
        {
            EnergyChange?.Invoke(energyId);
        }
    }
}