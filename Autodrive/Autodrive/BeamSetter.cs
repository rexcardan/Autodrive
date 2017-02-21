using Autodrive.Linacs.Varian.CSeries.ServiceModeTableOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SM = Autodrive.ServiceModeSession;

namespace Autodrive
{
    public class BeamSetter
    {
    //    public static void SetBeam(ModeOptions mode, EnergyOptions energy, TreatmentModeOptions tOptions,
    //     RepRateOptions repRate, int mu, int time, AccessoryOptions acc, EDWOptions edwOptions = null)
    //    {
    //        bool setFullBeam = SetBeam(mode, energy, tOptions, repRate, mu, time, edwOptions);
    //        if (SM.Instance.ServiceConsoleState.Energies.IsPhoton && setFullBeam)
    //        {
    //            SM.Instance.ServiceConsoleState.Accessories.Select(acc);
    //        }
    //        else if (SM.Instance.ServiceConsoleState.Energies.IsPhoton && !setFullBeam)
    //        {
    //            SetAccessory(acc);
    //        }
    //        else
    //        {
    //            throw new Exception("Cannot set a photon accessory on an electron field!");
    //        }
    //        if (setFullBeam)
    //        {
    //            Thread.Sleep(SM.Instance.MachineConstraints.EnergySwitchTimeSec * 1000);
    //        }
    //    }

    //    public static void SetBeam(ModeOptions mode, EnergyOptions energy, TreatmentModeOptions tOptions,
    //RepRateOptions repRate, int mu, int time, ConeOptions cone)
    //    {
    //        bool setFullBeam = SetBeam(mode, energy, tOptions, repRate, mu, time);
    //        if (!SM.Instance.ServiceConsoleState.Energies.IsPhoton && setFullBeam)
    //        {
    //            SM.Instance.ServiceConsoleState.Cones.Select(cone);
    //        }
    //        else if (!SM.Instance.ServiceConsoleState.Energies.IsPhoton && !setFullBeam)
    //        {
    //            SetCone(cone);
    //        }
    //        else
    //        {
    //            throw new Exception("Cannot set a electron cone on a photon field!");
    //        }
    //        if (setFullBeam)
    //        {
    //            Thread.Sleep(SM.Instance.MachineConstraints.EnergySwitchTimeSec * 1000);
    //        }
    //    }

        //public static bool SetBeam(ModeOptions mode, EnergyOptions energy, TreatmentModeOptions tOptions,
        //    RepRateOptions repRate, int mu, int time, EDWOptions edwOpts = null)
        //{
        //    bool energyChange = energy != SM.Instance.ServiceConsoleState.Energies.Current;
        //    bool repRateChange = repRate != SM.Instance.ServiceConsoleState.RepRates.Current;
        //    bool muChange = mu != SM.Instance.MachineState.MU;
        //    bool timeChange = time != SM.Instance.MachineState.Time;
        //    bool edwChange = edwOpts != null &&
        //                     (edwOpts.Angle != SM.Instance.MachineState.EDWAngle || edwOpts.Orientation != SM.Instance.MachineState.EDWOrient);

        //    //If energy is different go ahead and set all again
        //    if (energyChange || edwChange)
        //    {
        //        SM.Instance.MachineState.X1 = double.NaN;
        //        SM.Instance.MachineState.X2 = double.NaN;
        //        SM.Instance.MachineState.Y1 = double.NaN;
        //        SM.Instance.MachineState.Y2 = double.NaN;

        //        SM.Instance.ServiceConsoleState.Main.Select(MainOptions.SET_UP);
        //        Thread.Sleep(150);
        //        SM.Instance.ServiceConsoleState.Setup.Select(SetupOptions.SET_ALL);
        //        Thread.Sleep(150);
        //        SM.Instance.ServiceConsoleState.Modes.Select(mode);
        //        Thread.Sleep(150);
        //        SM.Instance.ServiceConsoleState.TreatmentModes.Select(tOptions);
        //        Thread.Sleep(150);
        //        if (mode == ModeOptions.EDW && edwOpts != null)
        //        {
        //            if (edwOpts.Orientation == EDWOrientation.Y1IN)
        //            {
        //                SM.Instance.Keyboard.Press("1");
        //            }
        //            else
        //            {
        //                SM.Instance.Keyboard.Press("2");
        //            }
        //            SM.Instance.Keyboard.PressEnter();

        //            Thread.Sleep(150);
        //            SM.Instance.Keyboard.EnterNumber(edwOpts.Y1);
        //            SM.Instance.Keyboard.PressEnter();
        //            Thread.Sleep(150);
        //            SM.Instance.Keyboard.EnterNumber(edwOpts.Y2);
        //            SM.Instance.Keyboard.PressEnter();
        //            Thread.Sleep(150);
        //            int angle;
        //            if (!int.TryParse(edwOpts.Angle.ToString().Replace("_", ""), out angle))
        //            {
        //                throw new Exception("EDW Angle could not be parsed");
        //            }
        //            SM.Instance.Keyboard.EnterNumber(angle);
        //            SM.Instance.Keyboard.PressEnter();
        //            Thread.Sleep(150);
        //            //Update machine state
        //            SM.Instance.MachineState.EDWAngle = edwOpts.Angle;
        //            SM.Instance.MachineState.EDWOrient = edwOpts.Orientation;
        //        }
        //        SM.Instance.ServiceConsoleState.Energies.Select(energy);
        //        Thread.Sleep(500);
        //        SM.Instance.ServiceConsoleState.RepRates.Select(repRate);
        //        Thread.Sleep(150);
        //        SM.Instance.Keyboard.EnterNumber(mu);
        //        SM.Instance.Keyboard.PressEnter();
        //        SM.Instance.Keyboard.EnterNumber(time);
        //        SM.Instance.Keyboard.PressEnter();
        //        Thread.Sleep(200);
        //        return true;
        //    }

