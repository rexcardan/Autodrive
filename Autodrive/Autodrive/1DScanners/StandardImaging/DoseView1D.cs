using Autodrive.Interfaces;
using Autodrive.Logging;
using Autodrive.RS232;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Autodrive.RS232.RS232Processor;

namespace Autodrive._1DScanners.StandardImaging
{
    public class DoseView1D : I1DScanner
    {
        #region SI SPECIFICS
        public static readonly int TicksPerMM = 4000;
        public static readonly int MaxTickPosition = 1110000;
        #endregion

        private int lastKnownTickPosition = -1;
        private SerialMessenger mes;

        public double LastKnowPositionMM
        {
            get { return lastKnownTickPosition == -1 ? double.NaN : lastKnownTickPosition / TicksPerMM; }
        }

        public Logger Logger { get; set; }

        public string ComPort { get; private set; }

        public double GetCurrentDepthMM()
        {
            double result = double.NaN;

            mes.SendMessage($"*G?", (resp =>
            {
                bool success = false;
                var response = ProcessDoseView1DResponse(resp, out success, this.Logger);
                if (success)
                {
                    var canParse = int.TryParse(response, out lastKnownTickPosition);
                    if (canParse) result = LastKnowPositionMM;
                }
            }));

            return result;
        }

        public async Task<bool> GoToDepth(double depthMm)
        {
            return await Task.Run<bool>(() =>
            {
                var success = false;
                var reqTicks = (int)(depthMm * TicksPerMM);
                if (reqTicks < MaxTickPosition)
                {
                    mes.SendMessage($"*S{reqTicks}?", (resp =>
                    {
                        ProcessDoseView1DResponse(resp, out success, this.Logger);
                    }));

                    if (success)
                    {
                        while (GetStatus() != Status.IDLE) { Thread.Sleep(500); }
                    }
                }
                else
                {
                    Logger?.Log($"Cannot move to {reqTicks}. Maximum is {MaxTickPosition}");
                }
                if (success) { lastKnownTickPosition = reqTicks; }
                return success;
            });
        }

        public void Initalize(string com)
        {
            mes = new SerialMessenger(com, "\n");
            ComPort = com;

            //Serial Port Options
            if (mes.Port.IsOpen)
                mes.Port.Close();
            mes.Port.PortName = com;
            mes.Port.BaudRate = 19200;
            mes.Port.Parity = Parity.None;
            mes.Port.DataBits = 8;
            mes.Port.StopBits = StopBits.One;
            mes.Port.RtsEnable = false;
            mes.Port.DtrEnable = false;
            mes.Port.Handshake = Handshake.None;
            mes.Port.ReadTimeout = 222;
            mes.Port.WriteTimeout = 222;
            mes.Port.DiscardNull = true;
            mes.Port.ReceivedBytesThreshold = 1;
            mes.Start();
        }

        public Status GetStatus()
        {
            var status = Status.UNKNOWN;
            var statusString = string.Empty;
            mes.SendMessage("*s?", (resp =>
            {
                bool success = false;
                statusString = ProcessDoseView1DResponse(resp, out success, this.Logger);
            }));
            switch (statusString)
            {
                case "0": return Status.NEEDS_INITIALIZE;
                case "1": return Status.IDLE;
                case "2": return Status.IN_MOTION;
            }
            return status;
        }

        public string GetVersion()
        {
            string version = string.Empty;
            mes.SendMessage("*V?", resp =>
            {
                var valid = false;
                version = ProcessDoseView1DResponse(resp, out valid, this.Logger);
            });
            return version;
        }

        public bool SetOrigin()
        {
            var success = false;
            mes.SendMessage("*R?", (resp =>
            {
                var answer = ProcessDoseView1DResponse(resp, out success, this.Logger);
            }));
            return success;
        }
    }
}
