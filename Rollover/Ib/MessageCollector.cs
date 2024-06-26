﻿using IBApi;
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

        private int _timeout = 0;

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
            _ibClient.reqPositions();
            return CollectIbResponses<PositionMessage>(_ibClientQueue, Constants.ON_POSITION_END, _timeout);
        }

        public List<ContractDetailsMessage> reqContractDetails(Contract contract)
        {
            var reqId = ++_reqIdContractDetails;
            _ibClient.reqContractDetails(reqId, contract);
            return CollectIbResponses<ContractDetailsMessage>(_ibClientQueue, Constants.ON_CONTRACT_DETAILS_END, _timeout);
        }

        public List<SecurityDefinitionOptionParameterMessage> reqSecDefOptParams(
            string symbol,
            string exchange,
            string secType,
            int conId)
        {
            var reqId = ++_reqIdSecDefOptParam;
            _ibClient.reqSecDefOptParams(reqId, symbol, exchange, secType, conId);
            return CollectIbResponses<SecurityDefinitionOptionParameterMessage>(
                _ibClientQueue,
                Constants.ON_SECURITY_DEFINITION_OPTION_PARAMETER_END,
                _timeout);
        }

        public List<TMessage> CollectIbResponses<TMessage>(IIbClientQueue queue, string endToken, int timeout)
        {
            List<TMessage> positionMessages = new List<TMessage>();
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

        //public TickPriceMessage reqMktData(
        //    Contract contract,
        //    string generickTickList,
        //    bool snapshot,
        //    bool regulatorySnapshot,
        //    List<TagValue> mktDataOptions)
        //{
        //    Tuple<List<TickSizeMessage>, List<TickPriceMessage>> mktDataAsTuple = reqMktDataAsTuple(
        //        contract,
        //        generickTickList,
        //        snapshot,
        //        regulatorySnapshot,
        //        mktDataOptions);
        //    var tickPriceMessageList = mktDataAsTuple.Item2;

        //    // Last Price	4	Last price at which the contract traded (does not include some trades in RTVolume).
        //    // https://interactivebrokers.github.io/tws-api/tick_types.html
        //    return tickPriceMessageList.FirstOrDefault(m => m.Field == 4);
        //}

        public Tuple<List<TickSizeMessage>, List<TickPriceMessage>> reqMktData(
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

            int msgCountBefore = 0;
            bool endMessageReceived = false;
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
                if (message is string messageAsString && messageAsString == Constants.ON_TICK_SNAPSHOT_END)
                {
                    endMessageReceived = true;
                }
                if (endMessageReceived && msgCountBefore > 0 && (tickSizeMessageList.Count() + tickPriceMessageList.Count()) == msgCountBefore)
                {
                    return new Tuple<List<TickSizeMessage>, List<TickPriceMessage>>(
                        tickSizeMessageList, tickPriceMessageList);
                }
                msgCountBefore = tickSizeMessageList.Count() + tickPriceMessageList.Count();
            }

            return new Tuple<List<TickSizeMessage>, List<TickPriceMessage>>(
                new List<TickSizeMessage>(), new List<TickPriceMessage>());
        }

        public bool IsConnected()
        {
            return _ibClient.IsConnected();
        }

        public void placeOrder(Contract contract, Order order)
        {
            _ibClient.PlaceOrder(contract, order);

            // TODO
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            while (stopWatch.Elapsed.TotalMilliseconds < _configurationManager.GetConfiguration().Timeout)
            {
                var message = _ibClientQueue.Dequeue();

                if (message is string errorMessage)
                {
                    
                }
                if (message is ConnectionStatusMessage connectionStatusMessage)
                {
               
                }
                if (message is OpenOrderMessage openOrderMessage)
                {
                   
                }
                if (message is OrderStatusMessage orderStatusMessage)
                {

                }
                if (message is ExecutionMessage executionMessage)
                {

                }
                if (message is int)
                {

                }
            }
        }

        public void reqId()
        {
            _ibClient.reqIds(); 
        }
    }
}
