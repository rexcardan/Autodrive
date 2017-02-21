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
    }
}
