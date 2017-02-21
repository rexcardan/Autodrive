using Autodrive.Electrometers.PTW.Unidose.Enums;

namespace Autodrive.Electrometers.PTW.Unidose
{
    public class Correction
    {
        public MethodOfCorrection Method { get; set; }
        public ValidityOfCorrection Validity { get; set; }
        public double CorrectionFactorsProduct { get; set; }
        public double CorrectionFactor { get; set; }
        public double AdditionalCorrectionFactor { get; set; }
    }
}