using IbClient.messages;
using System;

namespace IbClient
{
    public interface IResponses
    {
        bool TryGetValidPrice(int reqId, Predicate<TickPriceMessage> ValidPriceCallback, out double? price);
        void AddTickPriceMessage(TickPriceMessage tickPriceMessage);
    }
}