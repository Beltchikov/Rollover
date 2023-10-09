namespace StockAnalyzer.Ib
{
    public interface IIbHostQueue
    {
        void Enqueue(object obj);
        object Dequeue();
        object Peek();
        int Count();
    }
}