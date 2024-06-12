using IbClient.messages;
using IBSampleApp.messages;
using System.Collections.Generic;

namespace SsbHedger.Utilities
{
    public interface IPositionMessageBuffer
    {
        List<PositionMessage> Messages { get; }

        void AddMessage(PositionMessage positionMessage);
        void Reset();
        int? FirstCallSize();
        int? SecondCallSize();
        double? FirstCallStrike();
        double? SecondCallStrike();
    }
}