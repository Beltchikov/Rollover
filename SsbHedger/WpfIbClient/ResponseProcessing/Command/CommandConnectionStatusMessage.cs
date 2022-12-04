using IbClient.messages;
using SsbHedger.WpfIbClient;

namespace SsbHedger.ResponseProcessing.Command
{
    public class CommandConnectionStatusMessage : ResponseCommand
    {
        private CommandConnectionStatusMessage() { }

        public CommandConnectionStatusMessage(IWpfIbClient client)
        {
            _client = client;
        }
        
        public override void SetParameters(object message)
        {
            if (message is ConnectionStatusMessage connectionStatusMessage)
            {
                _parameters.Add(connectionStatusMessage);
            }
        }

        public override void Execute()
        {
            _client.InvokeNextValidId((ConnectionStatusMessage)_parameters[0]);
        }
    }
}