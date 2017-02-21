using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autodrive.Electrometers.PTW.Unidose.Enums;
using Timer = System.Timers.Timer;
using Autodrive.Interfaces;
using Autodrive.Logging;

namespace Autodrive.Electrometers.PTW.Unidose
{
    public class PTWUnidose : IElectrometer
    {
        private SerialMessenger mes = null;
        private bool _cancel = false;

        public void Initialize(string comPort)
        {
            ComPort = comPort;
            mes = new SerialMessenger(ComPort);
        }

        public string ComPort { get; private set; }

        public Logger Logger { get; set; }

        public bool Verify()
        {
            if (!GetResponse(Messages.INDENTITY).Contains("UNIDOS"))
            {
                Logger?.Log("PTW Unidos not found on {0}!", ComPort);
                return true;
            }
            Logger?.Log("PTW Unidos found on {0}!", ComPort);
            return false;
        }

        public Correction GetCorrection()
        {
            string resp = GetResponse(Messages.CURRENT_CORRECTIONS);
            string[] split = resp.Split(' ');
            var correction = new Correction();
            MethodOfCorrection method;
            ValidityOfCorrection validity;
            double product;
            double factor;
            double additionalFactor;
            if (Enum.TryParse(split[0], out method))
                correction.Method = method;
            if (Enum.TryParse(split[1], out validity))
                correction.Validity = validity;
            if (double.TryParse(split[2], out product))
                correction.CorrectionFactorsProduct = product;
            if (double.TryParse(split[3], out factor))
                correction.CorrectionFactor = factor;
            if (double.TryParse(split[4], out additionalFactor))
                correction.AdditionalCorrectionFactor = additionalFactor;
            return correction;
        }

        public MeasureRange GetRange()
        {
            string resp = GetResponse(Messages.CURRENT_RANGE).Trim();
            string[] split = resp.Split(' ');
            var meas = new MeasureRange();
            Range r;
            double max;

            if (Enum.TryParse(split[0].ToUpper(), out r))
                meas.Range = r;
            if (double.TryParse(split[1], out max))
                meas.MaxValue = max;
            string symbol = split[2];
            switch (symbol)
            {
                case "f":
                    meas.Multiplier = Math.Pow(10, -15);
                    break;
                case "p":
                    meas.Multiplier = Math.Pow(10, -12);
                    break;
                case "n":
                    meas.Multiplier = Math.Pow(10, -9);
                    break;
                case "µ":
                    meas.Multiplier = Math.Pow(10, -6);
                    break;
                case "m":
                    meas.Multiplier = Math.Pow(10, -3);
                    break;
                case "k":
                    meas.Multiplier = Math.Pow(10, 3);
                    break;
                case "M":
                    meas.Multiplier = Math.Pow(10, 6);
                    break;
                default:
                    meas.Multiplier = 1;
                    break;
            }
            return meas;
        }

        public bool SetRange(Range range)
        {
            switch (range)
            {
                case Range.LOW:
                    if (GetResponse(Messages.RANGE_LOW) == Messages.RANGE_LOW)
                        return true;
                    break;
                case Range.MED:
                    if (GetResponse(Messages.RANGE_MEDIUMHIGH) == Messages.RANGE_MEDIUMHIGH)
                        return true;
                    break;
                case Range.HIGH:
                    if (GetResponse(Messages.RANGE_HIGH) == Messages.RANGE_HIGH)
                        return true;
                    break;
            }
            return false;
        }

        public bool SetMode(MeasureMode mode)
        {
            switch (mode)
            {
                case MeasureMode.DOSE:
                    if (GetResponse(Messages.MEASURE_DOSE) == Messages.MEASURE_DOSE)
                        return true;
                    break;
                case MeasureMode.DOSE_RATE:
                    if (GetResponse(Messages.MESAURE_DOSERATE) == Messages.MESAURE_DOSERATE)
                        return true;
                    break;
                case MeasureMode.INT_DOSE_RATE:
                    if (GetResponse(Messages.MESAURE_INTEGRATED) == Messages.MESAURE_INTEGRATED)
                        return true;
                    break;
            }
            return false;
        }

        public void StartMeasurement()
        {
            Logger?.Log("Starting measurement...");
            string resp = string.Empty;
            if ((resp = GetResponse(Messages.START)) == Messages.START)
            {
                Logger?.Log("Measurement successfully started.");
            }
            else
            {
                Logger?.Log("Measurement not started {0}.", resp);
            }
            Thread.Sleep(1000); //Operation takes 1s
        }

        public Value GetValue()
        {
            string resp = GetResponse(Messages.CURRENT_VALUE).Trim();
            string[] split = resp.Split(' ').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            var val = new Value();
            double time;
            double measure;
            if (double.TryParse(split[0].Replace("s", ""), out time))
                val.Time = time;
            if (double.TryParse(split[1], out measure))
                val.Measurement = measure;
            byte[] test = Encoding.ASCII.GetBytes(split[2]);
            if (Encoding.ASCII.GetBytes(split[2]).First() == 251)
                val.ResolutionPercent = ResolutionPercent.LESS_THAN_HALF;
            if (split[2] == "*")
                val.ResolutionPercent = ResolutionPercent.GREATER_THAN_HALF_LESS_ONE;
            if (split[2] == "**")
                val.ResolutionPercent = ResolutionPercent.GREATER_THAN_ONE;

            Logger?.Log("Measured value {0}", val);
            return val;
        }

        public async Task<bool> Zero()
        {
            Logger?.Log("Zeroing Unidose device on {0}", mes.CommName);
            string resp = GetResponse(Messages.NULL);
            if (resp.Contains(Errors.E13.ToString()))
            {
                Logger?.Log("Zeroing failed, must wait {0} s", resp.Split(' ')[1]);
                return false;
            }
            if (resp == Messages.NULL)
            {
                bool result = await Task.Factory.StartNew(() =>
                {
                    var ar = new AutoResetEvent(false);
                    var dStatus = new DeviceStatus();
                    var timer = new Timer();
                    timer.Interval = 2000; // every 2 sec
                    timer.Elapsed += (t, e) =>
                    {
                        dStatus = GetStatus();
                        if (dStatus.Status == Status.RUN || dStatus.Status == Status.RES || dStatus.HasError)
                        {
                            timer.Stop();
                            timer.Dispose();
                            ar.Set();
                        }
                    };
                    timer.Start();
                    ar.WaitOne(); // Wait until valid response
                    timer.Dispose();
                    bool success = dStatus.Status == Status.RUN || dStatus.Status == Status.RES;
                    // returns true if success, false if error;
                    Logger?.Log("Zeroing {0}.", success ? "was successful" : "failed");
                    return success;
                });
                return result;
            }
            return false;
        }

        public DeviceStatus GetStatus()
        {
            Logger?.Log("Getting Unidose device status on {0}...", mes.CommName);
            string resp = GetResponse(Messages.CURRENT_STATUS);
            var status = new DeviceStatus(resp);
            Logger?.Log("Status reported as {0}", status);
            return status;
        }

        public string GetIdentity()
        {
            return GetResponse(Messages.INDENTITY);
        }

        private string GetResponse(string message)
        {
            var ar = new AutoResetEvent(false);
            string resp = string.Empty;
            SerialMessenger.MessageReceivedHandler handler = (com, rMess) =>
            {
                resp = rMess;
                ar.Set();
            };
            mes.MessageReceived += handler;
            mes.SendMessage(message);
            var t = new System.Threading.Timer(state => { ar.Set(); }, null, 3000, Timeout.Infinite);
            ar.WaitOne();
            mes.MessageReceived -= handler;
            return resp;
        }
    }
}