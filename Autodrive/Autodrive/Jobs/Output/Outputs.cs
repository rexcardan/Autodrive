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
using System.Threading;
using Autodrive.Jobs.IO;

namespace Autodrive.Jobs.Output
{
    public class Outputs : IJob
    {
        private CSeriesLinac _linac;
        private IElectrometer _el;
        private I1DScanner _scan1D;
        public List<Tuple<Energy, double>> energyDepths = new List<Tuple<Energy, double>>();
        private double photonFovCM;
        private string electronCone;

        public string SavePath { get; set; }
        public int RepeatMeasurements { get; set; } = 2;
        public Logger Logger { get; set; }
        public int MUPerShot { get; set; } = 100;

        public Outputs(CSeriesLinac linac, IElectrometer el, I1DScanner scan1D)
        {
            _linac = linac;
            _el = el;
            _scan1D = scan1D;
            Logger = new Logger();
        }

        public void AddEnergyDepth(Energy energy, double depthMm)
        {
            energyDepths.Add(new Tuple<Energy, double>(energy, depthMm));
        }

        public void Run()
        {

            if (string.IsNullOrEmpty(SavePath))
            {
                Logger.Log("Save path is empty. Will save to desktop");
                SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "outputs.txt");
            };

            var measurementList = BuildMeasurementList();
            BeamManager.SetFixed();
            BeamManager.SetEnergy(Energy._6X);
            BeamManager.SetDoseRate(measurementList.First().MachineStateRun.DoseRate);

            foreach (var jr in measurementList)
            {
                Task depthTask = Task.Run(() => { });
                if (_scan1D.GetCurrentDepthMM() != jr.DepthOfMeasurentMM)
                {
                    depthTask = _scan1D.GoToDepth(jr.DepthOfMeasurentMM);
                }

                for (int n = 0; n < RepeatMeasurements; n++)
                {
                    Logger.Log($"Working on {jr.MachineStateRun.Energy}, Depth {jr.DepthOfMeasurentMM},  Measurement {n + 1}");

                    var state = _linac.GetMachineStateCopy();
                    //Check for cone change
                    if (_linac.GetMachineStateCopy().Accessory != jr.MachineStateRun.Accessory)
                    {
                        Console.Beep(4000, 1000);
                        Logger.Log($"Please change the cone to {jr.MachineStateRun.Accessory}");
                        Logger.Log($"Press ENTER when complete");
                        while (Console.ReadKey().Key != ConsoleKey.Enter) { }
                        Logger.Log($"{jr.MachineStateRun.Accessory} inserted! Continuing...");
                    }

                    _linac.SetMachineState(jr.MachineStateRun);

                    //Start measuring
                    depthTask.Wait();
                    _el.Reset();
                    _el.StartMeasurement();

                    if (n == 0) { _linac.BeamOn(); }
                    else { _linac.RepeatBeam(); }

                    Thread.Sleep(_linac.WaitMsForMU(jr.MachineStateRun.MU));

                    //Stop and get measurement
                    _el.StopMeasurement();
                    var measured = _el.GetValue().Measurement;
                    Logger?.Log($"Measured : {measured}");

                    jr.AddMeasurement(_el.GetValue().Measurement);
                }

                //Save results
                JobResultWriter.AppendResult(SavePath, jr);
            }
        }

        public void SetPhotonFieldSize(double fovCm)
        {
            photonFovCM = fovCm;
        }

        public void SetElectronCone(string cone)
        {
            electronCone = cone;
        }

        private List<JobResult> BuildMeasurementList(bool photons = true, bool electrons = true)
        {
            var machineState = MachineState.InitNew();
            var measurementList = new List<JobResult>();

            var lastPhotonFovHalf = 0.5;
            if (photons)
            {
                var changeState = machineState.Copy();
                lastPhotonFovHalf = changeState.X1 = changeState.X2 = photonFovCM / 2;
                changeState.Y1 = changeState.Y2 = photonFovCM / 2;
                changeState.MU = MUPerShot;
                changeState.DoseRate = DoseRate._600;
                foreach (var en in energyDepths.Where(e => EnergyHelper.IsPhoton(e.Item1)))
                {
                    var copy = changeState.Copy();
                    copy.Energy = en.Item1;
                    measurementList.Add(new JobResult(copy) { DepthOfMeasurentMM = en.Item2 });
                }
            }
            if (electrons)
            {
                //ELECTRONS
                var changeState = machineState.Copy();
                changeState.X1 = changeState.X2 = changeState.Y1 = changeState.Y2 = lastPhotonFovHalf;
                changeState.DoseRate = DoseRate._600;
                changeState.Accessory = electronCone;
                changeState.MU = MUPerShot;

                foreach (var en in energyDepths.Where(e => !EnergyHelper.IsPhoton(e.Item1)))
                {
                    var copy = changeState.Copy();
                    copy.Energy = en.Item1;
                    measurementList.Add(new JobResult(copy) { DepthOfMeasurentMM = en.Item2 });
                }
            }
            return measurementList;
        }
    }
}
