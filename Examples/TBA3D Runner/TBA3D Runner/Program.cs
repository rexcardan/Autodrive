using Autodrive.Linacs.Varian.CSeries;
using Autodrive.UIListeners;
using Cardan.ConsoleLib;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBA3D_Runner
{
    class Program
    {
        static ConsoleUI ui;
        static TbaScanApp tba = null;
        static CSeriesLinac linac;

        static void Main(string[] args)
        {
            ui = new ConsoleUI();
            ui.Write("---AUTODRIVE EXAMPLE : TBA RUNNER---");
            ui.Write("");

            ui.WritePrompt("Which port is the Autodrive linac controller on?");
            var com = ui.GetStringResponse(SerialPort.GetPortNames());
            linac = new CSeriesLinac();
            linac.Initialize(com);

            //Find the running TBA scan app
            while (tba == null)
            {
                tba = TbaScanApp.Find();
                if (tba == null)
                {
                    ui.WriteError("Cannot find TBA Scan software. Please make sure it is open. Then press Enter");
                    while (Console.ReadKey().Key != ConsoleKey.Enter) { }
                }
            }

            //Ask user to start task list running before we start listening for popups
            ui.Write("TBA Software found!.");
            ui.WritePrompt("Go ahead and start a task list and start the first beam. Press Enter when once you are started.");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }

            //Subscribe to changes that will be requested by the scanning software
            tba.PopupRaised += Tba_PopupRaised;
            tba.FieldSizeChange += Tba_FieldSizeChange;
            tba.ApplicatorChange += Tba_ApplicatorChange;
            tba.EnergyChange += Tba_EnergyChange;

            tba.ListenForPopup();

        }

        private static void Tba_PopupRaised(TbaPopup popup)
        {
            if (ui.GetYesNoResponse($"Close popup {popup.Instructions}")) ;
            popup.PressOk();
        }

        private static void Tba_EnergyChange(string energyId)
        {
            ui.Write($"Changing energy to {energyId}");
            linac.StopBeam();
            var current = linac.GetMachineState();
            switch (energyId)
            {
                case "6 MV": current.Energy = Autodrive.Linacs.Energy._6X; break;
                case "15 MV": current.Energy = Autodrive.Linacs.Energy._15X; break;
                case "6 MeV": current.Energy = Autodrive.Linacs.Energy._6MeV; break;
                case "9 MeV": current.Energy = Autodrive.Linacs.Energy._9MeV; break;
                case "12 MeV": current.Energy = Autodrive.Linacs.Energy._12MeV; break;
                case "15 MeV": current.Energy = Autodrive.Linacs.Energy._15MeV; break;
                case "18 MeV": current.Energy = Autodrive.Linacs.Energy._18MeV; break;
            }

            linac.SetMachineState(current);
            linac.BeamOn(9999);
        }

        private static void Tba_ApplicatorChange(string applicatorId)
        {
            //Alert User - Need human for this part
            Console.Beep(300, 1);
            linac.StopBeam();
            ui.WritePrompt($"You must change the applicator to {applicatorId}");
            ui.WritePrompt($"Press ENTER when complete");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }

            var current = linac.GetMachineState();
            current.Accessory = applicatorId;
            linac.SetMachineState(current);
            linac.BeamOn(9999);
        }

        private static void Tba_FieldSizeChange(double x, double y)
        {
            ui.Write($"Changing field size to {x} x {y}");
            linac.StopBeam();
            var current = linac.GetMachineState();
            current.X1 = current.X2 = x / 2;
            current.Y1 = current.Y2 = x / 2;
            linac.SetMachineState(current);
            linac.BeamOn(9999);
        }
    }
}
