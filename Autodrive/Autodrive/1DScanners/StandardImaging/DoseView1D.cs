using Autodrive.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive._1DScanners.StandardImaging
{
    public class DoseView1D : I1DScanner
    {
        public double GetCurrentDepthMM()
        {
            throw new NotImplementedException();
        }

        public Task<bool> GoToDepth(double depthMm)
        {
            throw new NotImplementedException();
        }
    }
}
