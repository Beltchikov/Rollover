using System;

namespace StockAnalyzer.DataProviders.Types
{
    public class DateTimeWithError
    {
        public DateTimeWithError(DateTime? value, string error)
        {
            Value = value;
            Error = error;
        }

        public DateTime? Value { get; set; }
        public string? Error { get; set; }
    }
}
