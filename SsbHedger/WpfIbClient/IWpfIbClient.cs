using System;

namespace SsbHedger.WpfIbClient
{
    public interface IWpfIbClient 
    {
        event Action<int, bool> NextValidId;
        event Action<int, string> Error;

        void Execute();
        void InvokeError(int reqId, string message);
    }
}