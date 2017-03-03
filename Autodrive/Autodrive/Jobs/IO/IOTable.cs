using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.Jobs.IO
{
    public class IOTable : List<dynamic[]>
    {
        public Dictionary<string, dynamic> Metadata { get; set; } = new Dictionary<string, dynamic>();
    }
}
