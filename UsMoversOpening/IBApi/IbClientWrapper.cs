using IBApi;
using IBSampleApp;
using System;
using System.Diagnostics.CodeAnalysis;

namespace UsMoversOpening.IBApi
{
    [ExcludeFromCodeCoverage]
    public class IbClientWrapper : IIbClientWrapper
    {
        private IBClient _ibClient;
        private EReaderMonitorSignalWrapper _signal;

        public IbClientWrapper(EReaderMonitorSignalWrapper eReaderMonitorSignalWrapper)
        {
            _signal = eReaderMonitorSignalWrapper;
            _ibClient = new IBClient(eReaderMonitorSignalWrapper.EReaderMonitorSignal);
        }

        public void eConnect(string host, int port, int clientId)
        {
            _ibClient.ClientSocket.eConnect(host, port, clientId);
        }

        public EReaderMonitorSignalWrapper Signal => _signal;
        public EClientSocket ClientSocket => _ibClient.ClientSocket;

        public Action<int, int, string, Exception> OnErrorFunction
        { 
            set
            {
                _ibClient.Error += value;
            }
        }
    }
}
