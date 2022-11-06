namespace SsbHedger.ResponseProcessing
{
    public interface IResponseProcessor
    {
        void Process(object message);
        void SetLogic(ILogic sut);
    }
}