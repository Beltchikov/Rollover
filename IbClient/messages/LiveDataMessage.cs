using IBApi;
using IBSampleApp.messages;

namespace IbClient.messages
{
    public class LiveDataMessage
    {
        public LiveDataMessage(Contract contract, HistoricalDataMessage message)
        {
            Contract = contract;
            HistoricalDataMessage = message;
        }

        public Contract Contract { get; set; }
        public HistoricalDataMessage HistoricalDataMessage { get; set; }
    }
}