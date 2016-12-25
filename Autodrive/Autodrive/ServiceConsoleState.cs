using Autodrive.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive
{
    public class ServiceConsoleState
    {
        public ServiceConsoleState()
        {
            Main = new MainTable();
            Setup = new SetupTable();
            InterlockTrig = new InterlockTrigTable();
            Interlocks = new InterlockTable();
            Modes = new ModeTable();
            TreatmentModes = new TreatmentModeTable();
            Energies = new EnergyTable();
            RepRates = new RepRateTable(Energies);
            Accessories = new AccessoryTable();
            Cones = new ConeTable();
            Motor = new MotorTable();
            GantryAutomatic = new GantryAutomaticTable();
            CouchAutomatic = new CouchAutomaticTable();
        }

        public MainTable Main { get; set; }
        public SetupTable Setup { get; set; }
        public InterlockTrigTable InterlockTrig { get; set; }
        public InterlockTable Interlocks { get; set; }
        public ModeTable Modes { get; set; }
        public TreatmentModeTable TreatmentModes { get; set; }
        public EnergyTable Energies { get; set; }
        public RepRateTable RepRates { get; set; }
        public AccessoryTable Accessories { get; set; }
        public ConeTable Cones { get; set; }
        public MotorTable Motor { get; set; }
        public GantryAutomaticTable GantryAutomatic { get; set; }
        public CouchAutomaticTable CouchAutomatic { get; private set; }
    }
}
