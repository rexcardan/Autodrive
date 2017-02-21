using A = Autodrive.Linacs.Varian.CSeries.ServiceModeTableOptions.ConeOptions;

namespace Autodrive.Linacs.Varian.CSeries.ServiceModeTables
{
    public class ConeTable : NavigationTable<A>
    {
        public ConeTable()
        {
            table = new A[6][];
            table[0] = new[] {A.A6};
            table[1] = new[] {A.A10};
            table[2] = new[] {A.A15};
            table[3] = new[] {A.A20};
            table[4] = new[] {A.A25};
            table[5] = new[] {A.A10x6};
            Current = A.A6;
        }

        public override void Select(A option)
        {
            switch (option)
            {
                case A.A6:
                    ServiceModeSession.Instance.Keyboard.EnterNumber(0);
                    break;
                case A.A10:
                    ServiceModeSession.Instance.Keyboard.EnterNumber(1);
                    break;
                case A.A15:
                    ServiceModeSession.Instance.Keyboard.EnterNumber(2);
                    break;
                case A.A20:
                    ServiceModeSession.Instance.Keyboard.EnterNumber(3);
                    break;
                case A.A25:
                    ServiceModeSession.Instance.Keyboard.EnterNumber(4);
                    break;
                case A.A10x6:
                    ServiceModeSession.Instance.Keyboard.EnterNumber(5);
                    break;
            }
            Current = option;
        }
    }
}