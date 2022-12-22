namespace SsbHedger2.ResponseProcessing
{
    public interface IReaderThreadQueue
    {
        public void Enqueue(object item);

        public object? Dequeue();

        public int Count();
    }
}