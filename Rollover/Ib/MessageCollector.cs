using IBApi;
using IBSampleApp.messages;
using Rollover.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Rollover.Ib
{
    public class MessageCollector : IMessageCollector
    {
        private readonly IIbClientWrapper _ibClient;
        private readonly IConnectedCondition _connectedCondition;
        private readonly IIbClientQueue _ibClientQueue;
        private readonly IConfigurationManager _configurationManager;

        private int _reqIdContractDetails = 0;
        private int _reqIdSecDefOptParam = 0;
        private int _reqIdMktData = 0;
        
        private int _timeout= 0;

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

            _timeout = _configurationManager.GetConfiguration().Timeout;
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
                if (message is string messageAsString)
                {
                    connectionMessages.OnErrorMessages.Add(messageAsString);
                }
                else if (message is ConnectionStatusMessage statusMessage)
                {
                    if (connectionMessages.ConnectionStatusMessage != null)
                    {
                        throw new Exception("Unexpected. Multiple ConnectionStatusMessage");
                    }
                    connectionMessages.ConnectionStatusMessage = statusMessage;
                }
                else if (message is ManagedAccountsMessage accountsMessage)
                {
                    if (connectionMessages.ManagedAccountsMessage != null)
                    {
                        throw new Exception("Unexpected. Multiple ManagedAccountsMessage");
                    }
                    connectionMessages.ManagedAccountsMessage = accountsMessage;
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
            return GetIbData<PositionMessage>(_ibClientQueue, Constants.ON_POSITION_END, _timeout);
        }

        public List<TMessage> GetIbData<TMessage>(IIbClientQueue queue, string endToken, int timeout)
        {
            List<TMessage> positionMessages = new List<TMessage>();

            _ibClient.reqPositions();
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            int msgCountBefore = 0;
            bool endMessageReceived = false;
            while (stopWatch.Elapsed.TotalMilliseconds < timeout)
            {
                var message = queue.Dequeue();

                if (message is TMessage positionMessage)
                {
                    positionMessages.Add(positionMessage);
                }
                if (message is string messageAsString && messageAsString == endToken)
                {
                    endMessageReceived = true;
                }

                if (endMessageReceived && msgCountBefore > 0 && positionMessages.Count() == msgCountBefore)
                {
                    return positionMessages;
                }
                msgCountBefore = positionMessages.Count();
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

                if (message is ContractDetailsMessage detailsMessage)
                {
                    contractDetailsMessages.Add(detailsMessage);
                }
                else if (message is string messageAsString)
                {
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
            var securityDefinitionOptionParameterMessage
                = new List<SecurityDefinitionOptionParameterMessage>();

            var reqId = ++_reqIdSecDefOptParam;
            _ibClient.reqSecDefOptParams(reqId, symbol, exchange, secType, conId);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            while (stopWatch.Elapsed.TotalMilliseconds < _configurationManager.GetConfiguration().Timeout)
            {
                var message = _ibClientQueue.Dequeue();

                if (message is SecurityDefinitionOptionParameterMessage parameterMessage)
                {
                    securityDefinitionOptionParameterMessage.Add(parameterMessage);
                }
                else if (message is string messageAsString)
                {
                    if (messageAsString == Constants.ON_SECURITY_DEFINITION_OPTION_PARAMETER_END)
                    {
                        return securityDefinitionOptionParameterMessage;
                    }
                }
            }

            return securityDefinitionOptionParameterMessage;
        }

        public TickPriceMessage reqMktData(
            Contract contract,
            string generickTickList,
            bool snapshot,
            bool regulatorySnapshot,
            List<TagValue> mktDataOptions)
        {
            var tickPriceMessageList = reqMktDataAsList(
                contract,
                generickTickList,
                snapshot,
                regulatorySnapshot,
                mktDataOptions);

            if (!tickPriceMessageList.Any())
            {
                return null;
            }

            return tickPriceMessageList.OrderByDescending(m => m.Price).First();
        }

        public List<TickPriceMessage> reqMktDataAsList(
            Contract contract,
            string generickTickList,
            bool snapshot,
            bool regulatorySnapshot,
            List<TagValue> mktDataOptions)
        {
            var mktDataAsTuple = reqMktDataAsTuple(
                contract,
                generickTickList,
                snapshot,
                regulatorySnapshot,
                mktDataOptions);

            if (mktDataAsTuple.Item1.All(t => t.Size == -1)
                && mktDataAsTuple.Item2.All(t => (int)Math.Round(t.Price, 0) == -1))
            {
                return new List<TickPriceMessage>();
            }

            return mktDataAsTuple.Item2;
        }

        public Tuple<List<TickSizeMessage>, List<TickPriceMessage>> reqMktDataAsTuple(
            Contract contract,
            string generickTickList,
            bool snapshot,
            bool regulatorySnapshot,
            List<TagValue> mktDataOptions)
        {
            var tickSizeMessageList = new List<TickSizeMessage>();
            var tickPriceMessageList = new List<TickPriceMessage>();

            var reqId = ++_reqIdMktData;
            _ibClient.reqMktData(reqId, contract, generickTickList, snapshot, regulatorySnapshot, mktDataOptions);

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            while (stopWatch.Elapsed.TotalMilliseconds < _configurationManager.GetConfiguration().Timeout)
            {
                var message = _ibClientQueue.Dequeue();

                if (message is TickSizeMessage sizeMessage)
                {
                    tickSizeMessageList.Add(sizeMessage);
                }
                if (message is TickPriceMessage priceMessage)
                {
                    tickPriceMessageList.Add(priceMessage);
                }
                if (message is string messageAsString)
                {
                    if (messageAsString == Constants.ON_TICK_SNAPSHOT_END)
                    {
                        return new Tuple<List<TickSizeMessage>, List<TickPriceMessage>>(
                            tickSizeMessageList, tickPriceMessageList);
                    }
                }
            }

            return new Tuple<List<TickSizeMessage>, List<TickPriceMessage>>(
                new List<TickSizeMessage>(), new List<TickPriceMessage>());
        }
    }
}
