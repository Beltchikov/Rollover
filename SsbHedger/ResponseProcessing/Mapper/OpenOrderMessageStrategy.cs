using IbClient.messages;
using System.Collections.Generic;

namespace SsbHedger.ResponseProcessing.Mapper
{
    public class OpenOrderStrategy : IMapperStrategy
    {
        public void AddResponse(object message, List<ReqIdAndResponses> responses)
        {
            var openOrderMessage = message as OpenOrderMessage;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            responses.Add(new ReqIdAndResponses(openOrderMessage.OrderId, new List<object> { openOrderMessage }));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }
}