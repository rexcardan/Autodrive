using Autodrive.Linacs.Varian.CSeries.ServiceModeTableOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.Linacs.Varian.CSeries
{
    /// <summary>
    /// Class responsible for moving the gantry and the couch
    /// </summary>
    public class MotionManager
    {
        private ServiceModeSession _session;
        public MotionWatch MotionWatch { get; private set; }

        public MotionManager(ServiceModeSession session)
        {
            _session = session;
            this.MotionWatch = new MotionWatch();
        }

        public void SetCouchAutomatic(double couchVert, double couchLong, double couchLat, double couchRot)
        {
            if (MotionWatch.IsSystemInMotion) { MotionWatch.MotionCompleteEvent.WaitOne(); }
            _session.ResetConsoleState();
            _session.Keyboard.Press("M");
            _session.ServiceConsoleState.Main.Current = MainOptions.MOTOR;
            _session.Keyboard.Press("C");
            _session.ServiceConsoleState.Motor.Current = MotorOptions.COUCH_AUTOMATIC;
            _session.ServiceConsoleState.CouchAutomatic.Current = CouchAutoOptions.VERT;

            if (_session.MachineState.CouchVert != couchVert)
            {
                _session.ServiceConsoleState.CouchAutomatic.MoveTo(CouchAutoOptions.VERT);
                _session.Keyboard.EnterNumber(couchVert);
                this.MotionWatch.AddMotion(_session.MachineState.CouchVert, couchVert, _session.MachineConstraints.CouchVertMoveCMPerSec);
            }

            if (_session.MachineState.CouchLng != couchLong)
            {
                _session.ServiceConsoleState.CouchAutomatic.MoveTo(CouchAutoOptions.LONG);
                _session.Keyboard.EnterNumber(couchLong);
                this.MotionWatch.AddMotion(_session.MachineState.CouchLng, couchLong, _session.MachineConstraints.CouchMoveCMPerSec);
            }

            if (_session.MachineState.CouchLat != couchLat)
            {
                _session.ServiceConsoleState.CouchAutomatic.MoveTo(CouchAutoOptions.LAT);
                _session.Keyboard.EnterNumber(couchLat);
                this.MotionWatch.AddMotion(_session.MachineState.CouchLat, couchLat, _session.MachineConstraints.CouchMoveCMPerSec);
            }

            if (_session.MachineState.CouchRot != couchRot)
            {
                _session.ServiceConsoleState.CouchAutomatic.MoveTo(CouchAutoOptions.ROT);
                _session.Keyboard.EnterNumber(couchRot);
                this.MotionWatch.AddMotion(_session.MachineState.CouchRot, couchRot, _session.MachineConstraints.CouchRotDegPerSec);
            }
            _session.Keyboard.PressF2();
            this.MotionWatch.StartMotionClock();

            //Update machine state
            _session.MachineState.CouchVert = couchVert;
            _session.MachineState.CouchLat = couchLat;
            _session.MachineState.CouchLng = couchLong;
            _session.MachineState.CouchRot = couchRot;
        }

        /// <summary>
        /// Sets parameters for gantry and collimator
        /// </summary>
        /// <param name="collimatorAngle"></param>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <param name="gantryAngle"></param>
        public void SetGantryAutomatic(double collimatorAngle, double x1, double x2, double y1, double y2, double gantryAngle)
        {
            //Wait until previous motion is complete to avoid a collision
            if (MotionWatch.IsSystemInMotion) { MotionWatch.MotionCompleteEvent.WaitOne(); }
            //Get console state in common known position
            _session.ResetConsoleState();
            //Motor
            _session.Keyboard.Press("M");
            _session.ServiceConsoleState.Main.Current = MainOptions.MOTOR;
            //Gantry
            _session.Keyboard.Press("G");
            _session.ServiceConsoleState.Motor.Current = MotorOptions.GANTRY_AUTOMATIC;
            //Let the table know which cell the cursor is currently on
            _session.ServiceConsoleState.GantryAutomatic.Current = GantryAutoOptions.COLLIMATOR_ROT;

            if (_session.MachineState.CollimatorRot != collimatorAngle)
            {
                _session.Keyboard.EnterNumber(collimatorAngle);
                this.MotionWatch.AddMotion(_session.MachineState.CollimatorRot, collimatorAngle, _session.MachineConstraints.CollimatorDegPerSec);
            };

            if (_session.MachineState.Y1 != y1)
            {
                _session.ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.COLL_Y1);
                _session.Keyboard.EnterNumber(y1);
                this.MotionWatch.AddMotion(_session.MachineState.Y1, y1, _session.MachineConstraints.YJawCMPerSec);
            }
            if (_session.MachineState.Y2 != y2)
            {
                _session.ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.COLL_Y2);
                _session.Keyboard.EnterNumber(y2);
                this.MotionWatch.AddMotion(_session.MachineState.Y2, y2, _session.MachineConstraints.YJawCMPerSec);
            }

            if (_session.MachineState.X1 != x1)
            {
                _session.ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.COLL_X1);
                _session.Keyboard.EnterNumber(x1);
                this.MotionWatch.AddMotion(_session.MachineState.X1, x1, _session.MachineConstraints.XJawCMPerSec);
            }
            if (_session.MachineState.X2 != x2)
            {
                _session.ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.COLL_X2);
                _session.Keyboard.EnterNumber(x2);
                this.MotionWatch.AddMotion(_session.MachineState.X2, x2, _session.MachineConstraints.XJawCMPerSec);
            }
            if (_session.MachineState.GantryRot != gantryAngle)
            {
                _session.ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.GANTRY_ROT);
                _session.Keyboard.EnterNumber(gantryAngle);
                this.MotionWatch.AddMotion(_session.MachineState.GantryRot, gantryAngle, _session.MachineConstraints.GantryDegPerSec);
            }

            //Enable motion to begin (dead man switch must be held somehow - I suggest a stapler ;)
            _session.Keyboard.PressF2();

            //Starts an underlying timer which can be monitored to see if motion is still occuring
            this.MotionWatch.StartMotionClock();

            //Update machine state
            _session.MachineState.X1 = x1;
            _session.MachineState.X2 = x2;
            _session.MachineState.Y1 = y1;
            _session.MachineState.Y2 = y2;
            _session.MachineState.GantryRot = gantryAngle;
            _session.MachineState.CollimatorRot = collimatorAngle;

            if (MotionWatch.IsSystemInMotion)
                this.MotionWatch.MotionCompleteEvent.WaitOne();
        }
    }
}
