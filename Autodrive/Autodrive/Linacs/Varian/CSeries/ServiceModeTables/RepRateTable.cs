﻿using R = Autodrive.Linacs.Varian.CSeries.ServiceModeTableOptions.RepRateOptions;

namespace Autodrive.Linacs.Varian.CSeries.ServiceModeTables
{
    public class RepRateTable : NavigationTable<R>
    {
        public RepRateTable(EnergyTable et)
        {
            table = new R[6][];
            table[0] = new[] {R._100};
            table[1] = new[] {R._200};
            table[2] = new[] {R._300};
            table[3] = new[] {R._400};
            table[4] = new[] {R._500};
            table[5] = new[] {R._600};
            Current = R._600;

            et.OptionChanged += (e, args) =>
            {
                if (et.IsPhoton)
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
                    ServiceModeSession.Instance.Keyboard.EnterNumber(1);
                    break;
                case R._200:
                    ServiceModeSession.Instance.Keyboard.EnterNumber(2);
                    break;
                case R._300:
                    ServiceModeSession.Instance.Keyboard.EnterNumber(3);
                    break;
                case R._400:
                    ServiceModeSession.Instance.Keyboard.EnterNumber(4);
                    break;
                case R._500:
                    ServiceModeSession.Instance.Keyboard.EnterNumber(5);
                    break;
                case R._600:
                    ServiceModeSession.Instance.Keyboard.EnterNumber(6);
                    break;
                case R._1000:
                    ServiceModeSession.Instance.Keyboard.EnterNumber(7);
                    break;
            }
        }
    }
}