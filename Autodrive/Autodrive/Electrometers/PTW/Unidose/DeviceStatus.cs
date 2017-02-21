using Autodrive.Electrometers.PTW.Unidose.Enums;

namespace Autodrive.Electrometers.PTW.Unidose
{
    public class DeviceStatus
    {
        private readonly string _status;

        public DeviceStatus()
        {
            _status = "N/A";
        }

        public DeviceStatus(string status)
        {
            _status = status;
        }

        public Status Status { get; set; }
        public bool HasError { get; set; }
        public Error Error { get; set; }

        public override string ToString()
        {
            return _status;
        }
    }
}