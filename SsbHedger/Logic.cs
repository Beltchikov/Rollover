using IbClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SsbHedger
{
    public class Logic : ILogic
    {
        IIBClient _ibClient;

        public Logic(IIBClient ibClient)
        {
            _ibClient = ibClient;
        }

        public void Execute()
        {
           _ibClient.ConnectAndStartReaderThread(
                       "localhost",
                       4001,
                       1);
        }
    }
}
