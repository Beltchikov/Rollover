using IbClient.messages;
using IbClient.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IbClient
{
    public class Responses : IResponses
    {
        private List<TickPriceMessage> _tickPriceMessages { get; set; }
        private List<TickSizeMessage> _tickSizeMessages { get; set; }

        private List<int> _reqIdsOfEndedSnapshots;

        public Responses()
        {
            _tickPriceMessages = new List<TickPriceMessage>();
            _tickSizeMessages = new List<TickSizeMessage>();
            _reqIdsOfEndedSnapshots = new List<int>();
        }
        public bool TryGetValidPrice(
            int reqId,
            Predicate<TickPriceMessage> ValidPriceCallback,
            out double? price,
            out TickType? tickType)
        {
            var tickPriceMessagesCopy = _tickPriceMessages.ToArray();

            foreach (var tickPriceMessage in tickPriceMessagesCopy.Where(m => m?.RequestId == reqId))
            {
                if (ValidPriceCallback(tickPriceMessage))
                {
                    price = tickPriceMessage.Price;
                    tickType = (TickType)Enum.Parse(typeof(TickType), tickPriceMessage.Field.ToString());
                    return true;
                }
            }

            price = null;
            tickType = null;
            return false;
        }

        public void AddTickPriceMessage(TickPriceMessage tickPriceMessage)
        {
            object lockObject = new object();
            lock (lockObject)
            {
                _tickPriceMessages.Add(tickPriceMessage);
            }
        }

        public void SetSnapshotEnd(int reqId)
        {
            object lockObject = new object();
            lock (lockObject)
            {
                _reqIdsOfEndedSnapshots.Add(reqId);
            }
        }

        public bool SnaphotEnded(int reqId)
        {
            var reqIdsOfEndedSnapshotsCopy = _reqIdsOfEndedSnapshots.ToArray();
            return reqIdsOfEndedSnapshotsCopy.Any(i => i.Equals(reqId));
        }
    }
}
