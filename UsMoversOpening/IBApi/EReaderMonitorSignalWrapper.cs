using IBApi;
using System.Diagnostics.CodeAnalysis;

namespace UsMoversOpening.IBApi
{
    [ExcludeFromCodeCoverage]
    public class EReaderMonitorSignalWrapper : IEReaderMonitorSignalWrapper
    {
        private EReaderMonitorSignal _signal;

        public EReaderMonitorSignalWrapper()
        {
            _signal = new EReaderMonitorSignal();
        }

        public EReaderMonitorSignal EReaderMonitorSignal => _signal;

        public void waitForSignal()
        {
            _signal.waitForSignal();
        }
    }
}
