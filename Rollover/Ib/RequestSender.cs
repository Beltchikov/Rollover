using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rollover.Ib
{
    public class RequestSender : IRequestSender
    {
        private IIbClientWrapper _ibClient;

        public RequestSender(IIbClientWrapper ibClient)
        {
            _ibClient = ibClient;
        }

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public void RegisterResponseHandlers()
        {
            _ibClient.Error += ResponseHandlers.OnError;
            _ibClient.NextValidId += ResponseHandlers.NextValidId;
            _ibClient.ManagedAccounts += ResponseHandlers.ManagedAccounts;
        }

        
    }
}
