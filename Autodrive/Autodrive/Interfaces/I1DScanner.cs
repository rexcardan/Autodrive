using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.Interfaces
{
    public interface I1DScanner
    {
        Task<bool> GoToDepth(double depthMm);
        double GetCurrentDepthMM();
    }
}
