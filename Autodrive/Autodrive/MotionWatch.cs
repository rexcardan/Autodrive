using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Autodrive
{
    public class MotionWatch
    {
        List<double> motionsToExecute = new List<double>();

        public MotionWatch()
        {

        }

        public void AddMotion(double currentValue, double nextValue, double speedPerSec)
        {
            var diff = Math.Abs(currentValue - nextValue);
            var sec = diff / speedPerSec + 1.5; //small buffer for small motions
            if (!double.IsNaN(sec))
            {
                motionsToExecute.Add(sec);
                MotionCompleteEvent.Reset();
            }
        }

        public void StartMotionClock()
        {
            if (motionsToExecute.Any())
            {
                MotionCompleteEvent.Reset();
                IsSystemInMotion = true;
                var maxTime = motionsToExecute.Max();
                Console.WriteLine($"Waiting {maxTime * 1000} ms for motion to complete...");
                timer = new Timer(SignalMotionComplete, null, (int)(maxTime * 1000), Timeout.Infinite);
            }
            else
            {
                MotionCompleteEvent.Set();
                IsSystemInMotion = false;
            }
        }

        private void SignalMotionComplete(object state)
        {
            MotionCompleteEvent.Set();
            timer.Dispose();
            IsSystemInMotion = false;
            motionsToExecute.Clear();
        }

        public ManualResetEvent MotionCompleteEvent = new ManualResetEvent(false);
        private Timer timer;

        public bool IsSystemInMotion { get; private set; }
    }
}
