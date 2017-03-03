using Autodrive.Jobs.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.Jobs.Processor
{
    /// <summary>
    /// Output factor processor
    /// </summary>
    public class OFProcessor
    {
        public static List<IOTable> GetTableRows(List<JobResult> results)
        {
            List<IOTable> tables = new List<IOTable>();
            //There are possibly multiple energies. Let's split out
            foreach (var energyGroup in results.GroupBy(r => r.MachineStateRun.Energy))
            {
                var energyTable = new IOTable();
                energyTable.Metadata.Add("Energy", energyGroup.Key);
                energyTable.Metadata.Add("Depth of measurement", energyGroup.First().DepthOfMeasurentMM);
                var maxMeasurements = energyGroup.Max(eg => eg.Measurements.Length);
                //header =>  | M1 | M2 | ....
                var tableHeader = Enumerable.Range(0, maxMeasurements).Select((i, m) => i == 0 ? null : $"M{i}").ToArray();
                energyTable.Add(tableHeader);

                //Each state is a different field size (should be)
                foreach (var state in energyGroup)
                {
                    var x = state.MachineStateRun.X1 + state.MachineStateRun.X2;
                    var y = state.MachineStateRun.Y1 + state.MachineStateRun.Y2;
                    var rowheader = $"{x.ToString("F1")} x {y.ToString("F1")}";
                    var measurements = state.Measurements.Select(s => (dynamic)s).ToList();
                    var row = new dynamic[] { rowheader }.Concat(measurements).ToArray();
                    energyTable.Add(row);
                }
                tables.Add(energyTable);
            }
            return tables;
        }
    }
}
