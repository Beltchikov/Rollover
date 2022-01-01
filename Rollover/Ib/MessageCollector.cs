using IBApi;
using IBSampleApp.messages;
using Rollover.Configuration;
using System;
using System.Collections.Generic;
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

        private int _reqIdContractDetails = 0;
        private int _reqIdSecDefOptParam = 0;

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
                if (message is string)
                {
                    connectionMessages.OnErrorMessages.Add(message as string);
                }
                else if (message is ConnectionStatusMessage)
                {
                    if (connectionMessages.ConnectionStatusMessage != null)
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

        public List<PositionMessage> reqPositions()
        {
            List<PositionMessage> positionMessages = new List<PositionMessage>();

            _ibClient.reqPositions();

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            while (stopWatch.Elapsed.TotalMilliseconds < _configurationManager.GetConfiguration().Timeout)
            {
                var message = _ibClientQueue.Dequeue();

                if (message is PositionMessage)
                {
                    positionMessages.Add(message as PositionMessage);
                }
                else if (message is string)
                {
                    var messageAsString = message as string;
                    if (messageAsString == Constants.ON_POSITION_END)
                    {
                        return positionMessages;
                    }
                }
            }

            return positionMessages;
        }

        public List<ContractDetailsMessage> reqContractDetails(Contract contract)
        {
            List<ContractDetailsMessage> contractDetailsMessages = new List<ContractDetailsMessage>();

            var reqId = ++_reqIdContractDetails;
            _ibClient.reqContractDetails(reqId, contract);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            while (stopWatch.Elapsed.TotalMilliseconds < _configurationManager.GetConfiguration().Timeout)
            {
                var message = _ibClientQueue.Dequeue();

                if (message is ContractDetailsMessage)
                {
                    contractDetailsMessages.Add(message as ContractDetailsMessage);
                }
                else if (message is string)
                {
                    var messageAsString = message as string;
                    if (messageAsString == Constants.ON_CONTRACT_DETAILS_END)
                    {
                        return contractDetailsMessages;
                    }
                }
            }

            return contractDetailsMessages;
        }

        public List<SecurityDefinitionOptionParameterMessage> reqSecDefOptParams(
            string symbol, 
            string exchange, 
            string secType, 
            int conId)
        {
            List<SecurityDefinitionOptionParameterMessage> securityDefinitionOptionParameterMessage 
                = new List<SecurityDefinitionOptionParameterMessage>();

            var reqId = ++_reqIdSecDefOptParam;
            _ibClient.reqSecDefOptParams(reqId, symbol, exchange, secType, conId);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            while (stopWatch.Elapsed.TotalMilliseconds < _configurationManager.GetConfiguration().Timeout)
            {
                var message = _ibClientQueue.Dequeue();

                if (message is SecurityDefinitionOptionParameterMessage)
                {
                    securityDefinitionOptionParameterMessage.Add(message as SecurityDefinitionOptionParameterMessage);
                }
                else if (message is string)
                {
                    var messageAsString = message as string;
                    if (messageAsString == Constants.ON_SECURITY_DEFINITION_OPTION_PARAMETER_END)
                    {
                        return securityDefinitionOptionParameterMessage;
                    }
                }
            }

            return securityDefinitionOptionParameterMessage;
        }

        public Tuple<TickSizeMessage, TickPriceMessage> reqMktData(
            Contract contract, 
            string generickTickList, 
            bool snapshot, 
            bool regulatorySnapshot, 
            List<TagValue> mktDataOptions)
        {
            throw new NotImplementedException();
        }
    }
}
