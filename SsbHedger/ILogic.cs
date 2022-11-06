using System;

namespace SsbHedger
{
    public interface ILogic
    {
        event Action<bool> NextValidId;
        event Action<string> Error;

        void Execute();
        void InvokeError(string message);
    }
}