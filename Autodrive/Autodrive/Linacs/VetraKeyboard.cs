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
            if (!_sp.IsOpen)
                _sp.Open();
        }

        public bool Press(string characters)
        {
            Debug.Write(characters);
            var ascii = ASCIIEncoding.ASCII.GetBytes(characters);
            _sp.WriteTimeout = 1000;
            try
            {
                _sp.Write(ascii, 0, ascii.Length);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public bool Press(char c)
        {
            try
            {
                _sp.WriteTimeout = 1000;
                _sp.Write(new char[] { c }, 0, 1);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        private bool Send(byte code)
        {
            var bytes = new byte[] { code };
            try
            {
                _sp.WriteTimeout = 1000;
                _sp.Write(bytes, 0, bytes.Length);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public bool PressEnter()
        {
            var enter = char.ConvertFromUtf32(13);
            return Press(enter);
        }

        public bool PressEsc()
        {
            var esc = char.ConvertFromUtf32(27);
            return Press(esc);
        }

        public bool PressLeft(int moveLeftAmount, int msDelay)
        {
            var success = true;
            for (int i = 0; i < moveLeftAmount; i++)
            {
                success = success & Send(0xD7);
                Thread.Sleep(msDelay);
            }
            return success;
        }

        public bool PressRight(int moveRightAmount, int msDelay)
        {
            var success = true;
            for (int i = 0; i < moveRightAmount; i++)
            {
                success = success & Send(0xD8);
                Thread.Sleep(msDelay);
            }
            return success;
        }

        public bool PressDown(int moveDownAmount, int msDelay)
        {
            var success = true;
            for (int i = 0; i < moveDownAmount; i++)
            {
                success = success & Send(0xD6);
                Thread.Sleep(msDelay);
            }
            return success;
        }

        public bool PressUp(int moveUpAmount, int msDelay)
        {
            bool success = true;
            for (int i = 0; i < moveUpAmount; i++)
            {
                success = success & Send(0xD5);
                Thread.Sleep(msDelay);
            }
            return success;
        }

        public bool EnterNumber(double num)
        {
            var numStr = num.ToString("f1");
            return Press(numStr);
        }

        public bool EnterNumber(int num)
        {
            var numStr = num.ToString();
            return Press(numStr);
        }

        public bool PressF2()
        {
            return Send(0xa1);
        }
    }
}
