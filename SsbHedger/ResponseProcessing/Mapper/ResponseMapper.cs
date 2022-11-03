using IbClient.messages;
using System;
using System.Collections.Generic;

namespace SsbHedger.ResponseProcessing.Mapper
{
    public class ResponseMapper : IResponseMapper
    {
        internal List<ReqIdAndResponses> _responses = new List<ReqIdAndResponses>();

        private Dictionary<Type, IStrategy> _strategies = new Dictionary<Type, IStrategy>()
        {
            {typeof(ErrorInfo), new ErrorInfoStrategy()},
            {typeof(OpenOrderMessage), new OpenOrderStrategy()},
            {typeof(ConnectionStatusMessage), new ConnectionStatusStrategy()},
            {typeof(ManagedAccountsMessage), new ManagedAccountsStrategy()}
        };

        public void AddResponse(object message)
        {
            try
            {
                IStrategy strategy = _strategies[message.GetType()];
                strategy.AddResponse(message, _responses);
            }
            catch (KeyNotFoundException keyNotFoundException)
            {
                throw new ArgumentException(
                    $"Unknown message type {message.GetType()}",
                    keyNotFoundException);
            }
        }

        public List<ReqIdAndResponses> GetGrouppedResponses()
        {
            var responsesCopy = new List<ReqIdAndResponses>(_responses);
            _responses.Clear();
            return responsesCopy;
        }

        public int Count => _responses.Count;
    }
}
