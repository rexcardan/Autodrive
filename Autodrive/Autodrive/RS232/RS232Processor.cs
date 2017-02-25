using Autodrive.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.RS232
{
    public class RS232Processor
    {
        public static string ProcessMax4000Response(string response, out bool isValid, Logger logger = null)
        {
            response = response.Trim();
            isValid = response.StartsWith("=>");
            if (response.StartsWith("?>"))
            {
                logger?.Log("Command error was detected. Doesn't understand input command.");
            }
            else if (response.StartsWith("!>"))
            {
                logger?.Log("Execution error was detected. Command syntax is correct, but couldn't execute for some reason");
            }
            return response.Replace("=>", "").Replace("\r", "").Replace("\n", "").Trim();
        }

        public static string ProcessDoseView1DResponse(string response, out bool isValid, Logger logger = null)
        {
            isValid = response.EndsWith("!>");
            if (isValid)
            {
                return response.Replace("<", "").Replace("!>", "").Trim();
            }
            else if (response.EndsWith("?>"))
            {
                logger?.Log("Command error was detected. Doesn't understand input command.");
            }
            else if (!response.EndsWith("!>"))
            {
                logger?.Log("Execution error was detected. Command syntax is correct, but couldn't execute for some reason");
            }
            return string.Empty;
        }
    }
}
