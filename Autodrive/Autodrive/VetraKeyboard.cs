using Autodrive.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Autodrive
{
    public class VetraKeyboard : IKeyboard
    {
        private string comPort = "COM3";

        private Dictionary<Key, string> asciiTable = new Dictionary<Key, string>()
        {
            {Key.A, "41" }
        };

        public bool PressKey(Key k)
        {
            if (asciiTable.ContainsKey(k))
            {
                var ascii = asciiTable[k];
                SendMessage(ascii);
                return true;
            }
            return false;
        }

        private void SendMessage(string ascii)
        {
            var serial = new Serial
        }
    }
}
