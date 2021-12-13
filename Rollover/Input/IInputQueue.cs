namespace Rollover.Input
{
    public interface IInputQueue
    {
        void Enqueue(string v);
        string Dequeue();
        int Count();
    }
}