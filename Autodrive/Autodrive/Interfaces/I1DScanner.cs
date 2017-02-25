using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.Interfaces
{
    public interface I1DScanner
    {
        double LastKnowPositionMM { get; }
        Task<bool> GoToDepth(double depthMm);
        double GetCurrentDepthMM();
    }
}
