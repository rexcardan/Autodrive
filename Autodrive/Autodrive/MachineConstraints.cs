using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive
{
    public class MachineConstraints
    {
        public double XJawCMPerSec { get; set; }
        public double YJawCMPerSec { get; set; }
        public double CollimatorDegPerSec { get; set; }
        public double GantryDegPerSec { get; set; }
        public double TableLatCMPerSec { get; set; }
        public double TableVertCMPerSec { get; set; }
        public double TableLongCMPerSec { get; set; }
        public double TableRotDegPerSec { get; set; }
        public int EnergySwitchTimeSec { get; set; }
        public double CouchMoveCMPerSec { get; set; }
        public double CouchVertMoveCMPerSec { get; set; }
        public double CouchRotDegPerSec { get; set; }

        public static MachineConstraints GetDefault()
        {
            return new MachineConstraints()
            {
                XJawCMPerSec = 1.5,
                YJawCMPerSec = 1.0,
                CollimatorDegPerSec = 0.5,
                GantryDegPerSec = 0.8,
                TableLatCMPerSec = 1.5,
                TableVertCMPerSec = 1.5,
                TableLongCMPerSec = 1.5,
                TableRotDegPerSec = 1.5,
                EnergySwitchTimeSec = 6,
                CouchMoveCMPerSec = 2,
                CouchVertMoveCMPerSec = 2,
                CouchRotDegPerSec = 8
            };
        }
    }
}
