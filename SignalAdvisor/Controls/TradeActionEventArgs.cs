using SignalAdvisor.Model;

namespace SignalAdvisor.Controls
{
    public class TradeActionEventArgs : EventArgs
    {
        public Instrument Instrument { get; }

        public TradeActionEventArgs(Instrument instrument)
        {
            Instrument = instrument;
        }
    }
}