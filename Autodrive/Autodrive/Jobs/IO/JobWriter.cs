using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.Jobs.IO
{
    public class JobWriter
    {
        public static void AppendResult(string file, Job jr)
        {
            var results = JobReader.ReadJson(file);
            results.Add(jr);
            WriteResults(file, results);
        }

        public static void WriteResults(string file, List<Job> results)
        {
            var json = JsonConvert.SerializeObject(results);
            File.WriteAllText(file, json);
        }
    }
}
