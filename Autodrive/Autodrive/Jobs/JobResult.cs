using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.Jobs
{
    public class JobResult
    {
        public MachineState MachineStateRun { get; set; }
        public double[] Measurements { get; set; } = new double[0];
        public DateTime TimeMeasured { get; set; }
        public double DepthOfMeasurentMM { get; set; }

        public JobResult(MachineState ms)
        {
            this.MachineStateRun = ms;
            this.TimeMeasured = DateTime.Now;
        }

        public void AddMeasurement(double val)
        {
            var temp = Measurements.ToList();
            temp.Add(val);
            Measurements = temp.ToArray();
        }
    }
}
