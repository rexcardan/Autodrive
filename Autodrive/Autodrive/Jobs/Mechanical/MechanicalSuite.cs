using Autodrive.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodrive.Logging;
using Autodrive.Linacs.Varian.CSeries;
using System.Threading;

namespace Autodrive.Jobs.Mechanical
{
    public class MechanicalSuite : IJob
    {
        private CSeriesLinac _linac;

        public MechanicalSuite(CSeriesLinac linac)
        {
            _linac = linac;
        }

        public Logger Logger { get; set; }

        public int RepeatMeasurements { get; set; }

        public string SavePath { get; set; }


        public void Run()
        {
            //Put couch and gantry and home position
            var ms = MachineState.InitNew();
            ms.Energy = Linacs.Energy._6X;
            //ms.CouchLat = 101;
            //ms.CouchLng = 101;
            //ms.CouchVert = 101;
            //ms.CouchRot = 181;
            //_linac.SetMachineState(ms);
            //Thread.Sleep(8000);
            ms.Time = 99;

            //Set through different x y jaw combos and beam on to film/portal
            var jawShows = JawShots.GetXYCalibrationShots(_linac);
            //jawShows.Run();

            //Shoot a couple square fields to test for coincidence
            //jawShows = JawShots.GetLightFieldCoincidence(_linac);
            //jawShows.Run();

            //TESTING FOR COUCH RELATIVE MOTION ACCURACY
            //Step couch 20 cm long and lat to shoot a square
            ms = _linac.GetMachineStateCopy();
            ms.CouchLat += 8;
            ms.CouchLng += 8;
            ms.X1 = ms.X2 = ms.Y1 = ms.Y2 = 1; //2 x 2 field
            ms.MU = 400;
            _linac.SetMachineState(ms);
            _linac.BeamOn();
            Thread.Sleep(_linac.WaitMsForMU(400));

            //Move couch to other side shoot a square
            ms.CouchLat -= 16;
            ms.CouchLng -= 16;
            _linac.SetMachineState(ms);
            _linac.BeamOn();
            Thread.Sleep(_linac.WaitMsForMU(400));

            //Collimator Star
            //var cs = new CollimatorStarShot(_linac);
            //cs.Run();

            //Couch Star
            var cos = new CouchStarShot(_linac);
            cos.Run();
        }
    }
}
