using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Autodrive.Electrometers.PTW.Unidose.PTWUnidose;

namespace Autodrive.Logging
{
    public class Logger
    {
        public delegate void LogHandler(string toLog);

        /// <summary>
        /// The event to subscribe to for logging
        /// </summary>
        public event LogHandler Logged;

        public void Log(string toLogMessage, params object[] args)
        {
            Logged?.Invoke(string.Format(toLogMessage, args));
        }
    }
}
