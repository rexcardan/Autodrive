using Autodrive.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodrive.Logging;
using Autodrive.Linacs.Varian.CSeries;
using Autodrive.Linacs;
using System.Threading;
using Autodrive.Jobs.IO;

namespace Autodrive.Jobs.Output
{
    public class EDWFactors : IJob
    {
        private CSeriesLinac _linac;
        private IElectrometer _el;
        private I1DScanner _scan1D;
        private Energy[] energiesToMeasure = new Energy[] { Energy._6X, Energy._15X };
        public Logger Logger { get; set; }
        public double MeasurementFOV { get; set; } = 10;

        public void SetEnergiesToMeasure(params Energy[] energies)
        {
            energiesToMeasure = energies;
        }

        public int RepeatMeasurements { get; set; }
        public double DepthOfMeasurementMM { get; set; } = 50;
        public string SavePath { get; set; }

        public EDWFactors(CSeriesLinac linac, IElectrometer el, I1DScanner scan1D)
        {
            _linac = linac;
            _el = el;
            _scan1D = scan1D;
            Logger = new Logger();
        }

        public void Run()
        {
            var states = BuildMeasurementList();
            foreach (var m in states)
            {
                var jr = new JobResult(m);
                jr.DepthOfMeasurentMM = DepthOfMeasurementMM;
                if (Math.Abs(_scan1D.LastKnowPositionMM - DepthOfMeasurementMM) > 0.1)
                {
                    Logger.Log($"Changing depth to {DepthOfMeasurementMM} mm");
                    _scan1D.GoToDepth(jr.DepthOfMeasurentMM);
                }

                for (int n = 0; n < RepeatMeasurements; n++)
                {
                    var fov = EnergyHelper.IsPhoton(m.Energy) ? $"{m.X1 * 2} x {m.Y1 * 2}" : m.Accessory;
                    Logger.Log($"Working on {m.Energy}, Depth {jr.DepthOfMeasurentMM}, {fov} ,  Measurement {n + 1}");

                    _linac.SetMachineState(m);

                    //Start measuring
                    _el.Reset();
                    _el.StartMeasurement();

                    if (n == 0) { _linac.BeamOn(); }
                    else { _linac.RepeatBeam(); }

                    Thread.Sleep(_linac.WaitMsForMU(m.MU, true));

                    //Stop and get measurement
                    _el.StopMeasurement();
                    var measured = _el.GetValue().Measurement;
                    Logger?.Log($"Measured : {measured}");

                    //Save results
                    jr.AddMeasurement(_el.GetValue().Measurement);

                    JobResultWriter.AppendResult(SavePath, jr);
                }
            }
        }

        private List<MachineState> BuildMeasurementList()
        {
            var machineState = MachineState.InitNew();
            var measurementList = new List<MachineState>();
            machineState.X1 = machineState.X2 = machineState.Y1 = machineState.Y2 = MeasurementFOV / 2;

            var wedgeAngles = new int[] { 10, 15, 20, 25, 30, 45, 60 };
            var wedgeList = wedgeAngles.Select(a => $"Y1IN{a}").Concat(wedgeAngles.Select(a => $"Y2OUT{a}"));

            foreach (var wedge in wedgeList)
            {
                var copy = machineState.Copy();
                copy.Accessory = wedge;

                energiesToMeasure.ToList().ForEach(en =>
                {
                    var changeState = copy.Copy();
                    changeState.Energy = en;
                    measurementList.Add(changeState);

                });
            }
            return measurementList;
        }
    }
}
