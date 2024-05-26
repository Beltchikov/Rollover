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

        public static Instrument FromTabbedLine(string instrumentText)
        {
            var instrumentTextSplitted = instrumentText.Trim().Split(['\t']).Select(s => s.Trim()).ToList();
            return new Instrument()
            {
                ConId = Int32.Parse(instrumentTextSplitted[0]),
                Symbol = instrumentTextSplitted[1],
                StopLoss = Int32.Parse(instrumentTextSplitted[2]),
                TakeProfit = Int32.Parse(instrumentTextSplitted[3]),
                Quantity = Int32.Parse(instrumentTextSplitted[4])
            };
        }

        public int ConId { get; set; }    
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