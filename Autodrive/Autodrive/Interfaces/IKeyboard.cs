using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.Interfaces
{
    public interface IKeyboard
    {
        void Press(char c);
        void Press(string characters);
        void PressEnter();
        void PressEsc();
        void PressUp(int moveUpAmount, int msDelay);
        void PressDown(int moveDownAmount, int msDelay);
        void PressLeft(int moveLeftAmount, int msDelay);
        void PressRight(int moveRightAmount, int msDelay);
        void EnterNumber(double num);
        void EnterNumber(int num);
        void PressF2();
    }
}
