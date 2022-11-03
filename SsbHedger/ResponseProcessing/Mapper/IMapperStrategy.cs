using System.Collections.Generic;

namespace SsbHedger.ResponseProcessing.Mapper
{
    public interface IMapperStrategy
    {
        void AddResponse(object message, List<ReqIdAndResponses> responses);
    }
}