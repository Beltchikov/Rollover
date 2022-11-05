using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SsbHedger
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
