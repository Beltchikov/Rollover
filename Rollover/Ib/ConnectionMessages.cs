using IBSampleApp.messages;
using System.Collections.Generic;

namespace Rollover.Ib
{
    public class ConnectionMessages
    {
        public List<string> OnErrorMessages { get; set; }
        public ConnectionStatusMessage ConnectionStatusMessage { get; set; }
        public ManagedAccountsMessage ManagedAccountsMessage { get; set; }
    }
}