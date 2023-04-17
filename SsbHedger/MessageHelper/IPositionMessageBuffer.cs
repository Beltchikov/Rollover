using IbClient.messages;

namespace SsbHedger.MessageHelper
{
    public interface IPositionMessageBuffer
    {
        void AddMessage(PositionMessage positionMessage);
        void Reset();
        int? FirstCallSize();
        int? SecondCallSize();
        double? FirstCallStrike();
        double? SecondCallStrike();
    }
}