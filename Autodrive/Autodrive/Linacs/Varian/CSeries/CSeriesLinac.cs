using Autodrive.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Autodrive.Linacs.DoseRate;

namespace Autodrive.Linacs.Varian.CSeries
{
    public class CSeriesLinac : ILinacController
    {
        private ServiceModeSession _session;

        public CSeriesLinac()
        {
            BeamCapabilities = new List<BeamCapability>()
                {
                new BeamCapability(Energy._6X, _100, _200, _300,_400,_500,_600),
                new BeamCapability(Energy._15X, _100, _200, _300,_400,_500,_600),
                new BeamCapability(Energy._6MeV, _100, _200, _300,_400,_500,_600),
                new BeamCapability(Energy._9MeV, _100, _200, _300,_400,_500,_600),
                new BeamCapability(Energy._12MeV, _100, _200, _300,_400,_500,_600),
                new BeamCapability(Energy._15MeV, _100, _200, _300,_400,_500,_600),
                new BeamCapability(Energy._18MeV, _100, _200, _300,_400,_500,_600),
                };
        }

        public List<BeamCapability> BeamCapabilities { get; set; }

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

        public void SetMachineState(MachineState ms)
        {
            var current = ServiceModeSession.Instance.MachineState;

            //Do mechanical operations first
            _session.MotionManager.SetGantryAutomatic(ms.CollimatorRot, ms.X1, ms.X2, ms.Y1, ms.Y2, ms.GantryRot);
            //Move couch
            _session.MotionManager.SetCouchAutomatic(ms.CouchVert, ms.CouchLng, ms.CouchLat, ms.CouchRot);

            if (AccessoryHelper.IsEDW(ms.Accessory))
            {
                var edwOptions = AccessoryHelper.GetEDWOptions(ms.Accessory);
                edwOptions.Y1 = ms.Y1;
                edwOptions.Y2 = ms.Y2;

                BeamManager.SetEDW(edwOptions);
            }
        }

        public void BeamOn(int mu)
        {
            throw new NotImplementedException();
        }
    }
}
