using IbClient.messages;
using System.Collections.Generic;
using System.Windows.Documents;

namespace SsbHedger.MessageHelper
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