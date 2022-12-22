using SsbHedger2.WpfIbClient;
using System.Collections.Generic;

namespace SsbHedger2.ResponseProcessing.Command
{
    public abstract class ResponseCommand
    {
        protected List<object> _parameters = new();

        protected IWpfIbClient _client = null!;

        public abstract void SetParameters(object message);

        public abstract void Execute();
    }
}