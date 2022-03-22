using IBApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsMoversOpening.IBApi
{
    public class IbClientWrapper : IIbClientWrapper
    {
        //private IBClient ibClient;

        public IbClientWrapper(IEReaderMonitorSignalWrapper eReaderMonitorSignalWrapper)
        {
            //ibClient = new EClient(eReaderMonitorSignalWrapper.EReaderMonitorSignal);
        }

        public void eConnect(string host, int port, int clientId)
        {
            throw new NotImplementedException();
        }
    }
}
