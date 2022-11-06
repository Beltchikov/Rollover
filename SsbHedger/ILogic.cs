using System;

namespace SsbHedger
{
    public interface ILogic
    {
        void Execute();
        event Action<bool> NextValidId;
        event Action<string> Error;
    }
}