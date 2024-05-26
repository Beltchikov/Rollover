namespace SignalAdvisor.Model
{
    public class Instrument
    {
        public Instrument()
        {
            Symbol = null!;
            LastSignal = null!;
            Algo = null!;
        }

        public string Symbol { get; set; }    
        public bool IsLong { get; set; }    
        public DateTime LastSignalTime { get; set; }    
        public string LastSignal { get; set; }    
        public string Algo { get; set; }    
        public double StopLoss { get; set; }    
        public double TakeProfit { get; set; }    
        public int Quantity { get; set; }    
    }
}