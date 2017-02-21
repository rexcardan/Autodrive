using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using C = Autodrive.Linacs.Varian.CSeries.ServiceModeTableOptions.CouchAutoOptions;
namespace Autodrive.Linacs.Varian.CSeries.ServiceModeTables
{
    public class CouchAutomaticTable : NavigationTable<C>
    {
        public CouchAutomaticTable()
        {
            table = new C[4][];
            table[0] = new[] { C.VERT };
            table[1] = new[] { C.LONG };
            table[2] = new[] { C.LAT };
            table[3] = new[] { C.ROT };
            Current = C.VERT;
        }

        public CouchAutomaticTable Vert(bool change, double vert, ref double timeSeconds)
        {
            if (change)
            {
                ServiceModeSession.Instance.ServiceConsoleState.CouchAutomatic.MoveTo(C.VERT);
                ServiceModeSession.Instance.Keyboard.EnterNumber(vert);
                var time =
                    Math.Abs(vert - ServiceModeSession.Instance.MachineState.CouchVert) /
                    ServiceModeSession.Instance.MachineConstraints.CouchMoveCMPerSec;
                timeSeconds = timeSeconds > time ? timeSeconds : time;
                ServiceModeSession.Instance.MachineState.CouchVert = vert;
            }
            return this;
        }

        public CouchAutomaticTable Long(bool change, double lng, ref double timeSeconds)
        {
            if (change)
            {
                ServiceModeSession.Instance.ServiceConsoleState.CouchAutomatic.MoveTo(C.LONG);
                ServiceModeSession.Instance.Keyboard.EnterNumber(lng);
                var time =
                    Math.Abs(lng - ServiceModeSession.Instance.MachineState.CouchLng) /
                    ServiceModeSession.Instance.MachineConstraints.CouchMoveCMPerSec;
                timeSeconds = timeSeconds > time ? timeSeconds : time;
                ServiceModeSession.Instance.MachineState.CouchLng = lng;
            }
            return this;
        }

        public CouchAutomaticTable Lat(bool change, double lat, ref double timeSeconds)
        {
            if (change)
            {
                ServiceModeSession.Instance.ServiceConsoleState.CouchAutomatic.MoveTo(C.LAT);
                ServiceModeSession.Instance.Keyboard.EnterNumber(lat);
                var time =
                    Math.Abs(lat - ServiceModeSession.Instance.MachineState.CouchLat) /
                    ServiceModeSession.Instance.MachineConstraints.CouchMoveCMPerSec;
                timeSeconds = timeSeconds > time ? timeSeconds : time;
                ServiceModeSession.Instance.MachineState.CouchLat = lat;
            }
            return this;
        }

        public CouchAutomaticTable Rot(bool change, double rot, ref double timeSeconds)
        {
            if (change)
            {
                ServiceModeSession.Instance.ServiceConsoleState.CouchAutomatic.MoveTo(C.ROT);
                ServiceModeSession.Instance.Keyboard.EnterNumber(rot);
                var time =
                    Math.Abs(rot - ServiceModeSession.Instance.MachineState.CouchRot) /
                    ServiceModeSession.Instance.MachineConstraints.CouchRotDegPerSec;
                timeSeconds = timeSeconds > time ? timeSeconds : time;
                ServiceModeSession.Instance.MachineState.CouchRot = rot;
            }
            return this;
        }

        public void GoTo()
        {
            ServiceModeSession.Instance.Keyboard.PressF2();
            ServiceModeSession.Instance.Keyboard.PressEsc();
            ServiceModeSession.Instance.ServiceConsoleState.CouchAutomatic.Current = C.VERT;
        }
    }
}
