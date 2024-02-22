using OpenQA.Selenium;

namespace StockAnalyzer.DataProviders.Types
{
    public class WithError<T>
    {
        public WithError(T? value)
        {            
            Value = value;
            Error = null;  
        }

        public WithError(string error)
        {
            Error = error; 
            Value = default; 
        }

        public T? Value { get; private set; }
        public string? Error { get; private set; }
    }
}