        //    //REP RATE
        //    if (repRateChange)
        //        SetRepRate(repRate);
        //    if (muChange)
        //        SetMU(mu);
        //    if (timeChange)
        //        SetTime(time);
        //    return false;
        //}

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

        public static void SetGantryAutomatic(double collimatorAngle, double X1, double X2, double Y1, double Y2,
            double gantryAngle)
        {
            //Look for changes first
            bool collChange = collimatorAngle != SM.Instance.MachineState.CollimatorRot && !double.IsNaN(collimatorAngle);
            bool x1Change = X1 != SM.Instance.MachineState.X1 && !double.IsNaN(X1);
            bool x2Change = X2 != SM.Instance.MachineState.X2 && !double.IsNaN(X2);
            bool y1Change = Y1 != SM.Instance.MachineState.Y1 && !double.IsNaN(Y1);
            bool y2Change = Y2 != SM.Instance.MachineState.Y2 && !double.IsNaN(Y2);
            bool gantryChange = gantryAngle != SM.Instance.MachineState.GantryRot && !double.IsNaN(gantryAngle);

            if (collChange || x1Change || x2Change || y1Change || y2Change || gantryChange)
            {
                SM.Instance.ServiceConsoleState.Main.Select(MainOptions.MOTOR);
                SM.Instance.ServiceConsoleState.Motor.Select(MotorOptions.GANTRY_AUTOMATIC);
                double timeToMoveSec = 0;

                //COLLIMATOR ROTATION
                if (collChange)
                {
                    SM.Instance.ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.COLLIMATOR_ROT);
                    SM.Instance.Keyboard.EnterNumber(collimatorAngle);
                    timeToMoveSec =
                        new[]
                        {
                            timeToMoveSec,
                            Math.Abs(collimatorAngle - SM.Instance.MachineState.CollimatorRot)/SM.Instance.MachineConstraints.CollimatorDegPerSec
                        }.Max
                            ();
                    SM.Instance.MachineState.CollimatorRot = collimatorAngle;
                }
                //Y1
                if (y1Change)
                {
                    SM.Instance.ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.COLL_Y1);
                    SM.Instance.Keyboard.EnterNumber(Y1);
                    timeToMoveSec =
                        new[]
                        {
                            timeToMoveSec,
                            Math.Abs(Y1 - (double.IsNaN(SM.Instance.MachineState.Y1) ? -20 : SM.Instance.MachineState.Y1))/
                            SM.Instance.MachineConstraints.YJawCMPerSec
                        }.Max();
                    SM.Instance.MachineState.Y1 = Y1;
                }
                //Y2
                if (y2Change)
                {
                    SM.Instance.ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.COLL_Y2);
                    SM.Instance.Keyboard.EnterNumber(Y2);
                    timeToMoveSec =
                        new[]
                        {
                            timeToMoveSec,
                            Math.Abs(Y2 - (double.IsNaN(SM.Instance.MachineState.Y2) ? -20 : SM.Instance.MachineState.Y2))/
                            SM.Instance.MachineConstraints.YJawCMPerSec
                        }.Max();
                    SM.Instance.MachineState.Y2 = Y2;
                }

                //X1
                if (x1Change)
                {
                    SM.Instance.ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.COLL_X1);
                    SM.Instance.Keyboard.EnterNumber(X1);
                    timeToMoveSec =
                        new[]
                        {
                            timeToMoveSec,
                            Math.Abs(X1 - (double.IsNaN(SM.Instance.MachineState.X1) ? -20 : SM.Instance.MachineState.X1))/
                            SM.Instance.MachineConstraints.XJawCMPerSec
                        }.Max();
                    SM.Instance.MachineState.X1 = X1;
                }

                //X2
                if (x2Change)
                {
                    SM.Instance.ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.COLL_X2);
                    SM.Instance.Keyboard.EnterNumber(X2);
                    timeToMoveSec =
                        new[]
                        {
                            timeToMoveSec,
                            Math.Abs(X2 - (double.IsNaN(SM.Instance.MachineState.X2) ? -20 : SM.Instance.MachineState.X2))/
                            SM.Instance.MachineConstraints.XJawCMPerSec
                        }.Max();
                    SM.Instance.MachineState.X2 = X2;
                }

                //GANTRY ROT
                if (gantryChange)
                {
                    SM.Instance.ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.GANTRY_ROT);
                    SM.Instance.Keyboard.EnterNumber(gantryAngle);
                    timeToMoveSec =
                        new[]
                        {timeToMoveSec, Math.Abs(gantryAngle - SM.Instance.MachineState.GantryRot)/SM.Instance.MachineConstraints.GantryDegPerSec}.Max
                            ();
                    SM.Instance.MachineState.GantryRot = gantryAngle;
                }
                //Go to
                SM.Instance.Keyboard.PressF2();
                SM.Instance.Keyboard.PressEsc();
                SM.Instance.ServiceConsoleState.GantryAutomatic.Current = GantryAutoOptions.COLLIMATOR_ROT;
                Console.WriteLine("Waiting {0}s for motion to complete...", timeToMoveSec.ToString("N1"));
                Thread.Sleep((int)timeToMoveSec * 1000);
            }
        }
    }
}
