using O = Autodrive.Linacs.Varian.CSeries.ServiceModeTableOptions.TreatmentModeOptions;

namespace Autodrive.Linacs.Varian.CSeries.ServiceModeTables
{
    public class TreatmentModeTable : NavigationTable<O>
    {
        public TreatmentModeTable()
        {
            table = new O[2][];
            table[0] = new[] {O.NEW_TREATMENT};
            table[1] = new[] {O.PARTIAL_TREATMENT};
        }
    }
}