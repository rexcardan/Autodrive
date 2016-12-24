using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodrive.Options;

namespace Autodrive.Tables
{
    public class EDWTable
    {
        public static void SetEDW(EDWOptions edwOptions)
        {
            if (edwOptions != null)
            {
                if (edwOptions.Orientation == EDWOrientation.Y1IN)
                {
                    Session.Instance.Keyboard.EnterNumber(1);
                }
                else
                {
                    Session.Instance.Keyboard.EnterNumber(2);
                }
                Session.Instance.Keyboard.PressEnter();
                Session.Instance.Wait();

                Session.Instance.Keyboard.EnterNumber(edwOptions.Y1);
                Session.Instance.Keyboard.PressEnter();
                Session.Instance.Wait();

                Session.Instance.Keyboard.EnterNumber(edwOptions.Y2);
                Session.Instance.Keyboard.PressEnter();
                Session.Instance.Wait();

                int angle;
                if (!int.TryParse(edwOptions.Angle.ToString().Replace("_", ""), out angle))
                {
                    throw new Exception("EDW Angle could not be parsed");
                }
                Session.Instance.Keyboard.EnterNumber(angle);
                Session.Instance.Keyboard.PressEnter();
                Session.Instance.Wait();
                //Update machine state
                Session.Instance.MachineState.EDWAngle = edwOptions.Angle;
                Session.Instance.MachineState.EDWOrient = edwOptions.Orientation;
            }
        }
    }
}
