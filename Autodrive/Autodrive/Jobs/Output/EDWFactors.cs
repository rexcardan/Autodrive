﻿using Autodrive.Interfaces;
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
using System.IO;

namespace Autodrive.Jobs.Output
{
    public class EDWFactors : IJob
    {
        private CSeriesLinac _linac;
        private IElectrometer _el;
        private I1DScanner _scan1D;

        //Default energies
        private Energy[] energiesToMeasure = new Energy[] { Energy._6X, Energy._15X };

        public Logger Logger { get; set; }
        public double MeasurementFOV { get; set; } = 10;
        public int MUPerShot { get; set; } = 200;

        public void SetEnergiesToMeasure(params Energy[] energies)
        {
            energiesToMeasure = energies;
        }

        public int RepeatMeasurements { get; set; } = 1;
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
            if (string.IsNullOrEmpty(SavePath))
            {
                Logger.Log("Save path is empty. Will save to desktop\n");
                SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "edwFactors.txt");
            };

            var states = BuildMeasurementList();
            foreach (var m in states)
            {
                var jr = new Job(m);
                jr.DepthOfMeasurentMM = DepthOfMeasurementMM;
                if (Math.Abs(_scan1D.LastKnowPositionMM - DepthOfMeasurementMM) > 0.1)
                {
                    Logger.Log($"Changing depth to {DepthOfMeasurementMM} mm\n");
                    _scan1D.GoToDepth(jr.DepthOfMeasurentMM);
                }

                for (int n = 0; n < RepeatMeasurements; n++)
                {
                    var fov = EnergyHelper.IsPhoton(m.Energy) ? $"{m.X1 * 2} x {m.Y1 * 2}" : m.Accessory;
                    Logger.Log($"Working on {m.Energy}, Depth {jr.DepthOfMeasurentMM}, {m.Accessory} ,  Measurement {n + 1}\n\n");

                    _linac.SetMachineState(m);

                    //Start measuring
                    _el.Reset();
                    _el.StartMeasurement();

                    if (n == 0) { _linac.BeamOn(); }
                    else { _linac.RepeatBeam(); }

                    var waitTime = _linac.WaitMsForMU(m.MU, true);
                    using (var t = new TimerLogger("Waiting on beam completion", waitTime, 1000, this.Logger))
                    {
                        Thread.Sleep(waitTime);
                    }
                    //Stop and get measurement
                    _el.StopMeasurement();
                    var measured = _el.GetValue().Measurement;
                    Logger?.Log($"Measured : {measured}\n");

                    jr.AddMeasurement(_el.GetValue().Measurement);
                }
                //Save results
                JobWriter.AppendResult(SavePath, jr);
            }
        }

        private List<MachineState> BuildMeasurementList()
        {
            var machineState = MachineState.InitNew();
            var measurementList = new List<MachineState>();
            machineState.X1 = machineState.X2 = machineState.Y1 = machineState.Y2 = MeasurementFOV / 2;
            machineState.MU = MUPerShot;

            var wedgeAngles = new int[] { 10, 15, 20, 25, 30, 45, 60 };
            var wedgeList = wedgeAngles.Select(a => $"Y1IN{a}").Concat(wedgeAngles.Select(a => $"Y2OUT{a}"));

            energiesToMeasure.ToList().ForEach(en =>
            {
                var copy = machineState.Copy();
                copy.Energy = en;

                foreach (var wedge in wedgeList)
                {
                    var changeState = copy.Copy();
                    changeState.Energy = en;
                    measurementList.Add(changeState);

                }
            });
            return measurementList;
        }
    }
}
