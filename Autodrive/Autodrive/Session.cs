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

        #region SINGLETON
        private static Session instance;

        public static Session Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Session();
                }
                return instance;
            }
        }
        #endregion

        private Session()
        {
            this.MachineState = MachineState.InitNew();
            this.MachineConstraints = MachineConstraints.GetDefault();
            this.ServiceConsoleState = new ServiceConsoleState();
        }

        internal void SetGantryAutomatic(double collimatorAngle, double x1, double x2, double y1, double y2, double gantryAngle)
        {
            throw new NotImplementedException();
        }

        public void Wait()
        {
            Thread.Sleep(150);
        }

        public MachineConstraints MachineConstraints { get; private set; }
        public MachineState MachineState { get; private set; }
        public ServiceConsoleState ServiceConsoleState { get; private set; }

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
        }

        public void ToggleDefaultInterlocks()
        {
            ServiceConsoleState.Main.Select(MainOptions.INTERLOCK_TG);
            ServiceConsoleState.InterlockTrig.Select(InterlockTrigOptions.OVERRIDE_INTRLKS);
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
