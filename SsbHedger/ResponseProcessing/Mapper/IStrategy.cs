using System.Collections.Generic;

namespace SsbHedger.ResponseProcessing.Mapper
{
    public interface IStrategy
    {
        void AddResponse(object message, List<ReqIdAndResponses> responses);
    }
}