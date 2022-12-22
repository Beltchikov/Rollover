using SsbHedger2.Model;
using SsbHedger2.WpfIbClient;
using System.Collections.Generic;

namespace SsbHedger2.ResponseProcessing.Command
{
    public class CommandErrorInfo : ResponseCommand
    {
        private CommandErrorInfo(){}

        public CommandErrorInfo(IWpfIbClient client)        
        {
            base._client = client;
        }

        public override void SetParameters(object message)
        {
            if (message is ErrorInfo errorInfo)
            {
                _parameters.Add(errorInfo.ReqId);
                _parameters.Add(errorInfo.Code);
                _parameters.Add(errorInfo.Message);
                _parameters.Add(errorInfo.Exception == null ? "" : errorInfo.Exception);
            }
        }

        public override void Execute()
        {
            _client.InvokeError((int)_parameters[0], $"Code:{_parameters[1]} Message:{_parameters[2]} Exception:{_parameters[3]}");
        }
    }
}
