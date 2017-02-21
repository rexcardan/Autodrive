using System;
using System.Linq;

namespace Autodrive.Linacs.Varian.CSeries.ServiceModeTables
{
    public abstract class NavigationTable<T>
    {
        protected T[][] table;

        public T Current { get; set; }

        protected void GetPosition(out int row, out int col, T value)
        {
            row = 0;
            while (!table[row].Contains(value))
            {
                row++;
            }
            col = table[row].ToList().IndexOf(value);
        }

        protected void GetCurrentPosition(out int row, out int col)
        {
            GetPosition(out row, out col, Current);
        }

        public void MoveTo(T option)
        {
            int row, currentRow, currentCol, col;
            GetPosition(out row, out col, option);
            GetCurrentPosition(out currentRow, out currentCol);

            int rows = table.Length;
            int columns = table[row].Length;

            int moveDownAmount = currentRow < row ? row - currentRow : 0;
            int moveUpAmount = currentRow > row ? currentRow - row : 0;
            int moveLeftAmount = currentCol > col ? currentCol - col : 0;
            int moveRightAmount = currentCol < col ? col - currentCol : 0;

            ServiceModeSession.Instance.Keyboard.PressUp(moveUpAmount, 300);
            ServiceModeSession.Instance.Keyboard.PressDown(moveDownAmount, 300);
            ServiceModeSession.Instance.Keyboard.PressLeft(moveLeftAmount, 300);
            ServiceModeSession.Instance.Keyboard.PressRight(moveRightAmount, 300);

            Current = option;
            OnOptionChanged(null);
        }

        public virtual void Select(T option)
        {
            MoveTo(option);
            ServiceModeSession.Instance.Keyboard.PressEnter();
        }

        public event EventHandler OptionChanged;

        protected virtual void OnOptionChanged(EventArgs args)
        {
            EventHandler handler = OptionChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }
    }
}