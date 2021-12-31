namespace Rollover.Ib
{
    public interface IConnectedCondition
    {
        bool IsConnected();
        void AddMessage(object message);
    }
}