using Autodrive.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodrive.Logging;
using Autodrive.RS232;
using System.IO.Ports;
using Autodrive.Electrometers.StandardImaging.Enums;
using System.Threading;
using Autodrive.Electrometers.Enums;
using static Autodrive.RS232.RS232Processor;

namespace Autodrive.Electrometers.StandardImaging
{
    public class Max4000 : IElectrometer
    {
        private SerialMessenger mes = null;

        public string ComPort { get; private set; }
        public Logger Logger { get; set; }

        public bool IsZeroed()
        {

            bool need = false;
            mes.SendMessage("*NEEDZ?", resp =>
            {
                bool success;
                var cleaned = ProcessMax4000Response(resp, out success,this.Logger);
                switch (cleaned)
                {
                    case "0": need = false; break;
                    case "1": need = true; break;
                }
            });
            return need;

        }

        public Value GetValue()
        {
            var value = new Value() { Measurement = double.NaN };
            mes.SendMessage("*CURCHG?", (resp) =>
            {
                bool success;
                var cleanedResponse = ProcessMax4000Response(resp, out success,this.Logger);
                var split = cleanedResponse.Split(' ');
                if (split.Length == 2)
                {
                    var number = split[0];
                    var unit = split[1];
                    var multiplier = 1.0;
                    switch (unit)
                    {
                        case "mC": multiplier = 10E-06; break;
                        case "nC": multiplier = 10E-09; break;
                        case "pC": multiplier = 10E-12; break;
                    }
                    double result;
                    if (double.TryParse(number, out result))
                    {
                        value.Measurement = result * multiplier;
                    }
                }
            });
            return value;
        }

        public bool SetRange(Range r)
        {
            var success = false;
            if (r == Range.HIGH)
            {
                mes.SendMessage("*RNG1", resp => ProcessMax4000Response(resp, out success,this.Logger));
            }
            else
            {
                mes.SendMessage("*RNG0", resp => ProcessMax4000Response(resp, out success,this.Logger));
            }
            return success;
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
            mes.Port.ReadTimeout = 222;
            mes.Port.DiscardNull = true;
            mes.Port.ReceivedBytesThreshold = 1;
            mes.Start();

            //Clear the line
            mes.SendMessage(0x03, (resp) => { Console.WriteLine(resp); });
        }

        public bool SetMode(MeasureMode mode)
        {
            var success = false;
            switch (mode)
            {
                case MeasureMode.CHARGE: mes.SendMessage("*CHGMAX?", resp => ProcessMax4000Response(resp, out success,this.Logger)); break;
                case MeasureMode.CHARGE_RATE: mes.SendMessage("*RTCHG?", resp => ProcessMax4000Response(resp, out success,this.Logger)); break;
                case MeasureMode.TRIGGERED: mes.SendMessage("*CHGTHR?", resp => ProcessMax4000Response(resp, out success,this.Logger)); break;
            }
            return success;
        }

        public void StartMeasurement()
        {
            mes.SendMessage("*STARTNP?");
            Thread.Sleep(500);
        }

        public bool Verify()
        {
            var success = false;
            mes.SendMessage("*IDN?", (resp) => ProcessMax4000Response(resp, out success,this.Logger));
            return success;
        }

        public async Task<bool> Zero()
        {
            return await Task.Run<bool>(() =>
            {
                var success = false;
                mes.SendMessage("*AUZ?");
                Thread.Sleep(2000);
                while (GetStatus() == Status.IS_ZEROING)
                {
                    success = true; //At least we know it started
                    Thread.Sleep(1000);
                }
                return success;
            });
        }

        public Status GetStatus()
        {
            var status = Status.UNKNOWN;
            mes.SendMessage("*STATUS?", (resp) =>
            {
                bool success;
                var cleanedResponse = ProcessMax4000Response(resp, out success,this.Logger);
                switch (cleanedResponse)
                {
                    case "0": status = Status.IDLE; break;
                    case "1": status = Status.IS_ZEROING; break;
                    case "2": status = Status.COLLECTING_CHARGE; break;
                    case "3": status = Status.THRESHOLD_RDY_NOT_TRIGGERED; break;
                    case "4": status = Status.OVERLOAD; break;
                }
            });
            return status;
        }

        public DeviceMode GetDeviceMode()
        {
            var status = DeviceMode.UNKNOWN;
            mes.SendMessage("*MODE?", (resp) =>
            {
                bool success;
                var cleanedResponse = ProcessMax4000Response(resp, out success,this.Logger);
                switch (cleanedResponse)
                {
                    case "2": status = DeviceMode.WARM_UP; break;
                    case "3": status = DeviceMode.ZERO; break;
                    case "4": status = DeviceMode.ZERO_PROGRESS; break;
                    case "5": status = DeviceMode.ZERO_DONE; break;
                    case "6": status = DeviceMode.RANGE_SELECT; break;
                    case "7": status = DeviceMode.BIAS; break;
                    case "8": status = DeviceMode.RATE; break;
                    case "9": status = DeviceMode.CHARGE; break;
                    case "10": status = DeviceMode.RATE_CHARGE; break;
                    case "11": status = DeviceMode.COLLECT_CHARGE; break;
                    case "12": status = DeviceMode.COLLECT_RATE_CHARGE; break;
                    case "13": status = DeviceMode.BATTERY_CHARGE; break;
                    case "14": status = DeviceMode.OVERLOAD; break;
                    case "22": status = DeviceMode.THRESHOLD_LEVEL; break;
                }
            });
            return status;
        }

        #region PLUMBING

        public bool SetBias(Bias biasVoltage)
        {
            if (GetBias() != biasVoltage)
            {
                var success = false;
                var biasNumber = "";
                switch (biasVoltage)
                {
                    case Bias.NEG_100PERC: biasNumber = "-100"; break;
                    case Bias.NEG_50PERC: biasNumber = "-50"; break;
                    case Bias.ZERO: biasNumber = "0"; break;
                    case Bias.POS_50PERC: biasNumber = "50"; break;
                    case Bias.POS_100PERC: biasNumber = "100"; break;
                }
                mes.SendMessage($"*BIAS{biasNumber}?", (resp) =>
                {
                ProcessMax4000Response(resp, out success,this.Logger);
                });
                return success;
            }

            return true;
        }

        public Bias GetBias()
        {
            Bias bias = Bias.UNKNOWN;
            mes.SendMessage("*BIAS?", (resp) =>
            {
                bool success;
                var cleanedResponse = ProcessMax4000Response(resp, out success,this.Logger);
                switch (cleanedResponse)
                {
                    case "100": bias = Bias.POS_100PERC; break;
                    case "50": bias = Bias.POS_50PERC; break;
                    case "0": bias = Bias.POS_50PERC; break;
                    case "-50": bias = Bias.NEG_50PERC; break;
                    case "-100": bias = Bias.NEG_100PERC; break;
                }
            });
            return bias;

        }

        public bool Reset()
        {
            var success = false;
            mes.SendMessage("*STOP?", resp => ProcessMax4000Response(resp, out success,this.Logger));
            Thread.Sleep(500);
            return success;
        }

        public void StopMeasurement()
        {
            var success = false;
            mes.SendMessage("*HALT?", resp => ProcessMax4000Response(resp, out success,this.Logger));
        }
        #endregion

    }
}
