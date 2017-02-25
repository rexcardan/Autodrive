using Autodrive.Linacs;
using Autodrive.Linacs.Varian.CSeries.ServiceModeTableOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive
{
    public class MachineState
    {
        public Energy Energy { get; set; }
        public DoseRate DoseRate { get; set; }
        public int MU { get; set; }
        public double Time { get; set; }
        public double CollimatorRot { get; set; }
        public double X1 { get; set; }
        public double X2 { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }
        public double GantryRot { get; set; }
        public double CouchVert { get; set; }
        public double CouchLng { get; set; }
        public double CouchLat { get; set; }
        public double CouchRot { get; set; }
        public string Accessory { get; set; }

        public static MachineState InitNew()
        {
            return new MachineState()
            {
                MU = 0,
                Time = 0,
                CollimatorRot = 180,
                X1 = 0,
                X2 = 0,
                Y1 = 0,
                Y2 = 0,
                GantryRot = 180,
                CouchVert = 100,
                CouchLng = 100,
                CouchLat = 100,
                CouchRot = 180,
            };
        }

        public MachineState Copy()
        {
            return (MachineState)this.MemberwiseClone();
        }
    }
}
