using SsbHedger.WpfIbClient;
using System.Collections.Generic;

namespace SsbHedger.ResponseProcessing.Command
{
    public abstract class ResponseCommand
    {
        protected List<object> _parameters = new();

        protected IWpfIbClient _client = null!;

        public abstract void SetParameters(object message);

        public abstract void Execute();
    }
}