namespace Rollover.Ib
{
    public interface IIbClientQueue
    {
        void Enqueue(object obj);
        object Dequeue();
        int Count();
    }
}