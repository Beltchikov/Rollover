using IBSampleApp.messages;
using System.Collections.Concurrent;
using System.Linq;

namespace Rollover.Ib
{
    public class ConnectedCondition : IConnectedCondition
    {
        ConcurrentDictionary<int, object> _messageList = new ConcurrentDictionary<int, object>();

        bool _managedAccountsAny;
        bool _isConnected;
        bool _idMinus1;
        bool _errorCode21041;
        bool _marketData;

        public void AddMessage(object message)
        {
            if((message is ManagedAccountsMessage) && (message as ManagedAccountsMessage).ManagedAccounts.Any())
            {
                _managedAccountsAny = true;
            }
            else if ((message is ConnectionStatusMessage) && (message as ConnectionStatusMessage).IsConnected)
            {
                _isConnected = true;
            }
            else if ((message is string) && (message as string).Contains("id=-1"))
            {
                _idMinus1 = true;
            }
            else if ((message is string) && (message as string).Contains("errorCode=2104"))
            {
                _errorCode21041 = true;
            }
            else if ((message is string) && (message as string).Contains("Market data"))
            {
                _marketData = true;
            }


            _messageList[_messageList.Count] = message;
        }

        public bool IsConnected()
        {
            //return _managedAccountsAny
            //    && _isConnected
            //    && _idMinus1
            //    && _errorCode21041
            //    && _marketData;

            return _managedAccountsAny
                && _isConnected;
        }
    }
}
