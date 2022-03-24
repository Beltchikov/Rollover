using IBApi;
using System;

namespace UsMoversOpening.IBApi
{
    public interface IIbClientWrapper
    {
        public EReaderMonitorSignalWrapper Signal { get; }
        public EClientSocket ClientSocket { get; }
        void eConnect(string host, int port, int clientId);
        public Action<int, int, string, Exception> OnErrorFunction { set; }
    }
}