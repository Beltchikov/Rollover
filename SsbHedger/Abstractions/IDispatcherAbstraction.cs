using System;

namespace SsbHedger.Abstractions
{
    public interface IDispatcherAbstraction
    {
        void Invoke(Action action);
     }
}