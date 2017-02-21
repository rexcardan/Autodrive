using A = Autodrive.Linacs.Varian.CSeries.ServiceModeTableOptions.GantryAutoOptions;

namespace Autodrive.Linacs.Varian.CSeries.ServiceModeTables
{
    public class GantryAutomaticTable : NavigationTable<A>
    {
        public GantryAutomaticTable()
        {
            table = new A[4][];
            table[0] = new[] {A.COLLIMATOR_ROT, A.COLL_Y1};
            table[1] = new[] {A.FIELD_Y, A.COLL_Y2};
            table[2] = new[] {A.FIELD_X, A.COLL_X1};
            table[3] = new[] {A.GANTRY_ROT, A.COLL_X2};
            Current = A.COLLIMATOR_ROT;
        }
    }
}