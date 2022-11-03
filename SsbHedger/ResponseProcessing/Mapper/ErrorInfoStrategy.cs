using System.Collections.Generic;

namespace SsbHedger.ResponseProcessing.Mapper
{
    public class ErrorInfoStrategy : IMapperStrategy
    {
        public void AddResponse(object message, List<ReqIdAndResponses> responses)
        {
            var errorInfo = message as ErrorInfo;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            responses.Add(new ReqIdAndResponses(errorInfo.ReqId, new List<object> { errorInfo }));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }
}