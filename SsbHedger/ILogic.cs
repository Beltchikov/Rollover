using System;

namespace SsbHedger
{
    public interface ILogic
    {
        event Action<int, bool> NextValidId;
        event Action<int, string> Error;

        void Execute();
        void InvokeError(int reqId, string message);
    }
}