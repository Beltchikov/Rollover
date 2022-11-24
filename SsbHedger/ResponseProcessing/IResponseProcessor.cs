using SsbHedger.WpfIbClient;

namespace SsbHedger.ResponseProcessing
{
    public interface IResponseProcessor
    {
        void Process(ReqIdAndResponses reqIdAndResponses);
        void SetLogic(IWpfIbClient sut);
    }
}