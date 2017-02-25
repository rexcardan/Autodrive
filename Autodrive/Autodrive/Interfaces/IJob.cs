using Autodrive.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.Interfaces
{
    public interface IJob
    {
        void Run();
        string SavePath { get; set; }
        int RepeatMeasurements { get; set; }
        Logger Logger { get; set; }
    }
}
