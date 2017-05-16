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
            var couchVertChange = !double.IsNaN(couchVert) && _session.MachineState.CouchVert != couchVert;
            var couchLongChange = !double.IsNaN(couchLong) && _session.MachineState.CouchLng != couchLong;
            var couchLatChange = !double.IsNaN(couchLat) && _session.MachineState.CouchLat != couchLat;
            var couchRotChange = !double.IsNaN(couchRot) && _session.MachineState.CouchRot != couchRot;

            if (!couchVertChange && !couchLongChange && !couchLatChange && !couchRotChange)
            {
                //No changes
                return;
            }

            if (MotionWatch.IsSystemInMotion) { MotionWatch.MotionCompleteEvent.WaitOne(); }
            _session.ResetConsoleState();
            _session.Keyboard.Press("M");
            _session.ServiceConsoleState.Main.Current = MainOptions.MOTOR;
            _session.Keyboard.Press("C");
            _session.ServiceConsoleState.Motor.Current = MotorOptions.COUCH_AUTOMATIC;
            _session.ServiceConsoleState.CouchAutomatic.Current = CouchAutoOptions.VERT;

            if (couchVertChange)
            {
                _session.ServiceConsoleState.CouchAutomatic.MoveTo(CouchAutoOptions.VERT);
                _session.Keyboard.EnterNumber(couchVert);
                this.MotionWatch.AddMotion(_session.MachineState.CouchVert, couchVert, _session.MachineConstraints.CouchVertMoveCMPerSec);
            }

            if (couchLongChange)
            {
                _session.ServiceConsoleState.CouchAutomatic.MoveTo(CouchAutoOptions.LONG);
                _session.Keyboard.EnterNumber(couchLong);
                this.MotionWatch.AddMotion(_session.MachineState.CouchLng, couchLong, _session.MachineConstraints.CouchMoveCMPerSec);
            }

            if (couchLatChange)
            {
                _session.ServiceConsoleState.CouchAutomatic.MoveTo(CouchAutoOptions.LAT);
                _session.Keyboard.EnterNumber(couchLat);
                this.MotionWatch.AddMotion(_session.MachineState.CouchLat, couchLat, _session.MachineConstraints.CouchMoveCMPerSec);
            }

            if (couchRotChange)
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
            var collChange = !double.IsNaN(collimatorAngle) && _session.MachineState.CollimatorRot != collimatorAngle;
            var y1Change = !double.IsNaN(y1) && _session.MachineState.Y1 != y1;
            var y2Change = !double.IsNaN(y2) && _session.MachineState.Y2 != y2;
            var x1Change = !double.IsNaN(x1) && _session.MachineState.X1 != x1;
            var x2Change = !double.IsNaN(x2) && _session.MachineState.X2 != x2;
            var gantryAngleChange = !double.IsNaN(gantryAngle) && _session.MachineState.GantryRot != gantryAngle;

            if (!collChange && !y1Change && !y2Change && !x1Change && !x2Change && !gantryAngleChange)
            { //No changes
                return;
            }

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

            if (collChange)
            {
                _session.Keyboard.EnterNumber(collimatorAngle);
                this.MotionWatch.AddMotion(_session.MachineState.CollimatorRot, collimatorAngle, _session.MachineConstraints.CollimatorDegPerSec);
            };

            if (y1Change)
            {
                _session.ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.COLL_Y1);
                _session.Keyboard.EnterNumber(y1);
                this.MotionWatch.AddMotion(_session.MachineState.Y1, y1, _session.MachineConstraints.YJawCMPerSec);
            }
            if (y2Change)
            {
                _session.ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.COLL_Y2);
                _session.Keyboard.EnterNumber(y2);
                this.MotionWatch.AddMotion(_session.MachineState.Y2, y2, _session.MachineConstraints.YJawCMPerSec);
            }

            if (x1Change)
            {
                _session.ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.COLL_X1);
                _session.Keyboard.EnterNumber(x1);
                this.MotionWatch.AddMotion(_session.MachineState.X1, x1, _session.MachineConstraints.XJawCMPerSec);
            }
            if (x2Change)
            {
                _session.ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.COLL_X2);
                _session.Keyboard.EnterNumber(x2);
                this.MotionWatch.AddMotion(_session.MachineState.X2, x2, _session.MachineConstraints.XJawCMPerSec);
            }
            if (gantryAngleChange)
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
