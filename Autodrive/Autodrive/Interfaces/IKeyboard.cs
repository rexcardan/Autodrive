using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.Interfaces
{
    public interface IKeyboard
    {
        bool Press(char c);
        bool Press(string characters);
        bool PressEnter();
        bool PressEsc();
        bool PressUp(int moveUpAmount, int msDelay);
        bool PressDown(int moveDownAmount, int msDelay);
        bool PressLeft(int moveLeftAmount, int msDelay);
        bool PressRight(int moveRightAmount, int msDelay);
        bool EnterNumber(double num);
        bool EnterNumber(int num);
        bool PressF2();
    }
}
