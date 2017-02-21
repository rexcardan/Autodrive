namespace Autodrive.Linacs.Varian.CSeries.ServiceModeTableOptions
{
    public class EDWOptions
    {
        public EDWOrientation Orientation { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }
        public EDWAngle Angle { get; set; }

        public override string ToString()
        {
            return string.Format("{0} at {1} degrees", Orientation, Angle.ToString().Replace("_", ""));
        }
    }
}