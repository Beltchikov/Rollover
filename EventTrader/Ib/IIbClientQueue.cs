namespace Eomn.Ib
{
    public interface IIbClientQueue
    {
        void Enqueue(object obj);
        object? Dequeue();
        object? Peek();
        int Count();
    }
}