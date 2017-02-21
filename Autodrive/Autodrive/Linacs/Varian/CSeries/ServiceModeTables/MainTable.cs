using Autodrive.Linacs.Varian.CSeries.ServiceModeTableOptions;

namespace Autodrive.Linacs.Varian.CSeries.ServiceModeTables
{
    public class MainTable : NavigationTable<MainOptions>
    {
        public MainTable()
        {
            table = new MainOptions[1][];
            table[0] = new[]
            {
                MainOptions.BEAM_CTRL, MainOptions.SET_UP, MainOptions.INTERLOCK_TG, MainOptions.DISPLAY,
                MainOptions.MOTOR, MainOptions.CALIB, MainOptions.UTILS, MainOptions.QUIT
            };
            Current = MainOptions.SET_UP;
        }
    }
}