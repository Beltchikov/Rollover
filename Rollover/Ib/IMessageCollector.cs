namespace Rollover.Ib
{
    public interface IMessageCollector
    {
        ConnectionMessages eConnect(string host, int port, int clientId);
    }
}