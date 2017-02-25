using Autodrive.Electrometers;
using Autodrive.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.Interfaces
{
    public interface IElectrometer
    {
        /// <summary>
        /// Starts a communication on the specified com port
        /// </summary>
        /// <param name="comPort">the com port to communicate on</param>
        void Initialize(string comPort);

        /// <summary>
        /// The logger to use with this component
        /// </summary>
        Logger Logger { get; set; }

        /// <summary>
        /// Verify the component is present and functioning
        /// </summary>
        /// <returns>true if component is verified, false otherwise</returns>
        bool Verify();

        /// <summary>
        /// Sets the measurement mode of the electrometer
        /// </summary>
        /// <param name="mode">the mode desired</param>
        /// <returns>true if measurement mode was set</returns>
        bool SetMode(MeasureMode mode);

        /// <summary>
        /// Starts the current measurement
        /// </summary>
        void StartMeasurement();

        /// <summary>
        /// Gets the current charge from the device
        /// </summary>
        /// <returns></returns>
        Value GetValue();

        /// <summary>
        /// Zeroes the electrometer
        /// </summary>
        /// <returns>returns true if zeroed</returns>
        Task<bool> Zero();

        bool IsZeroed();

        bool SetBias(Bias bias);

        Bias GetBias();

        bool Reset();

        void StopMeasurement();
    }
}
