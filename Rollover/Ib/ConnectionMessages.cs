using IBSampleApp.messages;
using System.Collections.Generic;

namespace Rollover.Ib
{
    public class ConnectionMessages
    {
        public ConnectionMessages()
        {
            OnErrorMessages = new List<string>();
        }
        
        public List<string> OnErrorMessages { get; }
        public ConnectionStatusMessage ConnectionStatusMessage { get; set; }
        public ManagedAccountsMessage ManagedAccountsMessage { get; set; }
        public bool Connected { get; set; }
    }
}