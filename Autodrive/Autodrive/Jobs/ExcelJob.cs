using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodrive.Interfaces;
using Autodrive.Linacs.Varian.CSeries;
using System.Threading;

namespace Autodrive.Jobs
{
    public class ExcelJob : Job
    {
        public ExcelJob(MachineState ms, int rowIndex) : base(ms)
        {
            this.RowIndex = rowIndex;
        }

        public int RowIndex { get; set; }

        /// <summary>
        /// Takes measurements using the electrometer and linac input
        /// </summary>
        /// <param name="el"></param>
        /// <param name="linac"></param>
        /// <param name="repeatBeam">flags whether or not this is a repeat measurement</param>
        /// <returns></returns>
        public double TakeMeasurement(IElectrometer el, CSeriesLinac linac, bool repeatBeam)
        {
            if (el != null)
                el.StartMeasurement();
            if (linac != null)
            {
                var ms = MachineStateRun;
                if (repeatBeam) { linac.RepeatBeam(); }
                else { linac.BeamOn(); }
                Thread.Sleep(linac.WaitMsForMU(ms.MU, ms.Accessory != null && AccessoryHelper.IsEDW(ms.Accessory)));
                Thread.Sleep(1000); //Extra second to allow electrometer settling
            }

            if (el != null)
                el.StopMeasurement();

            double val = double.NaN;
            if (el != null)
            {
                val = el.GetValue().Measurement;
                el.Reset();
            }
            AddMeasurement(val);
            return val;
        }
    }
}
