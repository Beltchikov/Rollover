using IbClient.messages;

namespace SsbHedger.MessageHelper
{
    public interface IPositionMessageBuffer
    {
        void AddMessage(PositionMessage positionMessage);
        int? FirstCallSize();
        void Reset();
        int? SecondCallSize();
    }
}