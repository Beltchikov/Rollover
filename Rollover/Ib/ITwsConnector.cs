namespace Rollover.Ib
{
    public interface ITwsConnector
    {
        void Connect(string host, int port, int clientId);
    }
}