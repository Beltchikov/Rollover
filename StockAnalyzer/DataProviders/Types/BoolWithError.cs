namespace StockAnalyzer.DataProviders.Types
{
    public class BoolWithError
    {
        public BoolWithError(bool? value, string? error)
        {
            Value = value;
            Error = error;  
        }

        public bool? Value { get; set; }
        public string? Error { get; set; }
    }
}
