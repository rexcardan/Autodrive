using Autodrive.Interfaces;
using Autodrive.Linacs.Varian.CSeries.ServiceModeTableOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Autodrive
{
    public class BeamTask : ITask
    {
        public ModeOptions Mode { get; set; }
        public EnergyOptions Energy { get; set; }
        public TreatmentModeOptions TreatmentOptions { get; set; }
        public RepRateOptions RepRate { get; set; }
        public int MU { get; set; }
        public int Time { get; set; }
        public AccessoryOptions Accessory { get; set; }
        public EDWOptions EDWOptions { get; set; }
        public ConeOptions Cone { get; set; }

        public bool IsModedUp { get; private set; }

        public bool IsPhoton
        {
            get { return Energy == EnergyOptions.X1 || Energy == EnergyOptions.X2; }
        }

        public void ModeUp()
        {
            //if (Energy == EnergyOptions.X1 || Energy == EnergyOptions.X2)
            //{
            //    if (Mode == ModeOptions.FIXED)
            //    {
            //        BeamSetter.SetBeam(Mode, Energy, TreatmentModeOptions.NEW_TREATMENT, RepRate, MU, 99, Accessory);
            //    }
            //    else if (Mode == ModeOptions.EDW)
            //    {
            //        var state = ServiceModeSession.Instance.MachineState;
            //        BeamSetter.SetBeam(Mode, Energy, TreatmentModeOptions.NEW_TREATMENT, RepRate, MU, 99, Accessory,
            //            EDWOptions);
            //        state.Y1 = state.Y2 = 0.5;
            //    }
            //}
            //else
            //{
            //    //Electron
            //    BeamSetter.SetBeam(ModeOptions.FIXED, Energy, TreatmentModeOptions.NEW_TREATMENT, RepRate, MU, 99, Cone);
            //}

            IsModedUp = true;
        }

        public void Run(bool sleep = true)
        {
            if (!IsModedUp)
            {
                ModeUp();
            }
            ServiceModeSession.Instance.BeamOn();
            double wedgeFactor = Mode == ModeOptions.EDW ? 1.3 : 1.00;
            double beamTime = (MU / double.Parse(RepRate.ToString().Replace("_", "")) * 60 * 1000) * wedgeFactor;
            if (sleep)
            {
                Thread.Sleep((int)beamTime + 3000);
            }
        }

        public void Repeat()
        {
            ServiceModeSession.Instance.RepeatBeam();
            double wedgeFactor = Mode == ModeOptions.EDW ? 1.3 : 1.00;
            double beamTime = (MU / double.Parse(RepRate.ToString().Replace("_", "")) * 60 * 1000) * wedgeFactor;
            Thread.Sleep((int)beamTime + 3000);
        }

        public override string ToString()
        {
            return string.Format("{0} {1} MU", Mode, MU);
        }
    }
}
