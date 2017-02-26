using Autodrive.Linacs.Varian.CSeries.ServiceModeTableOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SM = Autodrive.ServiceModeSession;

namespace Autodrive.Linacs.Varian.CSeries
{
    public class BeamManager
    {
        public static void SetRepRate(RepRateOptions repRate)
        {
            SM.Instance.ServiceConsoleState.Main.Select(MainOptions.SET_UP);
            Thread.Sleep(150);
            SM.Instance.ServiceConsoleState.Setup.Select(SetupOptions.REP_RATE);
            Thread.Sleep(200);
            SM.Instance.ServiceConsoleState.RepRates.Select(repRate);
            SM.Instance.Keyboard.PressEnter();
            Thread.Sleep(200);
            SM.Instance.ResetConsoleState();
        }

        public static void SetMU(int mu)
        {
            SetUp();
            SM.Instance.Keyboard.Press("D");
            SM.Instance.ServiceConsoleState.Setup.Current = (SetupOptions.DOSE);
            Thread.Sleep(200);
            SM.Instance.Keyboard.EnterNumber(mu);
            SM.Instance.Keyboard.PressEnter();
            Thread.Sleep(200);
            SM.Instance.ResetConsoleState();
        }

        public static void SetTime(double time)
        {
            SetUp();

            SM.Instance.Keyboard.Press("T");
            SM.Instance.ServiceConsoleState.Setup.Current = SetupOptions.TIME;
            SM.Instance.Wait(200);
            SM.Instance.Keyboard.EnterNumber(time);
            SM.Instance.Keyboard.PressEnter();
            SM.Instance.MachineState.Time = time;
            SM.Instance.Wait(200);
            SM.Instance.ResetConsoleState();
        }

        private static void SetUp()
        {
            SM.Instance.Keyboard.Press("S");
            SM.Instance.ServiceConsoleState.Main.Current = MainOptions.SET_UP;
            SM.Instance.Wait();
        }

        public static void SetAccessory(AccessoryOptions acc)
        {
            if (acc != SM.Instance.ServiceConsoleState.Accessories.Current)
            {
                SetUp();
                SM.Instance.Keyboard.Press("A");
                SM.Instance.ServiceConsoleState.Setup.Current = SetupOptions.ACCESSORIES;
                SM.Instance.Wait(200);

                SM.Instance.ServiceConsoleState.Accessories.Select(acc);
                SM.Instance.Keyboard.PressEnter();
                SM.Instance.Wait(200);
            }
        }

        public static void SetEnergy(Energy energy)
        {
            if (energy != SM.Instance.MachineState.Energy)
            {
                SetUp();
                SM.Instance.Keyboard.Press("E");
                SM.Instance.ServiceConsoleState.Setup.Current = SetupOptions.ENERGY;
                switch (energy)
                {
                    case Energy._6X: SM.Instance.ServiceConsoleState.Energies.Select(EnergyOptions.X1); break;
                    case Energy._15X:
                    case Energy._18X: SM.Instance.ServiceConsoleState.Energies.Select(EnergyOptions.X2); break;
                    case Energy._6MeV: SM.Instance.ServiceConsoleState.Energies.Select(EnergyOptions.E1); break;
                    case Energy._9MeV: SM.Instance.ServiceConsoleState.Energies.Select(EnergyOptions.E2); break;
                    case Energy._12MeV: SM.Instance.ServiceConsoleState.Energies.Select(EnergyOptions.E3); break;
                    case Energy._15MeV:
                    case Energy._16MeV: SM.Instance.ServiceConsoleState.Energies.Select(EnergyOptions.E4); break;
                    case Energy._18MeV:
                    case Energy._20MeV: SM.Instance.ServiceConsoleState.Energies.Select(EnergyOptions.E5); break;
                }
                SM.Instance.MachineState.Energy = energy;
                Thread.Sleep(SM.Instance.MachineConstraints.EnergySwitchTimeSec * 1000);
            }
        }

        public static void SetEDW(EDWOptions edwOptions)
        {
            SetUp();
            SM.Instance.Keyboard.Press("M");
            SM.Instance.ServiceConsoleState.Setup.Current = SetupOptions.MODE;
            SM.Instance.Keyboard.Press("E"); //EDW
            SM.Instance.ServiceConsoleState.Modes.Current = ModeOptions.EDW;
            SM.Instance.Keyboard.Press("N"); // New treatment

            var orientationOp = edwOptions.Orientation == EDWOrientation.Y1IN ? 1 : 2;
            SM.Instance.Keyboard.EnterNumber(orientationOp);
            SM.Instance.Keyboard.PressEnter();
            SM.Instance.Keyboard.EnterNumber(edwOptions.Y1);
            SM.Instance.Keyboard.PressEnter();
            SM.Instance.Keyboard.EnterNumber(edwOptions.Y2);
            SM.Instance.Keyboard.PressEnter();

            var angle = AccessoryHelper.GetEDWAngleNumber(edwOptions.Angle);
            SM.Instance.Keyboard.EnterNumber(angle);
            SM.Instance.Keyboard.PressEnter();
        }

        public static bool SetCone(ConeOptions cone)
        {
            if (cone != SM.Instance.ServiceConsoleState.Cones.Current)
            {
                SetUp();
                SM.Instance.Keyboard.Press("A");
                SM.Instance.ServiceConsoleState.Setup.Current = SetupOptions.ACCESSORIES;
                SM.Instance.Wait(200);

                SM.Instance.ServiceConsoleState.Cones.Select(cone);
                SM.Instance.Keyboard.PressEnter();
                SM.Instance.Wait(200);

                SM.Instance.MachineState.Accessory = cone.ToString();
                SM.Instance.ResetConsoleState();
                return true;
            }
            return false;//Not set
        }
    }
}
