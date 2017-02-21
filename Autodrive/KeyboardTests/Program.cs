
using Autodrive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var session = ServiceModeSession.Instance;
            session.Keyboard = new VetraKeyboard("COM3");
            session.KeySpeedMs = 100;
           // session.EnterDefaultPassword();
            session.ResetConsoleState();
            //session.ToggleDefaultInterlocks();


            session.MachineState.GantryRot = 180;
            session.MachineState.CollimatorRot = 180;
            session.MachineState.X1 = 5.0;
            session.MachineState.X2 = 5.0;
            session.MachineState.Y1 = 5.0;
            session.MachineState.Y2 = 5.0;
            session.MachineState.CouchLat = 100.2;
            session.MachineState.CouchVert = 127.9;
            session.MachineState.CouchLng = 54.4;

            //MonthlyMechanicals.InitializePosition();
            //MonthlyMechanicals.CouchStarShot();
            Console.Read();
          //  var tasks = new List<ITask>();
        }
    }
}
