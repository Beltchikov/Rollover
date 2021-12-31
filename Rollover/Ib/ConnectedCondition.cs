using IBSampleApp.messages;
using System.Collections.Concurrent;
using System.Linq;

namespace Rollover.Ib
{
    public class ConnectedCondition : IConnectedCondition
    {
        ConcurrentDictionary<int, object> _messageList = new ConcurrentDictionary<int, object>();   

        public void AddMessage(object message)
        {
            _messageList[_messageList.Count] = message;
        }

        public bool IsConnected()
        {
            bool condition = _messageList.Values.Any(i => i is ManagedAccountsMessage
                && (i as ManagedAccountsMessage).ManagedAccounts.Any());
            condition = condition && !_messageList.Values.Any(i => i is ConnectionStatusMessage
             && (i as ConnectionStatusMessage).IsConnected);

            condition = condition && _messageList.Values.Any(i => i is string && (i as string).Contains("id=-1"));
            condition = condition && _messageList.Values.Any(i => i is string && (i as string).Contains("errorCode=2104"));
            condition = condition && _messageList.Values.Any(i => i is string && (i as string).Contains("Market data"));

            return condition;
        }
    }
}
