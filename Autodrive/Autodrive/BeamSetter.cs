using Autodrive.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Autodrive
{
    public class BeamSetter
    {
        public static void SetBeam(ModeOptions mode, EnergyOptions energy, TreatmentModeOptions tOptions,
         RepRateOptions repRate, int mu, int time, AccessoryOptions acc, EDWOptions edwOptions = null)
        {
            bool setFullBeam = SetBeam(mode, energy, tOptions, repRate, mu, time, edwOptions);
            if (Session.Instance.ServiceConsoleState.Energies.IsPhoton && setFullBeam)
            {
                Session.Instance.ServiceConsoleState.Accessories.Select(acc);
            }
            else if (Session.Instance.ServiceConsoleState.Energies.IsPhoton && !setFullBeam)
            {
                SetAccessory(acc);
            }
            else
            {
                throw new Exception("Cannot set a photon accessory on an electron field!");
            }
            if (setFullBeam)
            {
                Thread.Sleep(Session.Instance.MachineConstraints.EnergySwitchTimeSec * 1000);
            }
        }

        public static void SetBeam(ModeOptions mode, EnergyOptions energy, TreatmentModeOptions tOptions,
    RepRateOptions repRate, int mu, int time, ConeOptions cone)
        {
            bool setFullBeam = SetBeam(mode, energy, tOptions, repRate, mu, time);
            if (!Session.Instance.ServiceConsoleState.Energies.IsPhoton && setFullBeam)
            {
                Session.Instance.ServiceConsoleState.Cones.Select(cone);
            }
            else if (!Session.Instance.ServiceConsoleState.Energies.IsPhoton && !setFullBeam)
            {
                SetCone(cone);
            }
            else
            {
                throw new Exception("Cannot set a electron cone on a photon field!");
            }
            if (setFullBeam)
            {
                Thread.Sleep(Session.Instance.MachineConstraints.EnergySwitchTimeSec * 1000);
            }
        }

        public static bool SetBeam(ModeOptions mode, EnergyOptions energy, TreatmentModeOptions tOptions,
            RepRateOptions repRate, int mu, int time, EDWOptions edwOpts = null)
        {
            bool energyChange = energy != Session.Instance.ServiceConsoleState.Energies.Current;
            bool repRateChange = repRate != Session.Instance.ServiceConsoleState.RepRates.Current;
            bool muChange = mu != Session.Instance.MachineState.MU;
            bool timeChange = time != Session.Instance.MachineState.Time;
            bool edwChange = edwOpts != null &&
                             (edwOpts.Angle != Session.Instance.MachineState.EDWAngle || edwOpts.Orientation != Session.Instance.MachineState.EDWOrient);

            //If energy is different go ahead and set all again
            if (energyChange || edwChange)
            {
                Session.Instance.MachineState.X1 = double.NaN;
                Session.Instance.MachineState.X2 = double.NaN;
                Session.Instance.MachineState.Y1 = double.NaN;
                Session.Instance.MachineState.Y2 = double.NaN;

                Session.Instance.ServiceConsoleState.Main.Select(MainOptions.SET_UP);
                Thread.Sleep(150);
                Session.Instance.ServiceConsoleState.Setup.Select(SetupOptions.SET_ALL);
                Thread.Sleep(150);
                Session.Instance.ServiceConsoleState.Modes.Select(mode);
                Thread.Sleep(150);
                Session.Instance.ServiceConsoleState.TreatmentModes.Select(tOptions);
                Thread.Sleep(150);
                if (mode == ModeOptions.EDW && edwOpts != null)
                {
                    if (edwOpts.Orientation == EDWOrientation.Y1IN)
                    {
                        Session.Instance.Keyboard.Press("1");
                    }
                    else
                    {
                        Session.Instance.Keyboard.Press("2");
                    }
                    Session.Instance.Keyboard.PressEnter();

                    Thread.Sleep(150);
                    Session.Instance.Keyboard.EnterNumber(edwOpts.Y1);
                    Session.Instance.Keyboard.PressEnter();
                    Thread.Sleep(150);
                    Session.Instance.Keyboard.EnterNumber(edwOpts.Y2);
                    Session.Instance.Keyboard.PressEnter();
                    Thread.Sleep(150);
                    int angle;
                    if (!int.TryParse(edwOpts.Angle.ToString().Replace("_", ""), out angle))
                    {
                        throw new Exception("EDW Angle could not be parsed");
                    }
                    Session.Instance.Keyboard.EnterNumber(angle);
                    Session.Instance.Keyboard.PressEnter();
                    Thread.Sleep(150);
                    //Update machine state
                    Session.Instance.MachineState.EDWAngle = edwOpts.Angle;
                    Session.Instance.MachineState.EDWOrient = edwOpts.Orientation;
                }
                Session.Instance.ServiceConsoleState.Energies.Select(energy);
                Thread.Sleep(500);
                Session.Instance.ServiceConsoleState.RepRates.Select(repRate);
                Thread.Sleep(150);
                Session.Instance.Keyboard.EnterNumber(mu);
                Session.Instance.Keyboard.PressEnter();
                Session.Instance.Keyboard.EnterNumber(time);
                Session.Instance.Keyboard.PressEnter();
                Thread.Sleep(200);
                return true;
            }

            //REP RATE
            if (repRateChange)
                SetRepRate(repRate);
            if (muChange)
                SetMU(mu);
            if (timeChange)
                SetTime(time);
            return false;
        }

        public static void SetRepRate(RepRateOptions repRate)
        {
            Session.Instance.ServiceConsoleState.Main.Select(MainOptions.SET_UP);
            Thread.Sleep(150);
            Session.Instance.ServiceConsoleState.Setup.Select(SetupOptions.REP_RATE);
            Thread.Sleep(200);
            Session.Instance.ServiceConsoleState.RepRates.Select(repRate);
            Session.Instance.Keyboard.PressEnter();
            Thread.Sleep(200);
        }

        public static void SetMU(int mu)
        {
            Session.Instance.ServiceConsoleState.Main.Select(MainOptions.SET_UP);
            Thread.Sleep(150);
            Session.Instance.ServiceConsoleState.Setup.Select(SetupOptions.DOSE);
            Thread.Sleep(200);
            Session.Instance.Keyboard.EnterNumber(mu);
            Session.Instance.Keyboard.PressEnter();
            Thread.Sleep(200);
        }

        public static void SetTime(double time)
        {
            Session.Instance.ServiceConsoleState.Main.Select(MainOptions.SET_UP);
            Thread.Sleep(150);
            Session.Instance.ServiceConsoleState.Setup.Select(SetupOptions.TIME);
            Thread.Sleep(200);
            Session.Instance.Keyboard.EnterNumber(time);
            Session.Instance.Keyboard.PressEnter();
            Session.Instance.MachineState.Time = time;
            Thread.Sleep(200);
        }

        public static void SetAccessory(AccessoryOptions acc)
        {
            if (acc != Session.Instance.ServiceConsoleState.Accessories.Current)
            {
                Session.Instance.ServiceConsoleState.Main.Select(MainOptions.SET_UP);
                Thread.Sleep(150);
                Session.Instance.ServiceConsoleState.Setup.Select(SetupOptions.ACCESSORIES);
                Thread.Sleep(200);
                Session.Instance.ServiceConsoleState.Accessories.Select(acc);
                Session.Instance.Keyboard.PressEnter();
                Thread.Sleep(200);
            }
        }

        public static void SetCone(ConeOptions cone)
        {
            if (cone != Session.Instance.ServiceConsoleState.Cones.Current)
            {
                Session.Instance.ServiceConsoleState.Main.Select(MainOptions.SET_UP);
                Thread.Sleep(150);
                Session.Instance.ServiceConsoleState.Setup.Select(SetupOptions.ACCESSORIES);
                Thread.Sleep(200);
                Session.Instance.ServiceConsoleState.Cones.Select(cone);
                Session.Instance.Keyboard.PressEnter();
                Thread.Sleep(200);
            }
        }

        public static void SetGantryAutomatic(double collimatorAngle, double X1, double X2, double Y1, double Y2,
            double gantryAngle)
        {
            //Look for changes first
            bool collChange = collimatorAngle != Session.Instance.MachineState.CollimatorRot && !double.IsNaN(collimatorAngle);
            bool x1Change = X1 != Session.Instance.MachineState.X1 && !double.IsNaN(X1);
            bool x2Change = X2 != Session.Instance.MachineState.X2 && !double.IsNaN(X2);
            bool y1Change = Y1 != Session.Instance.MachineState.Y1 && !double.IsNaN(Y1);
            bool y2Change = Y2 != Session.Instance.MachineState.Y2 && !double.IsNaN(Y2);
            bool gantryChange = gantryAngle != Session.Instance.MachineState.GantryRot && !double.IsNaN(gantryAngle);

            if (collChange || x1Change || x2Change || y1Change || y2Change || gantryChange)
            {
                Session.Instance.ServiceConsoleState.Main.Select(MainOptions.MOTOR);
                Session.Instance.ServiceConsoleState.Motor.Select(MotorOptions.GANTRY_AUTOMATIC);
                double timeToMoveSec = 0;

                //COLLIMATOR ROTATION
                if (collChange)
                {
                    Session.Instance.ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.COLLIMATOR_ROT);
                    Session.Instance.Keyboard.EnterNumber(collimatorAngle);
                    timeToMoveSec =
                        new[]
                        {
                            timeToMoveSec,
                            Math.Abs(collimatorAngle - Session.Instance.MachineState.CollimatorRot)/Session.Instance.MachineConstraints.CollimatorDegPerSec
                        }.Max
                            ();
                    Session.Instance.MachineState.CollimatorRot = collimatorAngle;
                }
                //Y1
                if (y1Change)
                {
                    Session.Instance.ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.COLL_Y1);
                    Session.Instance.Keyboard.EnterNumber(Y1);
                    timeToMoveSec =
                        new[]
                        {
                            timeToMoveSec,
                            Math.Abs(Y1 - (double.IsNaN(Session.Instance.MachineState.Y1) ? -20 : Session.Instance.MachineState.Y1))/
                            Session.Instance.MachineConstraints.YJawCMPerSec
                        }.Max();
                    Session.Instance.MachineState.Y1 = Y1;
                }
                //Y2
                if (y2Change)
                {
                    Session.Instance.ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.COLL_Y2);
                    Session.Instance.Keyboard.EnterNumber(Y2);
                    timeToMoveSec =
                        new[]
                        {
                            timeToMoveSec,
                            Math.Abs(Y2 - (double.IsNaN(Session.Instance.MachineState.Y2) ? -20 : Session.Instance.MachineState.Y2))/
                            Session.Instance.MachineConstraints.YJawCMPerSec
                        }.Max();
                    Session.Instance.MachineState.Y2 = Y2;
                }

                //X1
                if (x1Change)
                {
                    Session.Instance.ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.COLL_X1);
                    Session.Instance.Keyboard.EnterNumber(X1);
                    timeToMoveSec =
                        new[]
                        {
                            timeToMoveSec,
                            Math.Abs(X1 - (double.IsNaN(Session.Instance.MachineState.X1) ? -20 : Session.Instance.MachineState.X1))/
                            Session.Instance.MachineConstraints.XJawCMPerSec
                        }.Max();
                    Session.Instance.MachineState.X1 = X1;
                }

                //X2
                if (x2Change)
                {
                    Session.Instance.ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.COLL_X2);
                    Session.Instance.Keyboard.EnterNumber(X2);
                    timeToMoveSec =
                        new[]
                        {
                            timeToMoveSec,
                            Math.Abs(X2 - (double.IsNaN(Session.Instance.MachineState.X2) ? -20 : Session.Instance.MachineState.X2))/
                            Session.Instance.MachineConstraints.XJawCMPerSec
                        }.Max();
                    Session.Instance.MachineState.X2 = X2;
                }

                //GANTRY ROT
                if (gantryChange)
                {
                    Session.Instance.ServiceConsoleState.GantryAutomatic.MoveTo(GantryAutoOptions.GANTRY_ROT);
                    Session.Instance.Keyboard.EnterNumber(gantryAngle);
                    timeToMoveSec =
                        new[]
                        {timeToMoveSec, Math.Abs(gantryAngle - Session.Instance.MachineState.GantryRot)/Session.Instance.MachineConstraints.GantryDegPerSec}.Max
                            ();
                    Session.Instance.MachineState.GantryRot = gantryAngle;
                }
                //Go to
                Session.Instance.Keyboard.PressF2();
                Session.Instance.Keyboard.PressEsc();
                Session.Instance.ServiceConsoleState.GantryAutomatic.Current = GantryAutoOptions.COLLIMATOR_ROT;
                Console.WriteLine("Waiting {0}s for motion to complete...", timeToMoveSec.ToString("N1"));
                Thread.Sleep((int)timeToMoveSec * 1000);
            }
        }
    }
}
