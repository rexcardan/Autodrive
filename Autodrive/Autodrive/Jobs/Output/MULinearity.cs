using Autodrive.Interfaces;
using Autodrive.Linacs;
using Autodrive.Linacs.Varian.CSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodrive.Logging;
using Autodrive.Jobs.IO;
using System.IO;
using System.Threading;

namespace Autodrive.Jobs.Output
{
    public class MULinearity : IJob
    {
        private IElectrometer _el;
        private CSeriesLinac _linac;
        private I1DScanner _scan1D;
        private List<int> musToTest = new List<int>();
        private List<Energy> energiesToTest = new List<Energy>();

        public string SavePath { get; set; }
        public double ScanningDepthMM { get; set; } = 50;
        public int RepeatMeasurements { get; set; } = 2;

        public Logger Logger { get; set; }

        public MULinearity(CSeriesLinac linac, IElectrometer el, I1DScanner scan1D)
        {
            _linac = linac;
            _el = el;
            _scan1D = scan1D;
            Logger = new Logger();
        }

        public void SetMULevels(params int[] mus)
        {
            mus.ToList().ForEach(m => { if (!musToTest.Contains(m)) { musToTest.Add(m); } });
        }

        public void SetEnergiesToTest(params Energy[] energies)
        {
            energies.ToList().ForEach(e => { if (!energiesToTest.Contains(e)) { energiesToTest.Add(e); } });
        }

        public void Run()
        {
            if (string.IsNullOrEmpty(SavePath))
            {
                Logger.Log("Save path is empty. Will save to desktop\n");
                SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "mulinearity.txt");
            };

            //Move to Scanning Depth
            Logger.Log("Moving scanning chamber...\n");
            _scan1D.GoToDepth(ScanningDepthMM).Wait();

            var ms = MachineState.InitNew();
            ms.X1 = ms.X2 = ms.Y1 = ms.Y2 = 5;

            foreach (var en in energiesToTest)
            {
                foreach (var mu in musToTest)
                {
                    var movingMs = ms.Copy();
                    movingMs.Energy = en;
                    movingMs.MU = mu;
                    var jr = new Job(movingMs);
                    jr.DepthOfMeasurentMM = ScanningDepthMM;
                    for (int n = 0; n < RepeatMeasurements; n++)
                    {
                        Logger.Log($"Working on {en}, {mu} MU, Measurement {n + 1}\n");

                        _linac.SetMachineState(movingMs);

                        //Start measuring
                        _el.Reset();
                        _el.StartMeasurement();

                        if (n == 0) { _linac.BeamOn(); }
                        else { _linac.RepeatBeam(); }

                        Thread.Sleep(_linac.WaitMsForMU(mu));

                        //Stop and get measurement
                        _el.StopMeasurement();
                        var measured = _el.GetValue().Measurement;
                        Logger?.Log($"Measured : {measured}\n");

                        //Save results
                        jr.AddMeasurement(_el.GetValue().Measurement);
                    }
                    JobWriter.AppendResult(SavePath, jr);
                }
            }
        }

        public static MULinearity GetDefault(CSeriesLinac linac, IElectrometer el, I1DScanner scan1D)
        {
            var muTest = new MULinearity(linac, el, scan1D);
            muTest.Logger.Logged += (log=>Console.WriteLine(log));
            muTest.ScanningDepthMM = 50;
            muTest.SetEnergiesToTest(Energy._6X, Energy._15X);
            muTest.SetMULevels(10, 20, 50, 100, 200, 500);
            return muTest;
        }
    }
}
