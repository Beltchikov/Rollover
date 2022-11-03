using IbClient.messages;
using System.Collections.Generic;

namespace SsbHedger.ResponseProcessing.Mapper
{
    public class ManagedAccountsStrategy : IMapperStrategy
    {
        public void AddResponse(object message, List<ReqIdAndResponses> responses)
        {
            var managedAccountsMessage = message as ManagedAccountsMessage;
#pragma warning disable CS8604 // Possible null reference argument.
            responses.Add(new ReqIdAndResponses(-1, new List<object> { managedAccountsMessage }));
#pragma warning restore CS8604 // Possible null reference argument.
        }
    }
}