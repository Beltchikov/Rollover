using IBApi;

namespace UsMoversOpening.IBApi
{
    public interface IIbClientWrapper
    {
        public EReaderMonitorSignalWrapper Signal { get; }
        public EClientSocket ClientSocket { get; }
        void eConnect(string host, int port, int clientId);
    }
}