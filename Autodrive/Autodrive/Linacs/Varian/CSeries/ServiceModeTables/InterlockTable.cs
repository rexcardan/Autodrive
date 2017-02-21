using I = Autodrive.Linacs.Varian.CSeries.ServiceModeTableOptions.InterlockOptions;

namespace Autodrive.Linacs.Varian.CSeries.ServiceModeTables
{
    public class InterlockTable : NavigationTable<I>
    {
        public InterlockTable()
        {
            table = new I[4][];
            table[0] = new[]
            {
                I.DOS2, I.DS12, I.XDP1, I.XDP2, I.XDRS, I.XDR1, I.XDR2, I.EXQ1, I.EXQ2, I.EXQT, I.ION1, I.ION2, I.LVPS,
                I.VAC1, I.FLOW, I.PUMP
            };
            table[1] = new[]
            {
                I.DOS1, I.TIME, I.CDOS, I.CMNR, I.UDRS, I.UDR1, I.UDR2, I.DPSN, I.MOTN, I.COLL, I.IPSN, I.ACC, I.VAC2,
                I.HWFA, I.CTRL, I.KFIL
            };
            table[2] = new[]
            {
                I.MOD, I.HVOC, I.HVCB, I.STPS, I.GAS, I.CNF, I.TDLY, I.STPR, I.PNDT, I.DOOR, I.KEY, I.MODE, I.CARR,
                I.TDRV, I.TARG, I.FOIL
            };
            table[3] = new[]
            {
                I.VSWR, I.BMAG, I.KSOL, I.ENSW, I.AIR, I.GFIL, I.MLC
            };
            Current = I.DOS2;
        }


        public override void Select(I option)
        {
            int row, currentRow, currentCol, col;
            GetPosition(out row, out col, option);
            GetCurrentPosition(out currentRow, out currentCol);

            int rows = table.Length;
            int columns = table[row].Length;

            int moveLeftAmount = currentCol > col ? currentCol - col : 0;
            int moveRightAmount = currentCol < col ? col - currentCol : 0;

            base.Select(option);
            GetCurrentPosition(out currentRow, out currentCol);

            if (moveLeftAmount > 0)
            {
                // Move left one more time (it has alread occurred just need to update current
                if (currentCol == 0)
                {
                    Current = table[currentRow][table[currentRow].Length - 1];
                }
                else
                {
                    Current = table[currentRow][currentCol - 1];
                }
            }
            else if (moveRightAmount > 0)
            {
                // Move left one more time (it has alread occurred just need to update current
                if (table[currentRow].Length - 1 == currentCol)
                {
                    Current = table[currentRow][0];
                }
                else
                {
                    Current = table[currentRow][currentCol + 1];
                }
            }
        }
    }
}