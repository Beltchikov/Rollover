namespace Rollover.Ib
{
    public interface IConnectedCondition
    {
        void AddInput(string input);
        bool IsConnected();
    }
}