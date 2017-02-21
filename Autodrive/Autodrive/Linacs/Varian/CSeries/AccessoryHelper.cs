using Autodrive.Linacs.Varian.CSeries.ServiceModeTableOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.Linacs.Varian.CSeries
{
    public class AccessoryHelper
    {
        public static string GetEDWString(EDWAngle angle, EDWOrientation orientation)
        {
            return string.Format("{0}{1}", orientation, angle.ToString().Replace("_", ""));
        }

        public static bool IsEDW(string accessory)
        {
            return accessory.StartsWith("Y1IN") || accessory.StartsWith("Y2OUT");
        }

        public static EDWOptions GetEDWOptions(string accessory)
        {
            var options = new EDWOptions();
            options.Orientation = accessory.StartsWith("Y1IN") ? EDWOrientation.Y1IN : EDWOrientation.Y2OUT;
            var numberString = options.Orientation == EDWOrientation.Y1IN ? accessory.Substring(4) : accessory.Substring(5);
            int number = int.Parse(numberString);
            switch (number)
            {
                case 10:
                    options.Angle = EDWAngle._10;
                    break;
                case 15:
                    options.Angle = EDWAngle._15;
                    break;
                case 20:
                    options.Angle = EDWAngle._20;
                    break;
                case 25:
                    options.Angle = EDWAngle._25;
                    break;
                case 30:
                    options.Angle = EDWAngle._30;
                    break;
                case 45:
                    options.Angle = EDWAngle._45;
                    break;
                case 60:
                    options.Angle = EDWAngle._60;
                    break;
            }
            return options;
        }
    }
}
