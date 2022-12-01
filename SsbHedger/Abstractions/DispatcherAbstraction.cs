using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Threading;

namespace SsbHedger.Abstractions
{
    [ExcludeFromCodeCoverage]
    public class DispatcherAbstraction : IDispatcherAbstraction
    {
        private Dispatcher _dispatcher;

        public DispatcherAbstraction(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Invoke(Action action)
        {
            _dispatcher.Invoke(action);
        }
    }
}
