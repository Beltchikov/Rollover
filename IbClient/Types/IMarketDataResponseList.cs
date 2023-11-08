using IbClient.messages;

namespace IbClient.Types
{
    public interface IMarketDataResponseList
    {
        void Add(int reqId);
        void UpdateResponse(TickPriceMessage tickPriceMessage);
        MarketDataSnapshotResponse SetCompleted(int reqId);
    }
}