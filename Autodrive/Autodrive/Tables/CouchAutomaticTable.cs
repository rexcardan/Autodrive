using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using C = Autodrive.Options.CouchAutoOptions;
namespace Autodrive.Tables
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
                Session.Instance.ServiceConsoleState.CouchAutomatic.MoveTo(C.VERT);
                Session.Instance.Keyboard.EnterNumber(vert);
                var time =
                    Math.Abs(vert - Session.Instance.MachineState.CouchVert) /
                    Session.Instance.MachineConstraints.CouchMoveCMPerSec;
                timeSeconds = timeSeconds > time ? timeSeconds : time;
                Session.Instance.MachineState.CouchVert = vert;
            }
            return this;
        }

        public CouchAutomaticTable Long(bool change, double lng, ref double timeSeconds)
        {
            if (change)
            {
                Session.Instance.ServiceConsoleState.CouchAutomatic.MoveTo(C.LONG);
                Session.Instance.Keyboard.EnterNumber(lng);
                var time =
                    Math.Abs(lng - Session.Instance.MachineState.CouchLng) /
                    Session.Instance.MachineConstraints.CouchMoveCMPerSec;
                timeSeconds = timeSeconds > time ? timeSeconds : time;
                Session.Instance.MachineState.CouchLng = lng;
            }
            return this;
        }

        public CouchAutomaticTable Lat(bool change, double lat, ref double timeSeconds)
        {
            if (change)
            {
                Session.Instance.ServiceConsoleState.CouchAutomatic.MoveTo(C.LAT);
                Session.Instance.Keyboard.EnterNumber(lat);
                var time =
                    Math.Abs(lat - Session.Instance.MachineState.CouchLat) /
                    Session.Instance.MachineConstraints.CouchMoveCMPerSec;
                timeSeconds = timeSeconds > time ? timeSeconds : time;
                Session.Instance.MachineState.CouchLat = lat;
            }
            return this;
        }

        public CouchAutomaticTable Rot(bool change, double rot, ref double timeSeconds)
        {
            if (change)
            {
                Session.Instance.ServiceConsoleState.CouchAutomatic.MoveTo(C.ROT);
                Session.Instance.Keyboard.EnterNumber(rot);
                var time =
                    Math.Abs(rot - Session.Instance.MachineState.CouchRot) /
                    Session.Instance.MachineConstraints.CouchRotDegPerSec;
                timeSeconds = timeSeconds > time ? timeSeconds : time;
                Session.Instance.MachineState.CouchRot = rot;
            }
            return this;
        }

        public void GoTo()
        {
            Session.Instance.Keyboard.PressF2();
            Session.Instance.Keyboard.PressEsc();
            Session.Instance.ServiceConsoleState.CouchAutomatic.Current = C.VERT;
        }
    }
}
