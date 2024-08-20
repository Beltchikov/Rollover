namespace StockAnalyzer.DataProviders.Types
{
    public class WithError<T>
    {
        public WithError(T? data)
        {            
            Data = data;
            Error = null;  
        }

        public WithError(string error)
        {
            Error = error; 
            Data = default; 
        }

        public T? Data { get; set; }
        public string? Error { get; set; }

        public bool HasData => Data != null;
        public bool HasError=> ! string.IsNullOrWhiteSpace(Error);
    }
}
