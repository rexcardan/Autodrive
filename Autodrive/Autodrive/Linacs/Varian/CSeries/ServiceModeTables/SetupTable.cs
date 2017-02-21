using Autodrive.Linacs.Varian.CSeries.ServiceModeTableOptions;

namespace Autodrive.Linacs.Varian.CSeries.ServiceModeTables
{
    public class SetupTable : NavigationTable<SetupOptions>
    {
        public SetupTable()
        {
            table = new SetupOptions[8][];
            table[0] = new[] {SetupOptions.SET_ALL};
            table[1] = new[] {SetupOptions.MODE};
            table[2] = new[] {SetupOptions.ENERGY};
            table[3] = new[] {SetupOptions.REP_RATE};
            table[4] = new[] {SetupOptions.DOSE};
            table[5] = new[] {SetupOptions.TIME};
            table[6] = new[] {SetupOptions.ACCESSORIES};
            table[7] = new[] {SetupOptions.PERFORM_REPEAT};
            Current = SetupOptions.SET_ALL;
        }
    }
}