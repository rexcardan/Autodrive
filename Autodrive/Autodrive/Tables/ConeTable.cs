using A = Autodrive.Options.ConeOptions;

namespace Autodrive.Tables
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
                    Session.Instance.Keyboard.EnterNumber(0);
                    break;
                case A.A10:
                    Session.Instance.Keyboard.EnterNumber(1);
                    break;
                case A.A15:
                    Session.Instance.Keyboard.EnterNumber(2);
                    break;
                case A.A20:
                    Session.Instance.Keyboard.EnterNumber(3);
                    break;
                case A.A25:
                    Session.Instance.Keyboard.EnterNumber(4);
                    break;
                case A.A10x6:
                    Session.Instance.Keyboard.EnterNumber(5);
                    break;
            }
            Current = option;
        }
    }
}