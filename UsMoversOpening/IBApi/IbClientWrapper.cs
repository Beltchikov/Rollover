using IBSampleApp;

namespace UsMoversOpening.IBApi
{
    public class IbClientWrapper : IIbClientWrapper
    {
        private IBClient ibClient;

        public IbClientWrapper(IEReaderMonitorSignalWrapper eReaderMonitorSignalWrapper)
{
            ibClient = new IBClient(eReaderMonitorSignalWrapper.EReaderMonitorSignal);
        }

        public void eConnect(string host, int port, int clientId)
        {
            ibClient.ClientSocket.eConnect(host, port, clientId);
        }
    }
}
