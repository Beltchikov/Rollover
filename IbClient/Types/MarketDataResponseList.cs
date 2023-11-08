using IbClient.messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IbClient.Types
{
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
            MarketDataSnapshotResponse response = _responseList.Single(r => r.GetId() == tickPriceMessage.RequestId);
            response.AddTickPriceMessage(tickPriceMessage);

        }

        public MarketDataSnapshotResponse SetCompleted(int reqId)
        {
            MarketDataSnapshotResponse response = _responseList.Single(r => r.GetId() == reqId);
            response.SetEndOfSnapshot();
            return response;
        }

        public bool ResponseIsCompleted(int reqId)
        {
            MarketDataSnapshotResponse response = _responseList.Single(r => r.GetId() == reqId);
            return response.EndOfSnapshot();
        }

        public double? GetPrice(int reqId, TickType tickType)
        {
            MarketDataSnapshotResponse response = _responseList.Single(r => r.GetId() == reqId);
            return response.GetPrice(tickType);
            //return _responseList.Single(r => r.GetResponse()) == reqId);
        }
    }
}
