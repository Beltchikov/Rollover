﻿namespace SignalAdvisor.Model
{
    public class Instrument
    {
        public Instrument()
        {
            Symbol = null!;
            Currency = null!;
            Exchange = null!;
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
                Currency = instrumentTextSplitted[2],
                Exchange = instrumentTextSplitted[3],
                StopLossInCents = Int32.Parse(instrumentTextSplitted[4]),
                TakeProfitInCents = Int32.Parse(instrumentTextSplitted[5]),
                Quantity = Int32.Parse(instrumentTextSplitted[6])
            };
        }

        public int ConId { get; set; }    
        public string Symbol { get; set; }    
        public string Currency { get; set; }    
        public string Exchange { get; set; }    
        public bool IsLong { get; set; }    
        public DateTime LastSignalTime { get; set; }    
        public string LastSignal { get; set; }    
        public string Algo { get; set; }    
        public double StopLossInCents { get; set; }    
        public double TakeProfitInCents { get; set; }    
        public int Quantity { get; set; }
        public int RequestIdMktData { get; set; }
    }
}