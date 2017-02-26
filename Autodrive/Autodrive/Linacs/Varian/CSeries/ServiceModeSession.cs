using Autodrive.Interfaces;
using Autodrive.Linacs.Varian.CSeries;
using Autodrive.Linacs.Varian.CSeries.ServiceModeTableOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Autodrive
{
    public class ServiceModeSession
    {
        public IKeyboard Keyboard { get; set; }

        #region SINGLETON
        private static ServiceModeSession instance;

        public static ServiceModeSession Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServiceModeSession();
                    instance.Initialize();
                }
                return instance;
            }
        }

        public void ResetConsoleState()
        {
            Keyboard.PressEsc();
            Wait();
            Keyboard.Press("B");
            Wait();
            Keyboard.PressEsc();
            Wait();
            this.ServiceConsoleState.Main.Current = MainOptions.BEAM_CTRL;
        }

        public MotionManager MotionManager { get; private set; }

        #endregion

        private ServiceModeSession() { }

        private void Initialize()
        {
            this.MachineState = MachineState.InitNew();
            this.MachineConstraints = MachineConstraints.GetDefault();
            this.ServiceConsoleState = new ServiceConsoleState();
            MotionManager = new MotionManager(this);
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
            ResetConsoleState();
            Keyboard.Press("B");
            ServiceConsoleState.Main.Current = MainOptions.BEAM_CTRL;
            Keyboard.Press("Y");

            if (AccessoryHelper.IsEDW(MachineState.Accessory))
            {
                //The jaw moves during the beam on operation
                MachineState.Y1 = MachineState.Y2 = -5;
            }
        }

        public void RepeatBeam()
        {
            ResetConsoleState();
            Keyboard.Press("S");
            ServiceConsoleState.Main.Current = MainOptions.SET_UP;

            Keyboard.Press("D");
            ServiceConsoleState.Setup.Current = SetupOptions.DOSE;
            Keyboard.PressEnter();
            BeamOn();
        }

        public void BeamOff()
        {
            Keyboard.Press("B");
            ResetConsoleState();
        }

        public void EnterDefaultPassword()
        {
            Keyboard.Press("1111");
            Keyboard.PressEnter();
            Wait(3000);
        }

        public void ToggleDefaultInterlocks()
        {
            ResetConsoleState();
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
