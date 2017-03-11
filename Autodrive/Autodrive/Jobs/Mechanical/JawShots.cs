using Autodrive.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodrive.Logging;
using Autodrive.Linacs.Varian.CSeries;
using System.Threading;
using Autodrive.Linacs;

namespace Autodrive.Jobs.Mechanical
{
    public class JawShots : IJob
    {
        private List<MachineState> toShoot = new List<MachineState>();
        private CSeriesLinac _linac;

        public JawShots(CSeriesLinac linac)
        {
            _linac = linac;
        }

        public Logger Logger { get; set; }

        public int RepeatMeasurements { get; set; } = 1;

        public string SavePath { get; set; }

        public int MUPerShot { get; set; } = 500;

        public void Run()
        {
            foreach (var shot in toShoot)
            {
                shot.MU = MUPerShot;
                _linac.SetMachineState(shot);
                _linac.BeamOn();
                Thread.Sleep(_linac.WaitMsForMU(shot.MU));
                for (int i = 1; i < RepeatMeasurements; i++)
                {
                    _linac.RepeatBeam();
                }
            }
        }

        public void AddShot(double x1, double x2, double y1, double y2, Energy energy = Energy._6X)
        {
            var ms = ServiceModeSession.Instance.MachineState.Copy();
            ms.X1 = x1;
            ms.X2 = x2;
            ms.Y1 = y1;
            ms.Y2 = y2;
            ms.Energy = energy;
            toShoot.Add(ms);
        }

        public void AddShot(double x, double y, Energy energy = Energy._6X)
        {
            var ms = ServiceModeSession.Instance.MachineState.Copy();
            ms.X1 = x / 2;
            ms.X2 = x / 2;
            ms.Y1 = y / 2;
            ms.Y2 = y / 2;
            ms.Energy = energy;
            toShoot.Add(ms);
        }

        public static JawShots GetLightFieldCoincidence(CSeriesLinac linac)
        {
            var js = new JawShots(linac);
            js.AddShot(10, 10);
            js.AddShot(15, 15, Energy._15X);
            return js;
        }

        public static JawShots GetXYCalibrationShots(CSeriesLinac linac)
        {
            var js = new JawShots(linac);
            var sizes = new double[] { 2.5, 5, 7.5, 10, 15 };
            foreach (var size in sizes)
                js.AddShot(size, size, 1.3, 1.3);
            foreach (var size in sizes)
                js.AddShot(1.3, 1.3, size, size);
            return js;
        }
    }
}
