using Autodrive.Linacs.Varian.CSeries.ServiceModeTableOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            if (accessory == null) { return false; }
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

        public static int GetEDWAngleNumber(EDWAngle angle)
        {
            switch (angle)
            {
                case EDWAngle._10:
                    return 10;
                case EDWAngle._15:
                    return 15;
                case EDWAngle._20:
                    return 20;
                case EDWAngle._25:
                    return 25;
                case EDWAngle._30:
                    return 30;
                case EDWAngle._45:
                    return 45;
                case EDWAngle._60:
                    return 60;
            }
            return 0;
        }

        public static bool IsElectronCone(string accessory)
        {
            if (accessory == null) { return false; }
            return Regex.IsMatch(accessory, @"[Aa]\d{1,2}");
        }

        public static ConeOptions GetElectronCone(string accessory)
        {
            var size = Regex.Match(accessory, @"\d+$");
            var sizeInt = int.Parse(size.Value);
            switch (sizeInt)
            {
                case 6: return ConeOptions.A6;
                case 10: return ConeOptions.A10;
                case 15: return ConeOptions.A15;
                case 20: return ConeOptions.A20;
                case 25: return ConeOptions.A25;
            }
            return ConeOptions.None;
        }
    }
}
