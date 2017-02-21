using M = Autodrive.Linacs.Varian.CSeries.ServiceModeTableOptions.ModeOptions;

namespace Autodrive.Linacs.Varian.CSeries.ServiceModeTables
{
    public class ModeTable : NavigationTable<M>
    {
        public ModeTable()
        {
            table = new M[5][];
            table[0] = new[] {M.FIXED};
            table[1] = new[] {M.ARC};
            table[2] = new[] {M.TOTAL_BODY};
            table[3] = new[] {M.HDTSE};
            table[4] = new[] {M.EDW};
            Current = M.FIXED;
        }
    }
}