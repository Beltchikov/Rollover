using IbClient.messages;
using System.Collections.Generic;

namespace SsbHedger.ResponseProcessing.Mapper
{
    internal class ConnectionStatusStrategy : IMapperStrategy
    {
        public void AddResponse(object message, List<ReqIdAndResponses> responses)
        {
            var connectionStatusMessage = message as ConnectionStatusMessage;
#pragma warning disable CS8604 // Possible null reference argument.
            responses.Add(new ReqIdAndResponses(-1, new List<object> { connectionStatusMessage }));
#pragma warning restore CS8604 // Possible null reference argument.
        }
    }
}