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

        public TimerLogger(int msWait, int countingSpeedMs, Logger log)
        {
            this.log = log;
            timeLeft = (double)msWait / 1000.0;
            timer = new Timer(TimerCallback, null, 0, countingSpeedMs);
            this.countingSpeed = (double)countingSpeedMs / 1000.0;
        }

        private void TimerCallback(Object o)
        {
            timeLeft = timeLeft >  countingSpeed? timeLeft - countingSpeed : 0;
            Console.WriteLine($"\r{timeLeft.ToString("F2")} sec left");
            if (timeLeft == 0) { GC.Collect(); }
        }

        public void Dispose()
        {
            timer.Dispose();
        }
    }
}
