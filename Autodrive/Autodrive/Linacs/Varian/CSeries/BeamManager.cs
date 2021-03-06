﻿using Autodrive.Linacs.Varian.CSeries.ServiceModeTableOptions;
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
            if (mu != 0)
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
        }

        public static void SetTime(double time)
        {
            SetUp();

            SM.Instance.Keyboard.Press("T");
            SM.Instance.ServiceConsoleState.Setup.Current = SetupOptions.TIME;
            Thread.Sleep(200);
            SM.Instance.Keyboard.EnterNumber(time);
            SM.Instance.Keyboard.PressEnter();
            SM.Instance.MachineState.Time = time;
            Thread.Sleep(200);
            SM.Instance.ResetConsoleState();
        }

        public static void SetDoseRate(DoseRate doseRate)
        {
            SetUp();

            SM.Instance.Keyboard.Press("R");
            SM.Instance.ServiceConsoleState.Setup.Current = SetupOptions.REP_RATE;
            Thread.Sleep(200);
            switch (doseRate)
            {
                case DoseRate._100: SM.Instance.Keyboard.EnterNumber(1); break;
                case DoseRate._200: SM.Instance.Keyboard.EnterNumber(2); break;
                case DoseRate._300: SM.Instance.Keyboard.EnterNumber(3); break;
                case DoseRate._400: SM.Instance.Keyboard.EnterNumber(4); break;
                case DoseRate._500: SM.Instance.Keyboard.EnterNumber(5); break;
                case DoseRate._600: SM.Instance.Keyboard.EnterNumber(6); break;
            }
            Thread.Sleep(200);
            SM.Instance.MachineState.DoseRate = doseRate;
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
                Thread.Sleep(200);

                SM.Instance.ServiceConsoleState.Accessories.Select(acc);
                SM.Instance.Keyboard.PressEnter();
                Thread.Sleep(200);
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
                    case Energy._6E: SM.Instance.ServiceConsoleState.Energies.Select(EnergyOptions.E1); break;
                    case Energy._9E: SM.Instance.ServiceConsoleState.Energies.Select(EnergyOptions.E2); break;
                    case Energy._12E: SM.Instance.ServiceConsoleState.Energies.Select(EnergyOptions.E3); break;
                    case Energy._15E:
                    case Energy._16E: SM.Instance.ServiceConsoleState.Energies.Select(EnergyOptions.E4); break;
                    case Energy._18E:
                    case Energy._20E: SM.Instance.ServiceConsoleState.Energies.Select(EnergyOptions.E5); break;
                }
                SM.Instance.MachineState.Energy = energy;
                SM.Instance.AddWaitTime("Waiting on carriage and bmag", SM.Instance.MachineConstraints.EnergySwitchTimeSec * 1000);
            }
        }

        public static void SetFixed()
        {
            SetUp();
            SM.Instance.Keyboard.Press("M");
            SM.Instance.ServiceConsoleState.Setup.Current = SetupOptions.MODE;
            SM.Instance.Keyboard.Press("F"); //Fixed
            SM.Instance.ServiceConsoleState.Modes.Current = ModeOptions.FIXED;
            SM.Instance.Keyboard.Press("N"); // New treatment
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
                Thread.Sleep(200);

                SM.Instance.ServiceConsoleState.Cones.Select(cone);
                SM.Instance.Keyboard.PressEnter();
                Thread.Sleep(200);

                SM.Instance.MachineState.Accessory = cone.ToString();
                SM.Instance.ResetConsoleState();
                SM.Instance.AddWaitTime("Allowing jaw motion for cone insert", 5000);
                return true;
            }
            return false;//Not set
        }
    }
}
