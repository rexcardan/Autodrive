using R = Autodrive.Options.RepRateOptions;

namespace Autodrive.Tables
{
    public class RepRateTable : NavigationTable<R>
    {
        public RepRateTable()
        {
            table = new R[6][];
            table[0] = new[] {R._100};
            table[1] = new[] {R._200};
            table[2] = new[] {R._300};
            table[3] = new[] {R._400};
            table[4] = new[] {R._500};
            table[5] = new[] {R._600};
            Current = R._600;

            Session.Instance.ServiceConsoleState.Energies.OptionChanged += (e, args) =>
            {
                if (Session.Instance.ServiceConsoleState.Energies.IsPhoton)
                {
                    table = new R[6][];
                    table[0] = new[] {R._100};
                    table[1] = new[] {R._200};
                    table[2] = new[] {R._300};
                    table[3] = new[] {R._400};
                    table[4] = new[] {R._500};
                    table[5] = new[] {R._600};
                    if (Current == R._1000)
                    {
                        Current = R._600;
                    }
                }
                else
                {
                    table = new R[7][];
                    table[0] = new[] {R._100};
                    table[1] = new[] {R._200};
                    table[2] = new[] {R._300};
                    table[3] = new[] {R._400};
                    table[4] = new[] {R._500};
                    table[5] = new[] {R._600};
                    table[6] = new[] {R._1000};
                }
            };
        }

        public override void Select(R option)
        {
            switch (option)
            {
                case R._100:
                    Session.Instance.Keyboard.EnterNumber(1);
                    break;
                case R._200:
                    Session.Instance.Keyboard.EnterNumber(2);
                    break;
                case R._300:
                    Session.Instance.Keyboard.EnterNumber(3);
                    break;
                case R._400:
                    Session.Instance.Keyboard.EnterNumber(4);
                    break;
                case R._500:
                    Session.Instance.Keyboard.EnterNumber(5);
                    break;
                case R._600:
                    Session.Instance.Keyboard.EnterNumber(6);
                    break;
                case R._1000:
                    Session.Instance.Keyboard.EnterNumber(7);
                    break;
            }
        }
    }
}