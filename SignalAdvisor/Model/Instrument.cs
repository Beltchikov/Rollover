namespace SignalAdvisor.Model
{
    public class Instrument
    {
        public Instrument()
        {
            Symbol = null!;
            Signal = null!;
            Algo = null!;
        }

        public string Symbol { get; set; }    
        public bool IsLong { get; set; }    
        public DateTime SignalTime { get; set; }    
        public string Signal { get; set; }    
        public string Algo { get; set; }    
        public int SlInCents { get; set; }    
        public int TpInCents { get; set; }    
        public int Qty { get; set; }    
    }
}