using Autodrive.Interfaces;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Autodrive
{
    public class VetraKeyboard : IKeyboard
    {
        private string _comPort;
        private SerialPort _sp;

        public VetraKeyboard(string comPort)
        {
            _comPort = comPort;
            _sp = new SerialPort(comPort);
            _sp.BaudRate = 9600;
            _sp.StopBits = StopBits.One;
            _sp.DataBits = 8;
            _sp.Handshake = Handshake.RequestToSendXOnXOff;
            _sp.Parity = Parity.None;
            _sp.Open();
        }

        public void Press(string characters)
        {
            var ascii = ASCIIEncoding.ASCII.GetBytes(characters);
            _sp.Write(ascii, 0, ascii.Length);
        }

        public void Press(char c)
        {
            var ascii = ASCIIEncoding.ASCII.GetBytes(new char[1] { c });
            _sp.Write(ascii, 0, ascii.Length);
        }

        public void PressEnter()
        {
            var enter = char.ConvertFromUtf32(13);
            Press(enter);
        }

        public void PressEsc()
        {
            var esc = char.ConvertFromUtf32(27);
            Press(esc);
        }

        public void PressLeft(int moveLeftAmount, int msDelay)
        {
            //D7 = 215
            var left = char.ConvertFromUtf32(215);
            Press(left);
        }

        public void PressRight(int moveRightAmount, int msDelay)
        {
            //D8 = 216
            var right = char.ConvertFromUtf32(216);
            Press(right);
        }

        public void PressDown(int moveDownAmount, int msDelay)
        {
            //D6 = 214
            var down = char.ConvertFromUtf32(214);
            Press(down);
        }

        public void EnterNumber(double num)
        {
            var numStr = num.ToString("f1");
            Press(numStr);
        }

        public void EnterNumber(int num)
        {
            var numStr = num.ToString();
            Press(numStr);
        }

        public void PressF2()
        {
            var f2 = char.ConvertFromUtf32(161);
            Press(f2);
        }
    }
}
