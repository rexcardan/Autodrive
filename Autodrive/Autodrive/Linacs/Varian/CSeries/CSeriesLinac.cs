using Autodrive.Interfaces;
using Autodrive.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Autodrive.Linacs.DoseRate;

namespace Autodrive.Linacs.Varian.CSeries
{
    public class CSeriesLinac : ILinacController
    {
        private ServiceModeSession _session;
        private int machineStatesSet = 0;
        /// <summary>
        /// The logger to use with this component
        /// </summary>
        public Logger Logger { get; set; }

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
            Logger?.Log($"Initalizing Autodrive on port {comPort}");
         
            _session = ServiceModeSession.Instance;
            if (Logger != null) { _session.Logger = Logger; }
            _session.Keyboard = new VetraKeyboard(comPort);
            _session.KeySpeedMs = 100;
            _session.ResetConsoleState();
        }

        public void OverrideDefaultInterlocks()
        {
            Logger?.Log($"Toggling interlocks...");
            _session.ToggleDefaultInterlocks();
        }

        public void SetMachineState(MachineState ms)
        {
            var current = ServiceModeSession.Instance.MachineState;
            if(machineStatesSet == 0) { BeamManager.SetFixed(); }
            BeamManager.SetEnergy(ms.Energy);

            //Do mechanical operations first
            //Do Gantry Operations
            _session.MotionManager.SetGantryAutomatic(ms.CollimatorRot, ms.X1, ms.X2, ms.Y1, ms.Y2, ms.GantryRot);
            if (machineStatesSet == 0) { Thread.Sleep(5000); }

            //Do Couch Operations
            _session.MotionManager.SetCouchAutomatic(ms.CouchVert, ms.CouchLng, ms.CouchLat, ms.CouchRot);
            if (machineStatesSet == 0) { Thread.Sleep(5000); }

            if (AccessoryHelper.IsEDW(ms.Accessory))
            {
                var edwOptions = AccessoryHelper.GetEDWOptions(ms.Accessory);
                edwOptions.Y1 = ms.Y1;
                edwOptions.Y2 = ms.Y2;
                _session.MachineState.Accessory = ms.Accessory;
                BeamManager.SetEDW(edwOptions);
            }

            else if (AccessoryHelper.IsElectronCone(ms.Accessory))
            {
                var coneOption = AccessoryHelper.GetElectronCone(ms.Accessory);
                BeamManager.SetCone(coneOption);
                _session.MachineState.Accessory = ms.Accessory;
            }

            BeamManager.SetTime(ms.Time);
            BeamManager.SetMU(ms.MU);

            machineStatesSet++;
        }

        public void StopBeam()
        {
            _session.BeamOff();
        }

        public MachineState GetMachineStateCopy()
        {
            return ServiceModeSession.Instance.MachineState.Copy();
        }

        public int WaitMsForMU(int mu, bool isEDW = false)
        {
            //TODO make not just 600 dr
            var ms = (int)((double)mu / 600 * 60 * 1000) + 2500; //add extra 2.5 sec
            if (isEDW)
            {
                var angle = AccessoryHelper.GetEDWOptions(_session.MachineState.Accessory).Angle;
                switch (angle)
                {
                    //These are custom factors to increase time, so we don't move on too quickly
                    case ServiceModeTableOptions.EDWAngle._10: ms = (int)(ms * 1.40); break;
                    case ServiceModeTableOptions.EDWAngle._15: ms = (int)(ms * 1.40); break;
                    case ServiceModeTableOptions.EDWAngle._20: ms = (int)(ms * 1.30); break;
                    case ServiceModeTableOptions.EDWAngle._25: ms = (int)(ms * 1.30); break;
                    case ServiceModeTableOptions.EDWAngle._30: ms = (int)(ms * 1.30); break;
                    case ServiceModeTableOptions.EDWAngle._45: ms = (int)(ms * 1.30); break;
                    case ServiceModeTableOptions.EDWAngle._60: ms = (int)(ms * 1.30); break;
                }
            }
            return ms;
        }

        public void BeamOn()
        {
            _session.Wait();
            _session.BeamOn();
            Thread.Sleep(1000);
        }

        public void RepeatBeam()
        {
            _session.RepeatBeam();
            Thread.Sleep(1000);
        }
    }
}
