using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Autodrive.Logging
{
    public class TimerLogger : IDisposable
    {
        private Timer timer;
        public double timeLeft = 0;
        private Logger log;
        private double countingSpeed;
        private string _message = string.Empty;

        public TimerLogger(string message, int msWait, int countingSpeedMs, Logger log)
        {
            _message = message;
            this.log = log;
            timeLeft = (double)msWait / 1000.0;
            timer = new Timer(TimerCallback, null, 0, countingSpeedMs);
            this.countingSpeed = (double)countingSpeedMs / 1000.0;
            CompletionEvent = new ManualResetEvent(false);
        }

        private void TimerCallback(Object o)
        {
            timeLeft = timeLeft >  countingSpeed? timeLeft - countingSpeed : 0;
            log?.Log($"\r{_message} | {timeLeft.ToString("F2")} sec left");
            if (timeLeft == 0) { CompletionEvent.Set(); GC.Collect(); }
        }

        public ManualResetEvent CompletionEvent { get; set; }

        public void Dispose()
        {
            timer.Dispose();
        }
    }
}
