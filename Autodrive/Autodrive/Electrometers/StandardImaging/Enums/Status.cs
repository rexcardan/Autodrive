using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.Electrometers.StandardImaging.Enums
{
    public enum Status
    {
        IDLE = 0,
        IS_ZEROING = 1,
        COLLECTING_CHARGE = 2,
        THRESHOLD_RDY_NOT_TRIGGERED = 3,
        OVERLOAD = 4,
        UNKNOWN = 5
    }
}
