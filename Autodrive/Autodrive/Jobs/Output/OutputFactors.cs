using Autodrive.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodrive.Logging;
using Autodrive.Linacs.Varian.CSeries;
using Autodrive.Linacs;
using System.IO;
using Autodrive.Jobs.IO;
using System.Threading;

namespace Autodrive.Jobs.Output
{
    public class OutputFactors : IJob
    {
        private CSeriesLinac _linac;
        private IElectrometer _el;
        private I1DScanner _scan1D;
        private Dictionary<Energy, double> energyDepths = new Dictionary<Energy, double>();
        private double[] photonsFovs = new double[0];
        private string[] electronCones = new string[0];

        public int RepeatMeasurements { get; set; } = 2;

        public Logger Logger { get; set; }
        public int MUPerShot { get; set; } = 250;

        public string SavePath { get; set; }

        public OutputFactors(CSeriesLinac linac, IElectrometer el, I1DScanner scan1D)
        {
            _linac = linac;
            _el = el;
            _scan1D = scan1D;
            Logger = new Logger();
        }

        public void AddEnergyDepth(Energy en, double depthMM)
        {
            energyDepths.Add(en, depthMM);
        }

        public void SetPhotonFieldSizes(params double[] fovs)
        {
            photonsFovs = fovs;
        }

        public void SetElectronCones(params string[] cones)
        {
            electronCones = cones;
        }

        public void Run()
        {
            Run(true, true);
        }

        public void Run(bool photons = true, bool electrons = true)
        {
            if (string.IsNullOrEmpty(SavePath))
            {
                Logger.Log("Save path is empty. Will save to desktop");
                SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "outputFactors.txt");
            };

            var measurementList = BuildMeasurementList(photons,electrons);

            foreach (var m in measurementList)
            {
                var jr = new JobResult(m);
                jr.DepthOfMeasurentMM = energyDepths[m.Energy];
                _scan1D.GoToDepth(jr.DepthOfMeasurentMM);

                for (int n = 0; n < RepeatMeasurements; n++)
                {
                    var fov = EnergyHelper.IsPhoton(m.Energy) ? $"{m.X1 * 2} x {m.Y1 * 2}" : m.Accessory;
                    Logger.Log($"Working on {m.Energy}, Depth {jr.DepthOfMeasurentMM}, {fov} ,  Measurement {n + 1}");

                    var state = _linac.GetMachineStateCopy();
                    //Check for cone change
                    if (_linac.GetMachineStateCopy().Accessory != m.Accessory)
                    {
                        Console.Beep(4000, 1000);
                        Logger.Log($"Please change the cone to {m.Accessory}");
                        Logger.Log($"Press ENTER when complete");
                        while (Console.ReadKey().Key != ConsoleKey.Enter) { }
                        Logger.Log($"{m.Accessory} inserted! Continuing...");
                    }

                    _linac.SetMachineState(m);

                    //Start measuring
                    _el.Reset();
                    _el.StartMeasurement();

                    if (n == 0) { _linac.BeamOn(); }
                    else { _linac.RepeatBeam(); }

                    Thread.Sleep(_linac.WaitMsForMU(m.MU));

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

        private List<MachineState> BuildMeasurementList(bool photons = true, bool electrons = true)
        {
            var machineState = MachineState.InitNew();
            var measurementList = new List<MachineState>();

            if (photons)
            {
                //PHOTONS
                photonsFovs.ToList().ForEach(fov =>
                {
                    var changeState = machineState.Copy();
                    changeState.X1 = changeState.X2 = fov / 2;
                    changeState.Y1 = changeState.Y2 = fov / 2;
                    changeState.MU = MUPerShot;

                    foreach (var en in energyDepths.Where(e => EnergyHelper.IsPhoton(e.Key)))
                    {
                        var copy = changeState.Copy();
                        copy.Energy = en.Key;
                        measurementList.Add(copy);
                    }
                });
            }

            if (electrons)
            {
                //ELECTRONS
                electronCones.ToList().ForEach(cone =>
                {
                    var changeState = machineState.Copy();
                    changeState.Accessory = cone;
                    changeState.MU = MUPerShot;

                    foreach (var en in energyDepths.Where(e => !EnergyHelper.IsPhoton(e.Key)))
                    {
                        var copy = changeState.Copy();
                        copy.Energy = en.Key;
                        measurementList.Add(copy);
                    }
                });
            }
            return measurementList;
        }

        public static OutputFactors GetDefault(CSeriesLinac linac, IElectrometer el, I1DScanner scan1D)
        {
            var of = new OutputFactors(linac, el, scan1D);
            of.Logger.Logged += (log => Console.WriteLine(log));
            of.AddEnergyDepth(Energy._6X, 15);
            of.AddEnergyDepth(Energy._15X, 27);
            of.AddEnergyDepth(Energy._6MeV, 13);
            of.AddEnergyDepth(Energy._9MeV, 21);
            of.AddEnergyDepth(Energy._12MeV, 29);
            of.AddEnergyDepth(Energy._15MeV, 33);
            of.AddEnergyDepth(Energy._18MeV, 22);

            of.SetPhotonFieldSizes(4, 10, 15, 20, 25, 30, 40);
            of.SetElectronCones("A6", "A10", "A15", "A20", "A25");
            return of;
        }
    }
}
