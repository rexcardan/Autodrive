using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.Linacs
{
    public class BeamCapability
    {
        public BeamCapability(Energy energy, params DoseRate[] availableRates)
        {
            this.Energy = energy;
            this.AvailableDoseRates = availableRates.ToList();
        }

        public Energy Energy { get; set; }
        public List<DoseRate> AvailableDoseRates { get; set; } = new List<DoseRate>();
    }
}
