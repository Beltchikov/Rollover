namespace IbClient.IbHost
{
    public interface IIbHostQueue
    {
        void Enqueue(object obj);
        object Dequeue();
        object Peek();
        int Count();
    }
}