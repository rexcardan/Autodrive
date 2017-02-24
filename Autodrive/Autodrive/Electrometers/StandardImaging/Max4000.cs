using Autodrive.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodrive.Logging;
using Autodrive.RS232;
using System.IO.Ports;

namespace Autodrive.Electrometers.StandardImaging
{
    public class Max4000 : IElectrometer
    {
        private SerialMessenger mes = null;

        public string ComPort { get; private set; }
        public Logger Logger { get; set; }

        public Value GetValue()
        {
            throw new NotImplementedException();
        }

        public void Initialize(string comPort)
        {
            mes = new SerialMessenger(comPort, "\n");
            ComPort = comPort;

            //Serial Port Options
            if (mes.Port.IsOpen)
                mes.Port.Close();
            mes.Port.PortName = comPort;
            mes.Port.BaudRate = 19200;
            mes.Port.Parity = Parity.None;
            mes.Port.DataBits = 8;
            mes.Port.StopBits = StopBits.One;
            mes.Port.RtsEnable = false;
            mes.Port.DtrEnable = false;
            mes.Port.Handshake = Handshake.None;
            mes.Port.ReadTimeout = 111;
            mes.Port.DiscardNull = true;
            mes.Port.ReceivedBytesThreshold = 1;
            mes.Start();

            //Clear the line
            mes.SendMessage(0x03, (resp)=> { Console.WriteLine(resp); });
        }

        public bool SetMode(MeasureMode mode)
        {
            throw new NotImplementedException();
        }

        public void StartMeasurement()
        {
            throw new NotImplementedException();
        }

        public bool Verify()
        {
            var success = false;
            mes.SendMessage("*IDN?", (resp) => { success = resp.Contains("MAX 4000"); });
            return success;
        }

        public Task<bool> Zero()
        {
            throw new NotImplementedException();
        }
    }
}
