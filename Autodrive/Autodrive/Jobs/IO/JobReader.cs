using Cardan.XCel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.Jobs.IO
{
    public class JobReader
    {
        public static List<Job> ReadJson(string file)
        {
            if (File.Exists(file))
            {
                var json = File.ReadAllText(file);
                return JsonConvert.DeserializeObject<List<Job>>(json);
            }
            return new List<Job>();
        }

        public static List<Job> ReadExcel(string file)
        {
            List<Job> jobs = new List<Job>();

            if (File.Exists(file))
            {
                var rows = new XCelBook(file).GetRows("Autodrive");
                var header = rows[0];
                var state = new MachineState();
                foreach (var row in rows.Skip(1).Where(r => r[0] != null && !((string)r[0]).StartsWith("//")).ToList())
                {
                    state.Energy = XCelRowParser.GetEnergy(header, row);
                }
            }
            return jobs;
        }

        public static List<Tuple<Job, int>> ReadExcelWithRowIndex(string file)
        {
            List<Tuple<Job, int>> jobs = new List<Tuple<Job, int>>();

            if (File.Exists(file))
            {
                var rows = new XCelBook(file).GetRows("Autodrive");
                var header = rows[0];
                foreach (var row in rows.Skip(1).Where(r => r[0] != null && !((string)r[0]).StartsWith("//")).ToList())
                {
                    var state = new MachineState();
                    var index = rows.IndexOf(row);

                    state.Accessory = XCelRowParser.GetAccessory(header, row);
                    state.CollimatorRot = XCelRowParser.GetCollimatorRot(header, row);
                    state.CouchLat = XCelRowParser.GetCouchLat(header, row);
                    state.CouchVert = XCelRowParser.GetCouchVert(header, row);
                    state.CouchLng = XCelRowParser.GetCouchLng(header, row);
                    state.CouchRot = XCelRowParser.GetCouchRot(header, row);
                    state.DoseRate = XCelRowParser.GetDoseRate(header, row);
                    state.Energy = XCelRowParser.GetEnergy(header, row);
                    state.GantryRot = XCelRowParser.GetGantryRot(header, row);
                    state.MU = XCelRowParser.GetMU(header, row);
                    state.Time = XCelRowParser.GetTime(header, row);
                    state.X1 = XCelRowParser.GetX1(header, row);
                    state.X2 = XCelRowParser.GetX2(header, row);
                    state.Y1 = XCelRowParser.GetY1(header, row);
                    state.Y2 = XCelRowParser.GetY2(header, row);
                    var job = new Job(state);
                    foreach(var measurement in XCelRowParser.GetMeasurements(header, row))
                    {
                        job.AddMeasurement(measurement);
                    }
                    job.DepthOfMeasurentMM = XCelRowParser.GetMeasurementDepth(header, row);
                    job.NumberOfMeasurementsDesired = XCelRowParser.GetNMeasurements(header, row);
                    jobs.Add(new Tuple<Job, int>(job, index));
                }
            }
            return jobs;
        }
    }
}
