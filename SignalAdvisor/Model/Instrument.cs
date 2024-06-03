using IBApi;
using System.ComponentModel;

namespace SignalAdvisor.Model
{
    public class Instrument : INotifyPropertyChanged
    {
        private readonly double COMMISSION_PER_STOCK = 0.005;
        private readonly double MIN_COMMISSION= 1;
        private double askPrice;
        private double bidPrice;

        public Instrument()
        {
            Symbol = null!;
            Currency = null!;
            Exchange = null!;
            LastSignal = null!;
            Algo = null!;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public static Instrument FromTabbedLine(string instrumentText)
        {
            var instrumentTextSplitted = instrumentText.Trim().Split([' ','\t']).Select(s => s.Trim()).ToList();
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
        public double AskPrice
        {
            get { return askPrice; }
            set {
                if (askPrice != value)
                {
                    askPrice = value;
                    NotifyPropertyChanged("AskPrice");
                }
            }
        }
        public double BidPrice
        {
            get { return bidPrice; }
            set
            {
                if (bidPrice != value)
                {
                    bidPrice = value;
                    NotifyPropertyChanged("BidPrice");
                }
            }
        }

        public void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public double CalculateStopLossPrice(double askPrice)
        {
            var stopLoss = StopLossInCents / 100;
            var stopLossPrice = Math.Round(askPrice - stopLoss, 2);
            return stopLossPrice;
        }

        public double CalculateTakeProfitPrice(double askPrice)
        {
            var takeProfit = TakeProfitInCents / 100;
            var takeProfitPrice = Math.Round(askPrice + takeProfit, 2);
            return takeProfitPrice;
        }

        public double CalculateCommision()
        {
            var commissionInCents = Math.Ceiling((double)Quantity * COMMISSION_PER_STOCK * 100);
            var commission = Math.Round(commissionInCents / 100, 2);

            if(commission < MIN_COMMISSION) commission = MIN_COMMISSION;

            return commission;
        }

        public Contract ToContract()
        {
            Contract contract = new Contract()
            {
                ConId = ConId,
                Symbol = Symbol,
                SecType = App.SEC_TYPE_STK,
                Currency = Currency,
                Exchange = Exchange
            };

            return contract;
        }
    }
}