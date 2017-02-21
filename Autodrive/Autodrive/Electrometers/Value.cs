using System.Globalization;
using Autodrive.Electrometers.PTW.Unidose.Enums;

namespace Autodrive.Electrometers
{
    public class Value
    {
        /// <summary>
        ///     Time in seconds since start when value was acquired
        /// </summary>
        public double Time { get; set; }

        public double Measurement { get; set; }

        public ResolutionPercent ResolutionPercent { get; set; }

        public override string ToString()
        {
            return string.Format("{0} at {1}s with {2}", Measurement.ToString("E3", CultureInfo.InvariantCulture),
                Time.ToString("N2"), ResolutionPercent);
        }
    }
}