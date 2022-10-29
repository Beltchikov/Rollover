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
            _ibClient.NextValidId += _ibClient_NextValidId;
            _ibClient.Error += _ibClient_Error;
            _ibClient.ManagedAccounts += _ibClient_ManagedAccounts;
            _ibClient.ContractDetails += _ibClient_ContractDetails;
            _ibClient.TickPrice += _ibClient_TickPrice;
            _ibClient.TickSize += _ibClient_TickSize;
            _ibClient.TickString += _ibClient_TickString;
            _ibClient.TickOptionCommunication += _ibClient_TickOptionCommunication;


            _ibClient.ConnectAndStartReaderThread(
                       "localhost",
                       4001,
                       1);
        }

        private void _ibClient_TickOptionCommunication(IbClient.messages.TickOptionMessage obj)
        {
            throw new NotImplementedException();
        }

        private void _ibClient_TickString(int arg1, int arg2, string arg3)
        {
            throw new NotImplementedException();
        }

        private void _ibClient_TickSize(IbClient.messages.TickSizeMessage obj)
        {
            throw new NotImplementedException();
        }

        private void _ibClient_TickPrice(IbClient.messages.TickPriceMessage obj)
        {
            throw new NotImplementedException();
        }

        private void _ibClient_ContractDetails(IbClient.messages.ContractDetailsMessage obj)
        {
            throw new NotImplementedException();
        }

        private void _ibClient_ManagedAccounts(IbClient.messages.ManagedAccountsMessage obj)
        {
            throw new NotImplementedException();
        }

        private void _ibClient_Error(int arg1, int arg2, string arg3, Exception arg4)
        {
            throw new NotImplementedException();
        }

        private void _ibClient_NextValidId(IbClient.messages.ConnectionStatusMessage obj)
        {
            throw new NotImplementedException();
        }
    }
}
