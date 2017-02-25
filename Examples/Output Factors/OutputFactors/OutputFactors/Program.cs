using Autodrive;
using Autodrive.Electrometers.StandardImaging;
using Autodrive.Interfaces;
using Autodrive.Linacs.Varian.CSeries;
using Cardan.ConsoleLib;
using Cardan.XCel;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OutputFactors
{
    class Program
    {
        static ConsoleUI ui = new ConsoleUI();
        static CSeriesLinac linac;
        static Max4000 elec;

        [STAThread]
        static void Main(string[] args)
        {
            ui.Write("---AUTODRIVE EXAMPLE : OUTPUT FACTORS---");
            ui.WritePrompt("Which port is the Autodrive linac controller on?");
            var com = ui.GetStringResponse(SerialPort.GetPortNames());
            linac = new CSeriesLinac();
            linac.Initialize(com);
            ui.Write(""); //--Space

            //Set up electrometer
            var elecVerified = false;
            while (!elecVerified)
            {
                ui.WritePrompt("Which port is the Electrometer on?");
                com = ui.GetStringResponse(SerialPort.GetPortNames());
                elec = new Max4000();
                elec.Initialize(com);
                elecVerified = elec.Verify();
                if (!elecVerified) { ui.WriteError("Cannot find the Max 4000 electrometer. Try again."); }
            }

            if (ui.GetYesNoResponse("Do I need to zero the electrometer?"))
            {    //Get Electrometer ready
                ui.Write(""); //--Space
                ui.Write("Zeroing Electrometer..."); //--Space
                elec.Zero().Wait();
            }

            elec.SetBias(Autodrive.Electrometers.Bias.NEG_100PERC);
            elec.SetMode(Autodrive.Electrometers.MeasureMode.CHARGE);
            elec.SetRange(Autodrive.Electrometers.Enums.Range.HIGH);
            Thread.Sleep(3000);


            ui.WritePrompt("I am going to store to an Excel file. I need some information");
            var savePath = ui.GetSaveFilePath("outputFactors.xlsx");

            PhotonFactors.RunFactors(ui, linac, elec, savePath);

            //Need to add depth changing for electrons
            //ElectronFactors.RunFactors(ui, linac, elec, savePath);

            ui.WritePrompt("COMPLETE!");

        }
    }
}
