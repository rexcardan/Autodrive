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
        }

        public static void SetMU(int mu)
        {
            SM.Instance.ServiceConsoleState.Main.Select(MainOptions.SET_UP);
            Thread.Sleep(150);
            SM.Instance.ServiceConsoleState.Setup.Select(SetupOptions.DOSE);
            Thread.Sleep(200);
            SM.Instance.Keyboard.EnterNumber(mu);
            SM.Instance.Keyboard.PressEnter();
            Thread.Sleep(200);
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

        internal static void SetEDW(EDWOptions edwOptions)
        {
            throw new NotImplementedException();
        }

        public static void SetCone(ConeOptions cone)
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
            }
        }
    }
}
