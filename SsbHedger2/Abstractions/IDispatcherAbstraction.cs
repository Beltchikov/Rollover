using System;

namespace SsbHedger2.Abstractions
{
    public interface IDispatcherAbstraction
    {
        void Invoke(Action action);
     }
}