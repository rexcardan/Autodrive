using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.Jobs.IO
{
    public class JobResultWriter
    {
        public static void AppendResult(string file, JobResult jr)
        {
            var results = JobResultReader.Read(file);
            results.Add(jr);
            WriteResults(file, results);
        }

        public static void WriteResults(string file, List<JobResult> results)
        {
            var json = JsonConvert.SerializeObject(results);
            File.WriteAllText(file, json);
        }
    }
}
