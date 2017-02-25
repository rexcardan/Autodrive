using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.Linacs
{
    public class EnergyHelper
    {
        public static bool IsPhoton(Energy key)
        {
            switch (key)
            {
                case Energy._6X:
                case Energy._6FFF:
                case Energy._10X:
                case Energy._10FFF:
                case Energy._15X:
                case Energy._18X:
                    return true;
                default: return false;
            }
        }
    }
}
