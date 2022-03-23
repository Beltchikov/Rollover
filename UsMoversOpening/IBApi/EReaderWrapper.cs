using IBApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsMoversOpening.IBApi
{
    public class EReaderWrapper : IEReaderWrapper
    {
        private EReader eReader;

        public EReaderWrapper(EClientSocket eClientSocket, EReaderMonitorSignalWrapper signalWrapper)
        {
            eReader = new EReader(eClientSocket, signalWrapper.EReaderMonitorSignal);
        }
    }
}
