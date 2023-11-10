using IbClient.messages;
using System.Collections.Generic;
using System.Linq;

namespace IbClient.Types
{
    // TODO evtl. obsolete
    public class MarketDataResponseList : IMarketDataResponseList
    {
        private List<MarketDataSnapshotResponse> _responseList;

        public MarketDataResponseList()
        {
            _responseList = new List<MarketDataSnapshotResponse>();
        }

        public void Add(int reqId)
        {
            _responseList.Add(new MarketDataSnapshotResponse(reqId));
        }

        public void UpdateResponse(TickPriceMessage tickPriceMessage)
        {
            MarketDataSnapshotResponse response = _responseList.Single(r => r.GetReqId() == tickPriceMessage.RequestId);
            response.AddTickPriceMessage(tickPriceMessage);

        }

        public MarketDataSnapshotResponse SetCompleted(int reqId)
        {
            MarketDataSnapshotResponse response = _responseList.Single(r => r.GetReqId() == reqId);
            response.SetEndOfSnapshot();
            return response;
        }
    }
}
