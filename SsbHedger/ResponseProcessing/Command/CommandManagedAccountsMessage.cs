﻿using IbClient.messages;
using SsbHedger.WpfIbClient;

namespace SsbHedger.ResponseProcessing.Command
{
    public class CommandManagedAccountsMessage : ResponseCommand
    {
        private CommandManagedAccountsMessage() { }

        public CommandManagedAccountsMessage(IWpfIbClient client)
        {
            _client = client;
        }

        public override void SetParameters(object message)
        {
            if (message is ManagedAccountsMessage managedAccountsMessage)
            {
                _parameters.Add(managedAccountsMessage);
            }
        }

        public override void Execute()
        {
            _client.InvokeManagedAccounts((ManagedAccountsMessage)_parameters[0]);
        }
    }
}