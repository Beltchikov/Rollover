using System.Collections.Generic;

namespace SsbHedger.ResponseProcessing
{
    public interface IResponseMapper
    {
        void AddResponse(object message);
        List<ReqIdAndResponses> GetGrouppedResponses();
        int Count { get; }
    }
}