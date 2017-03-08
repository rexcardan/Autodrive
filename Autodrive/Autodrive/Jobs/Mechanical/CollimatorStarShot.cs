using Autodrive.Interfaces;
using Autodrive.Linacs.Varian.CSeries;
using Autodrive.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.Jobs.Mechanical
{
    public class CollimatorStarShot : IJob
    {
        private CSeriesLinac _linac;

        public Logger Logger { get; set; }

        public int RepeatMeasurements { get; set; } = 1;

        public string SavePath { get; set; }

        public int MUPerShot { get; set; } = 500;

        public CollimatorStarShot(CSeriesLinac linac)
        {
            _linac = linac;
        }

        public void Run()
        {
            Logger?.Log($"=====COLLIMATOR STAR SHOT =====");

            var ms = _linac.GetMachineStateCopy();
            ms.X1 = ms.X2 = 0.5;
            ms.Y1 = ms.Y2 = 15;
            ms.MU = MUPerShot;
            ms.CouchRot = 180;

            foreach(var angle in new double[] { 270, 245, 220, 195, 170, 145, 120 })
            {
                Logger?.Log($"Moding up collimator angle {angle}...");
                ms.CollimatorRot = angle;
                _linac.SetMachineState(ms);
                _linac.BeamOn();
                _linac.WaitMsForMU(MUPerShot);
            }
         
        }
    }
}
