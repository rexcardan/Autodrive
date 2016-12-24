using A = Autodrive.Options.AccessoryOptions;

namespace Autodrive.Tables
{
    public class AccessoryTable : NavigationTable<A>
    {
        public AccessoryTable()
        {
            table = new A[4][];
            table[0] = new[] {A.W15_IN, A.W30_IN, A.W45_IN, A.W60_IN, A.NO_ACC};
            table[1] = new[] {A.W15_OUT, A.W30_OUT, A.W45_OUT, A.W60_OUT, A.USER_X};
            table[2] = new[] {A.W15_LEFT, A.W30_LEFT, A.W45_LEFT, A.W60_LEFT};
            table[3] = new[] {A.W15_RIGHT, A.W30_RIGHT, A.W45_RIGHT, A.W60_RIGHT};
            Current = A.W15_IN;
        }
    }
}