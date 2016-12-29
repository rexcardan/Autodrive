using Autodrive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardTests
{
    public class MonthlyMechanicals
    {
        public static void InitializePosition()
        {
            var mt = new MechanicalTask();
            mt.GantryAngle = 180;
            mt.CollimatorAngle = 180;
            mt.CouchLat = 100;
            mt.CouchLong = 100;
            mt.CouchRot = 180;
            mt.CouchVert = 100;
            mt.X1 = mt.X2 = mt.Y1 = mt.Y2 = 5.0;

            mt.ModeUp();
        }

        public static void CouchStarShot()
        {
            var mt = new MechanicalTask();
            mt.GantryAngle = 180;
            mt.CollimatorAngle = 180;
            mt.CouchLat = 100;
            mt.CouchLong = 100;
            mt.CouchVert = 100;
            mt.X1 = mt.X2 = mt.Y1 = mt.Y2 = 5.0;

            var bt = new BeamTask();
            bt.Energy = Autodrive.Options.EnergyOptions.X1;
            bt.Accessory = Autodrive.Options.AccessoryOptions.NO_ACC;
            bt.Mode = Autodrive.Options.ModeOptions.FIXED;
            bt.MU = 100;
            bt.Time = 99;
            bt.TreatmentOptions = Autodrive.Options.TreatmentModeOptions.NEW_TREATMENT;
            bt.RepRate = Autodrive.Options.RepRateOptions._600;

            foreach (var angle in new double[] {90, 130, 150, 180, 210, 240, 270})
            {
                mt.CouchRot = angle;
                mt.ModeUp();
                bt.ModeUp();
            }
        }
    }
}
