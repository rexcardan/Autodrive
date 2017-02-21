using E = Autodrive.Linacs.Varian.CSeries.ServiceModeTableOptions.EnergyOptions;

namespace Autodrive.Linacs.Varian.CSeries.ServiceModeTables
{
    public class EnergyTable : NavigationTable<E>
    {
        public EnergyTable()
        {
            table = new E[8][];
            table[0] = new[] {E.NO_MODE};
            table[1] = new[] {E.X1};
            table[2] = new[] {E.X2};
            table[3] = new[] {E.E1};
            table[4] = new[] {E.E2};
            table[5] = new[] {E.E3};
            table[6] = new[] {E.E4};
            table[7] = new[] {E.E5};
            Current = E.NO_MODE;
        }

        public bool IsPhoton
        {
            get { return Current == E.X1 || Current == E.X2; }
        }

        public override void Select(E option)
        {
            switch (option)
            {
                case E.NO_MODE:
                    ServiceModeSession.Instance.Keyboard.EnterNumber(0);
                    break;
                case E.X1:
                    ServiceModeSession.Instance.Keyboard.EnterNumber(2);
                    break;
                case E.X2:
                    ServiceModeSession.Instance.Keyboard.EnterNumber(1);
                    break;
                case E.E1:
                    ServiceModeSession.Instance.Keyboard.EnterNumber(3);
                    break;
                case E.E2:
                    ServiceModeSession.Instance.Keyboard.EnterNumber(4);
                    break;
                case E.E3:
                    ServiceModeSession.Instance.Keyboard.EnterNumber(5);
                    break;
                case E.E4:
                    ServiceModeSession.Instance.Keyboard.EnterNumber(6);
                    break;
                case E.E5:
                    ServiceModeSession.Instance.Keyboard.EnterNumber(7);
                    break;
            }

            Current = option;
        }
    }
}