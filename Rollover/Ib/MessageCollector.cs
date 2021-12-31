using IBSampleApp.messages;
using Rollover.Configuration;
using System;
using System.Diagnostics;
using System.Threading;

namespace Rollover.Ib
{
    public class MessageCollector : IMessageCollector
    {
        private IIbClientWrapper _ibClient;
        private IConnectedCondition _connectedCondition;
        private IIbClientQueue _ibClientQueue;
        private IConfigurationManager _configurationManager;

        public MessageCollector(
            IIbClientWrapper ibClient, 
            IConnectedCondition connectedCondition, 
            IIbClientQueue ibClientQueue, 
            IConfigurationManager configurationManager)
        {
            _ibClient = ibClient;
            _connectedCondition = connectedCondition;
            _ibClientQueue = ibClientQueue;
            _configurationManager = configurationManager;
        }

        public ConnectionMessages eConnect(string host, int port, int clientId)
        {
            ConnectAndStartConsoleThread(host, port, clientId);
            return CheckConnectionMessages(_ibClientQueue, _configurationManager.GetConfiguration().Timeout);
        }
                
        private void ConnectAndStartConsoleThread(string host, int port, int clientId)
        {
            _ibClient.eConnect(host, port, clientId);

            var reader = _ibClient.ReaderFactory();
            reader.Start();

            new Thread(() =>
            {
                while (_ibClient.IsConnected())
                {
                    _ibClient.WaitForSignal();
                    reader.processMsgs();
                }
            })
            { IsBackground = true }
            .Start();
        }

        private ConnectionMessages CheckConnectionMessages(IIbClientQueue ibClientQueue, int timeout)
        {
            var connectionMessages = new ConnectionMessages();
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            while (stopWatch.Elapsed.TotalMilliseconds < timeout)
            {
                var message = ibClientQueue.Dequeue();
                if(message is string)
                {
                    connectionMessages.OnErrorMessages.Add(message as string);
                }
                else if (message is ConnectionStatusMessage)
                {
                    if(connectionMessages.ConnectionStatusMessage != null)
                    {
                        throw new Exception("Unexpected. Multiple ConnectionStatusMessage");
                    }
                    connectionMessages.ConnectionStatusMessage = message as ConnectionStatusMessage;
                }
                else if (message is ManagedAccountsMessage)
                {
                    if (connectionMessages.ManagedAccountsMessage != null)
                    {
                        throw new Exception("Unexpected. Multiple ManagedAccountsMessage");
                    }
                    connectionMessages.ManagedAccountsMessage = message as ManagedAccountsMessage;
                }

                _connectedCondition.AddMessage(message);
                if (_connectedCondition.IsConnected())
                {
                    connectionMessages.Connected = true;
                    return connectionMessages;
                }
            }

            return connectionMessages;
        }
    }
}
