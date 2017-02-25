using Autodrive;
using Autodrive.Electrometers.StandardImaging;
using Autodrive.Linacs.Varian.CSeries;
using Cardan.ConsoleLib;
using Cardan.XCel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OutputFactors
{
    public class PhotonFactors
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
                    linac.SetMachineState(m);
                    elec.StartMeasurement();
                    Thread.Sleep(1000);
                    linac.RepeatBeam();
                    Thread.Sleep((int)(250 / 600 * 60 * 1000 + 1000)); // 250MU/600MY/min * 60 sec/min *1000 ms/sec + 1 extra second
                    elec.StopMeasurement();
                    var value = elec.GetValue().Measurement;
                    ui.Write($"Measured = {value}");
                    xcelRows.Add(new XCelData(m.Energy, m.X1 * 2, value));
                    xcel.SetRows(xcelRows, "Photons");
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
            var photonOFList = new double[] { 4, 14, 15, 20, 30, 40 }.Select(fov =>
            {
                var changeState = machineState.Copy();
                changeState.X1 = changeState.X2 = fov / 2;
                changeState.Y1 = changeState.Y2 = fov / 2;
                changeState.Energy = Autodrive.Linacs.Energy._6X;
                changeState.MU = 250;
                return changeState;
            }).ToList();

            //Add to measurement list, make a copy for other energy
            photonOFList.ForEach(p =>
            {
                measurementList.Add(p);
                var copy = p.Copy();
                copy.Energy = Autodrive.Linacs.Energy._15X;
                measurementList.Add(copy);
            });
            return measurementList;
        }
    }
}
