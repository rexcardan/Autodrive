using Autodrive.Linacs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.Interfaces
{
    public interface ILinacController
    {
        /// <summary>
        /// Initializes the controller for the linac
        /// </summary>
        void Initialize(string comPort);

        void SetMachineState(MachineState ms);

        /// <summary>
        /// A list of available energies as well as dose rates for that energy
        /// </summary>
        List<BeamCapability> BeamCapabilities { get; }

        void BeamOn(int mu);
    }
}
