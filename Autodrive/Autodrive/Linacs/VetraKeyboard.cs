using Autodrive.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
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
            Debug.Write(characters);
            var ascii = ASCIIEncoding.ASCII.GetBytes(characters);
            _sp.WriteTimeout = 500;
            try
            {
                _sp.Write(ascii, 0, ascii.Length);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public void Press(char c)
        {
            try
            {
                _sp.WriteTimeout = 500;
                _sp.Write(new char[] { c }, 0, 1);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private void Send(byte code)
        {
            var bytes = new byte[] { code };
            try
            {
                _sp.WriteTimeout = 500;
                _sp.Write(bytes, 0, bytes.Length);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
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
            for (int i = 0; i < moveLeftAmount; i++)
            {
                Send(0xD7);
                Thread.Sleep(msDelay);
            }
        }

        public void PressRight(int moveRightAmount, int msDelay)
        {
            for (int i = 0; i < moveRightAmount; i++)
            {
                Send(0xD8);
                Thread.Sleep(msDelay);
            }
        }

        public void PressDown(int moveDownAmount, int msDelay)
        {
            for (int i = 0; i < moveDownAmount; i++)
            {
                Send(0xD6);
                Thread.Sleep(msDelay);
            }
        }

        public void PressUp(int moveUpAmount, int msDelay)
        {
            for (int i = 0; i < moveUpAmount; i++)
            {
                Send(0xD5);
                Thread.Sleep(msDelay);
            }
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
            Send(0xa1);
        }
    }
}
