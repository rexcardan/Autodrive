using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cardan.XCel;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using Autodrive.Linacs.Varian.CSeries.ServiceModeTableOptions;
using Autodrive;
using Autodrive.Linacs;
using System.Text.RegularExpressions;

namespace Autodrive.Jobs.IO
{
    public class XCelRowParser
    {
        public static Energy GetEnergy(XCelData header, XCelData row)
        {
            var index = GetIgnoreCaseIndex(header, "Energy");
            Energy en = Energy.NONE;
            if (index != -1)
            {
                var val = row[index];
                if (val is string)
                {
                    var valAsString = (string)val;
                    if (Regex.IsMatch(valAsString, @"^\d+")) { valAsString = "_" + valAsString; }
                    Enum.TryParse<Energy>(valAsString, out en);
                }
            }
            return en;
        }

        public static double GetCollimatorRot(XCelData header, XCelData row)
        {
            return TryGetDouble(header, row, "Collimator", "Collimator Angle", "Coll");
        }

        public static double GetCouchVert(XCelData header, XCelData row)
        {
            return TryGetDouble(header, row, "Vert", "Couch Vert", "Couch Vertical", "Vertical");
        }

        public static double GetCouchRot(XCelData header, XCelData row)
        {
            return TryGetDouble(header, row, "Couch Rot", "Couch Rotation", "Table Angle", "Table Rotation", "Table Rot");
        }

        public static double GetGantryRot(XCelData header, XCelData row)
        {
            return TryGetDouble(header, row, "Gantry Rot", "Gantry Rotation", "Gantry Angle");
        }

        public static int GetMU(XCelData header, XCelData row)
        {
            return TryGetInt(header, row, "MU");
        }

        public static double GetX1(XCelData header, XCelData row)
        {
            return TryGetDouble(header, row, "X1");
        }

        public static double GetX2(XCelData header, XCelData row)
        {
            return TryGetDouble(header, row, "X2");
        }

        public static double GetY1(XCelData header, XCelData row)
        {
            return TryGetDouble(header, row, "Y1");
        }

        public static double GetMeasurementDepth(XCelData header, XCelData row)
        {
            return TryGetDouble(header, row, "Depth mm", "DepthMM", "Chamber Depth mm", "Chamber Depthmm");
        }

        public static double GetY2(XCelData header, XCelData row)
        {
            return TryGetDouble(header, row, "Y2");
        }

        public static IEnumerable<double> GetMeasurements(XCelData header, XCelData row)
        {
            throw new NotImplementedException();
        }

        public static double GetTime(XCelData header, XCelData row)
        {
            return TryGetDouble(header, row, "Time");
        }

        public static DoseRate GetDoseRate(XCelData header, XCelData row)
        {
            switch (TryGetInt(header, row, "Rate", "Dose Rate"))
            {
                case 100: return DoseRate._100;
                case 200: return DoseRate._200;
                case 300: return DoseRate._300;
                case 400: return DoseRate._400;
                case 500: return DoseRate._500;
                case 600: return DoseRate._600;
                case 1000: return DoseRate._1000;
                default: return DoseRate._600;
            }
        }

        public static int GetNMeasurements(XCelData header, XCelData row)
        {
            return TryGetInt(header, row, "N", "Num Measurements", "Number of Measurements");
        }

        public static double GetCouchLng(XCelData header, XCelData row)
        {
            return TryGetDouble(header, row, "Couch Long", "Couch Longitudinal", "Long", "Longitudinal");
        }

        public static double GetCouchLat(XCelData header, XCelData row)
        {
            return TryGetDouble(header, row, "Couch Lat", "Couch Lateral", "Lat", "Lateral");
        }

        public static string GetAccessory(XCelData header, XCelData row)
        {
            return TryGetString(header, row, "Acc", "Accessory");
        }

        public static List<double> ReadMeasurements(XCelData header, XCelData row)
        {
            var mheaders = Enumerable.Range(1, 5).Select(n => $"M{n}");
            return mheaders.Select(h => TryGetDouble(header, row, h)).Where(n => !double.IsNaN(n)).ToList();
        }

        public static int GetIgnoreCaseIndex(XCelData header, params string[] possibilities)
        {
            foreach (var poss in possibilities)
            {
                var index = Array.FindIndex(header.ToArray(), t => t.IndexOf(poss, StringComparison.InvariantCultureIgnoreCase) >= 0);
                if (index != -1) { return index; }
            }
            //Not found
            return -1;
        }

        private static double TryGetDouble(XCelData header, XCelData row, params string[] possibleHeaders)
        {
            var index = GetIgnoreCaseIndex(header, possibleHeaders);
            double dval = double.NaN;
            if (index != -1)
            {
                var val = row[index];
                if (val is double) { return (double)val; }
                if (val is int) { return (double)val; }
                if (val is string) { double.TryParse(val, out dval); }
            }
            return dval;
        }

        public static int TryGetInt(XCelData header, XCelData row, params string[] possibleHeaders)
        {
            var index = GetIgnoreCaseIndex(header, possibleHeaders);
            int ival = 0;
            if (index != -1)
            {
                var val = row[index];
                if (val is int) { return (int)val; }
                if (val is double) { return (int)val; }
                if (val is string) { int.TryParse(val, out ival); }
            }
            return ival;
        }

        public static string TryGetString(XCelData header, XCelData row, params string[] possibleHeaders)
        {
            var index = GetIgnoreCaseIndex(header, possibleHeaders);
            string ival = string.Empty;
            if (index != -1)
            {
                var val = row[index];
                if (val is string) { return val; }
            }
            return ival;
        }
    }
}
