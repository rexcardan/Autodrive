using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodrive.Linacs.Varian.CSeries.ServiceModeTableOptions;

namespace Autodrive.Linacs.Varian.CSeries.ServiceModeTables
{
    public class EDWTable
    {
        public static void SetEDW(EDWOptions edwOptions)
        {
            if (edwOptions != null)
            {
                if (edwOptions.Orientation == EDWOrientation.Y1IN)
                {
                    ServiceModeSession.Instance.Keyboard.EnterNumber(1);
                }
                else
                {
                    ServiceModeSession.Instance.Keyboard.EnterNumber(2);
                }
                ServiceModeSession.Instance.Keyboard.PressEnter();
                ServiceModeSession.Instance.Wait();

                ServiceModeSession.Instance.Keyboard.EnterNumber(edwOptions.Y1);
                ServiceModeSession.Instance.Keyboard.PressEnter();
                ServiceModeSession.Instance.Wait();

                ServiceModeSession.Instance.Keyboard.EnterNumber(edwOptions.Y2);
                ServiceModeSession.Instance.Keyboard.PressEnter();
                ServiceModeSession.Instance.Wait();

                int angle;
                if (!int.TryParse(edwOptions.Angle.ToString().Replace("_", ""), out angle))
                {
                    throw new Exception("EDW Angle could not be parsed");
                }
                ServiceModeSession.Instance.Keyboard.EnterNumber(angle);
                ServiceModeSession.Instance.Keyboard.PressEnter();
                ServiceModeSession.Instance.Wait();
                //Update machine state
                ServiceModeSession.Instance.MachineState.EDWAngle = edwOptions.Angle;
                ServiceModeSession.Instance.MachineState.EDWOrient = edwOptions.Orientation;
            }
        }
    }
}
