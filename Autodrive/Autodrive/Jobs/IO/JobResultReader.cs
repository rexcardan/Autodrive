using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.Jobs.IO
{
    public class JobResultReader
    {
        public static List<JobResult> Read(string file)
        {
            var json = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<List<JobResult>>(json);
        }
    }
}
