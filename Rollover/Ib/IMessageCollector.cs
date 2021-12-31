namespace Rollover.Ib
{
    public interface IMessageCollector
    {
        ConnectionMessages eConnect(string v1, int v2, int v3);
    }
}