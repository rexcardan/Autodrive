using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.Electrometers.StandardImaging.Enums
{
    public enum DeviceMode
    {
        UNKNOWN = 0,
        WARM_UP = 2,
        ZERO=3,
        ZERO_PROGRESS=4,
        ZERO_DONE = 5,
        RANGE_SELECT = 6,
        BIAS=7,
        RATE=8,
        CHARGE = 9,
        RATE_CHARGE = 10,
        COLLECT_CHARGE = 11,
        COLLECT_RATE_CHARGE = 12,
        BATTERY_CHARGE = 13,
        OVERLOAD=14,
        THRESHOLD_LEVEL = 22
    }
}
