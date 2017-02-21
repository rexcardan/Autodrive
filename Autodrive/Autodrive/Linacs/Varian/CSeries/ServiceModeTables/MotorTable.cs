using M = Autodrive.Linacs.Varian.CSeries.ServiceModeTableOptions.MotorOptions;

namespace Autodrive.Linacs.Varian.CSeries.ServiceModeTables
{
    public class MotorTable : NavigationTable<M>
    {
        public MotorTable()
        {
            table = new M[3][];
            table[0] = new[] {M.MANUAL_MOTION};
            table[1] = new[] {M.GANTRY_AUTOMATIC};
            table[2] = new[] {M.COUCH_AUTOMATIC};
            Current = M.MANUAL_MOTION;
        }
    }
}