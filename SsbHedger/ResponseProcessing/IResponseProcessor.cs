namespace SsbHedger.ResponseProcessing
{
    public interface IResponseProcessor
    {
        void Process(object message);
    }
}