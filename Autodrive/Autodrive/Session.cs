using Autodrive.Interfaces;
using Autodrive.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Autodrive
{
    public class Session
    {
        public IKeyboard Keyboard { get; set; }
        public MotionWatch MotionWatch { get; private set; }

        #region SINGLETON
        private static Session instance;

        public static Session Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Session();
                    instance.Initialize();
                }
                return instance;
            }
        }

        public void ResetState()
        {
            Keyboard.PressEsc();
            Wait();
            Keyboard.Press("B");
            Wait();
            Keyboard.PressEsc();
            Wait();
            this.ServiceConsoleState.Main.Current = MainOptions.BEAM_CTRL;
        }

        public void SetCouchAutomatic(double couchVert, double couchLong, double couchLat, double couchRot)
        {
            if (MotionWatch.IsSystemInMotion) { MotionWatch.MotionCompleteEvent.WaitOne(); }
            ResetState();
            Keyboard.Press("M");
            ServiceConsoleState.Main.Current = MainOptions.MOTOR;
            Keyboard.Press("C");
            ServiceConsoleState.Motor.Current = MotorOptions.COUCH_AUTOMATIC;
            ServiceConsoleState.CouchAutomatic.Current = CouchAutoOptions.VERT;

            if (MachineState.CouchVert != couchVert)
            {
                ServiceConsoleState.CouchAutomatic.MoveTo(CouchAutoOptions.VERT);
                Keyboard.EnterNumber(couchVert);
                this.MotionWatch.AddMotion(this.MachineState.CouchVert, couchVert, MachineConstraints.CouchVertMoveCMPerSec);
            }

            if (MachineState.CouchLng != couchLong)
            {
                ServiceConsoleState.CouchAutomatic.MoveTo(CouchAutoOptions.LONG);
                Keyboard.EnterNumber(couchLong);
                this.MotionWatch.AddMotion(this.MachineState.CouchLng, couchLong, MachineConstraints.CouchMoveCMPerSec);
            }

            if (MachineState.CouchLat != couchLat)
            {
                ServiceConsoleState.CouchAutomatic.MoveTo(CouchAutoOptions.LAT);
                Keyboard.EnterNumber(couchLat);
                this.MotionWatch.AddMotion(this.MachineState.CouchLat, couchLat, MachineConstraints.CouchMoveCMPerSec);
            }

            if (MachineState.CouchRot != couchRot)
            {
                ServiceConsoleState.CouchAutomatic.MoveTo(CouchAutoOptions.ROT);
                Keyboard.EnterNumber(couchRot);
                this.MotionWatch.AddMotion(this.MachineState.CouchRot, couchRot, MachineConstraints.CouchRotDegPerSec);
            }
            Keyboard.PressF2();
            this.MotionWatch.StartMotionClock();

            //Update machine state
            this.MachineState.CouchVert = couchVert;
            this.MachineState.CouchLat = couchLat;
            this.MachineState.CouchLng = couchLong;
            this.MachineState.CouchRot = couchRot;
        }

        #endregion

        private Session() { }

        private void Initialize()
        {
            this.MachineState = MachineState.InitNew();
            this.MachineConstraints = MachineConstraints.GetDefault();
            this.ServiceConsoleState = new ServiceConsoleState();
            this.MotionWatch = new MotionWatch();
        }

        public void SetGantryAutomatic(double collimatorAngle, double x1, double x2, double y1, double y2, double gantryAngle)
        {
            if (MotionWatch.IsSystemInMotion) { MotionWatch.MotionCompleteEvent.WaitOne(); }
            ResetState();
            Keyboard.Press("M");
            ServiceConsoleState.Main.Current = MainOptions.MOTOR;
            Keyboard.Press("G");
            ServiceConsoleState.Motor.Current = MotorOptions.GANTRY_AUTOMATIC;
            ServiceConsoleState.GantryAutomatic.Current = GantryAutoOptions.COLLIMATOR_ROT;

            if (MachineState.CollimatorRot != collimatorAngle)
            {
                Keyboard.EnterNumber(collimatorAngle);
                this.MotionWatch.AddMotion(this.MachineState.CollimatorRot, collimatorAngle, MachineConstraints.CollimatorDegPerSec);
            };

            if (MachineState.Y1 != y1)
            {
                ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.COLL_Y1);
                Keyboard.EnterNumber(y1);
                this.MotionWatch.AddMotion(this.MachineState.Y1, y1, MachineConstraints.YJawCMPerSec);
            }
            if (MachineState.Y2 != y2)
            {
                ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.COLL_Y2);
                Keyboard.EnterNumber(y2);
                this.MotionWatch.AddMotion(this.MachineState.Y2, y2, MachineConstraints.YJawCMPerSec);
            }

            if (MachineState.X1 != x1)
            {
                ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.COLL_X1);
                Keyboard.EnterNumber(x1);
                this.MotionWatch.AddMotion(this.MachineState.X1, x1, MachineConstraints.XJawCMPerSec);
            }
            if (MachineState.X2 != x2)
            {
                ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.COLL_X2);
                Keyboard.EnterNumber(x2);
                this.MotionWatch.AddMotion(this.MachineState.X2, x2, MachineConstraints.XJawCMPerSec);
            }
            if (MachineState.GantryRot != gantryAngle)
            {
                ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.GANTRY_ROT);
                Keyboard.EnterNumber(gantryAngle);
                this.MotionWatch.AddMotion(this.MachineState.GantryRot, gantryAngle, MachineConstraints.GantryDegPerSec);
            }
            Keyboard.PressF2();

            this.MotionWatch.StartMotionClock();

            //Update machine state
            this.MachineState.X1 = x1;
            this.MachineState.X2 = x2;
            this.MachineState.Y1 = y1;
            this.MachineState.Y2 = y2;
            this.MachineState.GantryRot = gantryAngle;
            this.MachineState.CollimatorRot = collimatorAngle;
        }

        

        public void Wait(int ms = 0)
        {
            ms = ms > 0 ? ms : KeySpeedMs;
            Thread.Sleep(ms);
        }

        public MachineConstraints MachineConstraints { get; private set; }
        public MachineState MachineState { get; private set; }
        public ServiceConsoleState ServiceConsoleState { get; private set; }
        public int KeySpeedMs { get; set; } = 100;

        public void BeamOn()
        {
            ServiceConsoleState.Main.Select(MainOptions.BEAM_CTRL);
            Keyboard.Press("Y");
        }

        public void RepeatBeam()
        {
            ServiceConsoleState.Main.Select(MainOptions.SET_UP);
            ServiceConsoleState.Setup.Select(SetupOptions.DOSE);
            Keyboard.PressEnter();
            BeamOn();
        }

        public void EnterDefaultPassword()
        {
            Keyboard.Press("1111");
            Keyboard.PressEnter();
            Wait(3000);
        }

        public void ToggleDefaultInterlocks()
        {
            ResetState();
            Keyboard.Press("I");
            ServiceConsoleState.Main.Current = MainOptions.INTERLOCK_TG;
            Keyboard.Press("O");
            ServiceConsoleState.InterlockTrig.Current = InterlockTrigOptions.OVERRIDE_INTRLKS;

            ServiceConsoleState.Interlocks.Select(InterlockOptions.MLC);
            ServiceConsoleState.Interlocks.Select(InterlockOptions.ACC);
            ServiceConsoleState.Interlocks.Select(InterlockOptions.MOTN);
            ServiceConsoleState.Interlocks.Select(InterlockOptions.COLL);
            ServiceConsoleState.Interlocks.Select(InterlockOptions.PNDT);
            ServiceConsoleState.Interlocks.Select(InterlockOptions.KEY);
            Keyboard.PressEsc();
        }
    }
}
