using Autodrive.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.Linacs.Varian.CSeries
{
    public class CSeriesLinac : ILinacController
    {
        private ServiceModeSession _session;

        public void Initialize(string comPort)
        {
            _session = ServiceModeSession.Instance;
            _session.Keyboard = new VetraKeyboard(comPort);
            _session.KeySpeedMs = 100;
            _session.ResetConsoleState();
        }

        public void OverrideDefaultInterlocks()
        {
            _session.ToggleDefaultInterlocks();
        }
    }
}
