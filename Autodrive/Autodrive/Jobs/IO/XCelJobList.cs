using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cardan.XCel;
using Autodrive.Interfaces;
using Autodrive.Linacs.Varian.CSeries;

namespace Autodrive.Jobs.IO
{
    public class XCelJobList
    {
        private XCelJobList() { }

        private string _bookPath = string.Empty;

        public static XCelJobList Read(string path)
        {
            var tl = new XCelJobList();
            tl.RowJobs = new List<Tuple<Job, int>>();

            var xcel = new XCelBook(path);
            tl._bookPath = path;

            tl.RowJobs = JobReader.ReadExcelWithRowIndex(path);
            return tl;
        }

        public List<Tuple<Job, int>> RowJobs { get; set; }

        public void Run(CSeriesLinac linac, IElectrometer el, I1DScanner scan1D)
        {
            foreach (var job in RowJobs)
            {
                var measurementsLeft = job.Item1.NumberOfMeasurementsDesired - job.Item1.Measurements.Length;
                if (measurementsLeft > 0)
                {
                    linac.SetMachineState(job.Item1.MachineStateRun);
                }
                if (scan1D != null)
                {
                    scan1D.GoToDepth(job.Item1.DepthOfMeasurentMM).Wait();
                }
                if (el != null && measurementsLeft > 0)
                {
                    for (int i = 0; i < measurementsLeft; i++)
                    {
                        el.StartMeasurement();
                        linac.BeamOn();
                        el.StopMeasurement();
                        var val = el.GetValue();
                        job.Item1.AddMeasurement(val.Measurement);
                    }
                    Save();
                }
            }
        }

        public void Save()
        {
            var xcel = new XCelBook(_bookPath);
            var rows = xcel.GetRows("Autodrive");
            var header = rows[0];

            foreach (var t in RowJobs)
            {
                var row = rows[t.Item2]; //Item2 is row index
                for (int i = 0; i < t.Item1.Measurements.Length; i++)
                {
                    var h = $"M{i + 1}"; //Measurement header (M1,M2,M3...)
                    row = TrySetDouble(header, row, h, t.Item1.Measurements[i]);
                }
                rows[t.Item2] = row;
            }
            xcel.SetRows(rows, "Autodrive");
            xcel.Save();
        }

        private static XCelData TrySetDouble(XCelData header, XCelData row, string measureHeader, double value)
        {
            var index = XCelRowParser.GetIgnoreCaseIndex(header, measureHeader);
            if (index != -1)
                row[index] = value;
            return row;
        }
    }
}
