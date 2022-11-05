using System;

namespace SsbHedger
{
    public interface IDispatcherAbstraction
    {
        void Invoke(Action action);
    }
}