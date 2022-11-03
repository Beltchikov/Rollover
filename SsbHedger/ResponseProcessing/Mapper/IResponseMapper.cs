using System.Collections.Generic;

namespace SsbHedger.ResponseProcessing.Mapper
{
    public interface IResponseMapper
    {
        void AddResponse(object message);
        List<ReqIdAndResponses> GetGrouppedResponses();
        int Count { get; }
    }
}