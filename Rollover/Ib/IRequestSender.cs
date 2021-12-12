namespace Rollover.Ib
{
    public interface IRequestSender
    {
        void RegisterResponseHandlers();
        void Connect(string host, int port, int clientId);
    }
}