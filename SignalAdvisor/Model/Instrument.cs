﻿namespace SignalAdvisor.Model
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
                StopLossInCents = Int32.Parse(instrumentTextSplitted[2]),
                TakeProfitInCents = Int32.Parse(instrumentTextSplitted[3]),
                Quantity = Int32.Parse(instrumentTextSplitted[4])
            };
        }

        public int ConId { get; set; }    
        public string Symbol { get; set; }    
        public bool IsLong { get; set; }    
        public DateTime LastSignalTime { get; set; }    
        public string LastSignal { get; set; }    
        public string Algo { get; set; }    
        public double StopLossInCents { get; set; }    
        public double TakeProfitInCents { get; set; }    
        public int Quantity { get; set; }

       
    }
}