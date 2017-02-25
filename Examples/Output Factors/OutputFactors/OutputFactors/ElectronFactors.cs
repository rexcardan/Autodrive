using Autodrive;
using Autodrive.Electrometers.StandardImaging;
using Autodrive.Linacs;
using Autodrive.Linacs.Varian.CSeries;
using Cardan.ConsoleLib;
using Cardan.XCel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Autodrive.Linacs.Energy;
namespace OutputFactors
{
    public class ElectronFactors
    {
        public static void RunFactors(ConsoleUI ui, CSeriesLinac linac, Max4000 elec, string savePath)
        {
            var xcel = new XCelBook(savePath);
            var xcelRows = new List<XCelData>();

            //Write header
            xcelRows.Add(new XCelData("ENERGY", "FOV", "MEASURED"));
            //Start measuring
            //Create a list of things to do
            var measurementList = BuildMeasurementList();
            int repeat = ui.GetIntInput("How many times to repeat each measurement?");

            foreach (var m in measurementList)
            {
                for (int i = 0; i < repeat; i++)
                {
                    ui.WritePrompt($"Starting measurement for {m.Energy} at {m.X1 * 2} x {m.Y1 * 2}");
                    //Check for cone change
                    if (linac.GetMachineStateCopy().Accessory != m.Accessory)
                    {
                        ui.WritePrompt($"Please change the cone to {m.Accessory}");
                        ui.WritePrompt($"Press ENTER when complete");
                        while (Console.ReadKey().Key != ConsoleKey.Enter) { }
                    }
                    linac.SetMachineState(m);
                    elec.StartMeasurement();
                    Thread.Sleep(1000);
                    linac.RepeatBeam();
                    Thread.Sleep((int)(250 / 600 * 60 * 1000 + 1000)); // 250MU/600MY/min * 60 sec/min *1000 ms/sec + 1 extra second
                    elec.StopMeasurement();
                    var value = elec.GetValue().Measurement;
                    ui.Write($"Measured = {value}");
                    xcelRows.Add(new XCelData(m.Energy, m.Accessory, value));
                    xcel.SetRows(xcelRows, "Electrons");
                    xcel.Save();
                    elec.Reset();
                }
            }
        }

        /// <summary>
        /// Creates a list of machine states which we will mode up one by one to take measurements
        /// </summary>
        /// <returns></returns>
        private static List<MachineState> BuildMeasurementList()
        {
            //PHOTONS
            var machineState = MachineState.InitNew();
            var measurementList = new List<MachineState>();
            var electronOFList = new string[] { "A6", "A10", "A15", "A20", "A25" }.Select(cone =>
             {
                 var changeState = machineState.Copy();
                 changeState.Accessory = cone;
                 changeState.Energy = Autodrive.Linacs.Energy._6MeV;
                 changeState.MU = 250;
                 return changeState;
             }).ToList();

            //Add to measurement list, make a copy for other energy
            electronOFList.ForEach(p =>
            {
                //Add 6MeV first
                measurementList.Add(p);

                //Foreach other energy copy a task
                foreach (var en in new Energy[] { _9MeV, _12MeV, _15MeV, _18MeV })
                {
                    var copy = p.Copy();
                    copy.Energy = en;
                    measurementList.Add(copy);
                }
            });
            return measurementList;
        }
    }
}