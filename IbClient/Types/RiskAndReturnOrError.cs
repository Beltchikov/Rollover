namespace IbClient.Types
{
    public class RiskAndReturnOrError
    {
        public RiskAndReturnOrError(double? risk, double? @return, string error)
        {
            Risk = risk;
            Return = @return;
            Error = error;
        }

        public double? Risk { get; set; }
        public double? Return { get; set; }
        public string Error { get; set; }
    }
}
