using IbClient.messages;
using IbClient.Types;
using System;

namespace IbClient
{
    public interface IResponses
    {
        bool TryGetValidPrice(
            int reqId,
            Predicate<TickPriceMessage> ValidPriceCallback,
            out double? price,
            out TickType? tickType);
        void AddTickPriceMessage(TickPriceMessage tickPriceMessage);
        void SetSnapshotEnd(int reqId);
    }
}