namespace SsbHedger.ResponseProcessing
{
    public interface IResponseProcessor
    {
        void Process(ReqIdAndResponses reqIdAndResponses);
        void SetLogic(ILogic sut);
    }
}