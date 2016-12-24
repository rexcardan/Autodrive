using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodrive.Interfaces
{
    public interface ITask
    {
        void ModeUp();
        bool IsModedUp { get; }
    }
}
