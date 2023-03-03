﻿using IbClient;
using SsbHedger.Model;
using System;
using System.Linq;
using IbClient.messages;
using SsbHedger.SsbConfiguration;
using IBApi;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Globalization;
using System.Net;

namespace SsbHedger
{
    public class IbHost : IIbHost
    {
        private readonly int TIMEOUT = 2000;
        private readonly int REQ_MKT_DATA_BULL_HEDGE_CALL_ID = 3001;
        private readonly int REQ_MKT_DATA_BEAR_HEDGE_CALL_ID = 3002;

        IConfiguration _configuration;
        IIBClient _ibClient;

        int _reqIdHistoricalData = 1000;
        int _reqContractDetails = 2000;
        Dictionary<int, double> _reqIdStrikeDict = null!;
        Dictionary<string, Contract> _contractDict = null!;
        Contract _contractUnderlying = null!;
        string _durationString = "1 D";
        string _barSizeSetting = "5 mins";
        string _whatToShow = "BID";
        int _useRTH = 0;
        bool _keepUpToDate = false;

        public IbHost(IConfiguration configuration)
        {
            _configuration = configuration;

            _ibClient = IBClient.CreateClient();

            _ibClient.Error += _ibClient_Error;
            _ibClient.NextValidId += _ibClient_NextValidId;
            _ibClient.ManagedAccounts += _ibClient_ManagedAccounts;
            _ibClient.ConnectionClosed += _ibClient_ConnectionClosed;
            _ibClient.HistoricalData += _ibClient_HistoricalData;
            _ibClient.HistoricalDataUpdate += _ibClient_HistoricalDataUpdate;
            _ibClient.HistoricalDataEnd += _ibClient_HistoricalDataEnd;
            _ibClient.Position += _ibClient_Position;
            _ibClient.PositionEnd += _ibClient_PositionEnd;
            _ibClient.ContractDetails += _ibClient_ContractDetails;
            _ibClient.ContractDetailsEnd += _ibClient_ContractDetailsEnd;
            _ibClient.TickPrice += _ibClient_TickPrice;
            _ibClient.TickSize += _ibClient_TickSize;
            _ibClient.TickPrice += _ibClient_TickPrice;
            _ibClient.TickString += _ibClient_TickString;

            _reqIdStrikeDict = new Dictionary<int, double>();
            _contractDict = new Dictionary<string, Contract>
            {
                {"SPY", new Contract(){Symbol = "SPY", SecType = "STK", Currency="USD", Exchange = "SMART"} }
            };

            _contractUnderlying = _contractDict[(string)_configuration.GetValue(Configuration.UNDERLYING_SYMBOL)];

        }

        public MainWindowViewModel? ViewModel { get; set; }

        public async Task<bool> ConnectAndStartReaderThread()
        {
            return await Task.Run(() =>
            {
                if (ViewModel == null)
                {
                    throw new ApplicationException("Unexpected! ViewModel is null");
                }

                _ibClient.ConnectAndStartReaderThread(
                                (string)_configuration.GetValue(Configuration.HOST),
                                (int)_configuration.GetValue(Configuration.PORT),
                                (int)_configuration.GetValue(Configuration.CLIENT_ID));

                var startTime = DateTime.Now;
                while ((DateTime.Now - startTime).TotalMilliseconds < TIMEOUT && !ViewModel.Connected) { }
                return ViewModel.Connected;
            });
        }

        public void Disconnect()
        {
            _ibClient.Disconnect();
        }

        public void ReqHistoricalData()
        {
            _reqIdHistoricalData++;
            _ibClient.ClientSocket.reqHistoricalData(
                _reqIdHistoricalData,
                _contractUnderlying,
                GetEndDateTime(),
                _durationString,
                _barSizeSetting,
                _whatToShow,
                _useRTH,
                1,
                _keepUpToDate,
                new List<TagValue>());
        }

        public void ReqPositions()
        {
            _ibClient.ClientSocket.reqPositions();
        }

        public void ApplyDefaultHistoricalData()
        {
            // TODO read from file

            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }

            var bars = new List<Model.Bar>()
            {
                new Model.Bar(DateTime.ParseExact("20230111 10:00:00", "yyyyMMdd hh:mm:ss", CultureInfo.InvariantCulture),
                    390.44, 390.93, 390.2, 390.84),
                new Model.Bar(DateTime.ParseExact("20230111 10:05:00", "yyyyMMdd hh:mm:ss", CultureInfo.InvariantCulture),
                    390.84, 391.18, 390.78, 391.01),
                new Model.Bar(DateTime.ParseExact("20230111 10:10:00", "yyyyMMdd hh:mm:ss", CultureInfo.InvariantCulture),
                    391.01, 391.07, 390.93, 391.02),
                new Model.Bar(DateTime.ParseExact("20230111 10:15:00", "yyyyMMdd hh:mm:ss", CultureInfo.InvariantCulture),
                    391.02, 391.5, 390.98, 391.46)
            };

            bars.ForEach(b => ViewModel.Bars.Add(b));


            //throw new NotImplementedException();

        }

        private string GetEndDateTime()
        {
            // TODO
            return "20230111 22:15:00";
        }

        private void _ibClient_Error(int reqId, int code, string message, Exception exception)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }
            ViewModel.Messages.Add(new Message(reqId, $"Code:{code} message:{message} exception:{exception}"));
            UpdateConnectionMessage(ViewModel.Connected);
        }

        private void _ibClient_ManagedAccounts(ManagedAccountsMessage message)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }
            ViewModel.Messages.Add(new Message(0, $"Managed accounts: {message.ManagedAccounts.Aggregate((r, n) => r + "," + n)}"));
        }

        private void _ibClient_NextValidId(ConnectionStatusMessage message)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }
            ViewModel.Messages.Add(new Message(0, message.IsConnected ? "CONNECTED!" : "NOT CONNECTED!"));
            ViewModel.Connected = message.IsConnected;
            UpdateConnectionMessage(message.IsConnected);
        }

        private void UpdateConnectionMessage(bool isConnected)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }

            ViewModel.ConnectionMessage = isConnected
                    ? $"CONNECTED! {_configuration.GetValue(Configuration.HOST)}, " +
                    $"{_configuration.GetValue(Configuration.PORT)}, client ID: {_configuration.GetValue(Configuration.CLIENT_ID)}"
                    : $"NOT CONNECTED! {_configuration.GetValue(Configuration.HOST)}, " +
                    $"{_configuration.GetValue(Configuration.PORT)}, client ID: {_configuration.GetValue(Configuration.CLIENT_ID)}";
        }

        private void _ibClient_ConnectionClosed()
        {
            const string DISCONNECTED = "DISCONNECTED!";


            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }
            ViewModel.Messages.Add(new Message(0, DISCONNECTED));
            ViewModel.Connected = false;
            ViewModel.ConnectionMessage = DISCONNECTED;
        }

        private void _ibClient_HistoricalData(HistoricalDataMessage message)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }
            ViewModel.Messages.Add(new Message(message.RequestId,
                $"HistoricalData: {message.Date} {message.Open} {message.High} {message.Low} {message.Close}"));
        }

        private void _ibClient_HistoricalDataUpdate(HistoricalDataMessage message)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }
            ViewModel.Messages.Add(new Message(message.RequestId,
                $"HistoricalDataUpdate: {message.Date} {message.Open} {message.High} {message.Low} {message.Close}"));

        }

        private void _ibClient_HistoricalDataEnd(HistoricalDataEndMessage message)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }
            ViewModel.Messages.Add(new Message(message.RequestId,
                $"HistoricalDataEnd: {message.StartDate} {message.EndDate} "));
        }

        private void _ibClient_Position(PositionMessage positionMessage)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }

            ViewModel.Messages.Add(new Message(0,
                $"PositionMessage: {positionMessage.Contract.ConId} " +
                $"{positionMessage.Contract.LocalSymbol} " +
                $"{positionMessage.AverageCost} " +
                $"{positionMessage.Position}"));

            if (positionMessage.Position != 0 && positionMessage.Contract != null)
            {
                if (positionMessage.Contract.Right == "C")
                {
                    SetSize(positionMessage);
                    SetCallStrike(positionMessage);
                    SetCallPrice(positionMessage);
                    SetBullHedgeStrike(positionMessage);

                    var newStrike = GetHigherStrike(positionMessage.Contract.Strike);
                    var contractForHedge = CopyContractWithOtherStrike(positionMessage.Contract, newStrike);
                    _reqContractDetails++;
                    _ibClient.ClientSocket.reqContractDetails(_reqContractDetails, contractForHedge);
                }
                if (positionMessage.Contract.Right == "P")
                {
                    SetSize(positionMessage);
                    SetPutStrike(positionMessage);
                    SetPutPrice(positionMessage);
                    SetBearHedgeStrike(positionMessage);

                    var newStrike = GetLowerStrike(positionMessage.Contract.Strike);
                    var contractForHedge = CopyContractWithOtherStrike(positionMessage.Contract, newStrike);
                    _reqContractDetails++;
                    _ibClient.ClientSocket.reqContractDetails(_reqContractDetails, contractForHedge);
                }
            }
        }

        private double GetLowerStrike(double strike)
        {
            return strike - 1;
        }

        private double GetHigherStrike(double strike)
        {
            return strike + 1;
        }

        private void _ibClient_PositionEnd()
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }
            ViewModel.Messages.Add(new Message(0, $"PositionEnd"));
        }

        private void _ibClient_ContractDetails(ContractDetailsMessage contractDetailsMessage)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }

            ViewModel.Messages.Add(new Message(0,
                $"ContractDetailsMessage: {contractDetailsMessage.ContractDetails.Contract.ConId} " +
                $"{contractDetailsMessage.ContractDetails.Contract.Symbol} " +
                $"{contractDetailsMessage.ContractDetails.Contract.Exchange} " +
                $"{contractDetailsMessage.ContractDetails.Contract.Right} " +
                $"{contractDetailsMessage.ContractDetails.Contract.LastTradeDateOrContractMonth} " +
                $"{contractDetailsMessage.ContractDetails.Contract.Strike} " +
                $"{contractDetailsMessage.ContractDetails.Contract.LocalSymbol}"));

            if (_reqIdStrikeDict.ContainsKey(contractDetailsMessage.RequestId))
            {
                _reqIdStrikeDict.Remove(contractDetailsMessage.RequestId);
            }
            else
            {
                int reqMktDataId = contractDetailsMessage.ContractDetails.Contract.Right == "P"
                    ? REQ_MKT_DATA_BEAR_HEDGE_CALL_ID
                    : REQ_MKT_DATA_BULL_HEDGE_CALL_ID;
                _ibClient.ClientSocket.reqMktData(
                    reqMktDataId,
                    contractDetailsMessage.ContractDetails.Contract,
                    "",
                    false,
                    false,
                    new List<TagValue>());
            }
        }

        private void _ibClient_ContractDetailsEnd(int obj)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }
            ViewModel.Messages.Add(new Message(0, $"ContractDetailsEnd"));
        }

        private void _ibClient_TickString(int tickerId, int tickType, string value)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }

            ViewModel.Messages.Add(new Message(0,
                $"TickString: {tickerId} {tickType} {value}"));
        }

        private void _ibClient_TickSize(TickSizeMessage tickSizeMessage)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }

            ViewModel.Messages.Add(new Message(0,
                $"TickSize: {tickSizeMessage.Field} {tickSizeMessage.Size}"));
        }

        private void _ibClient_TickPrice(TickPriceMessage tickPriceMessage)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }

            ViewModel.Messages.Add(new Message(0,
                $"TickPrice: {tickPriceMessage.Field} {tickPriceMessage.Price}"));

            if (tickPriceMessage.Field == 2)  // ask. Use 1 for bid
            {
                if (tickPriceMessage.RequestId == REQ_MKT_DATA_BEAR_HEDGE_CALL_ID)
                {
                    ViewModel.BearHedgePrice = tickPriceMessage.Price;
                }
                if (tickPriceMessage.RequestId == REQ_MKT_DATA_BULL_HEDGE_CALL_ID)
                {
                    ViewModel.BullHedgePrice = tickPriceMessage.Price;
                }
            }
        }

        private Contract CopyContractWithOtherStrike(Contract contract, double newStrike)
        {
            return new Contract
            {
                Symbol = contract.Symbol,
                SecType = contract.SecType,
                LastTradeDateOrContractMonth = contract.LastTradeDateOrContractMonth,
                Strike = newStrike,
                Right = contract.Right,
                Multiplier = contract.Multiplier,
                Exchange = "SMART",
                Currency = contract.Currency
            };
        }


        private void SetSize(PositionMessage positionMessage)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }

            if (ViewModel.Size != -positionMessage.Position)
            {
                ViewModel.Size = (int)-positionMessage.Position;
            }
        }

        private void SetCallStrike(PositionMessage positionMessage)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }

            ViewModel.CallShortStrike = positionMessage.Contract.Strike;
        }

        private void SetCallPrice(PositionMessage positionMessage)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }

            ViewModel.CallShortPrice = Math.Round(positionMessage.AverageCost / MainWindowViewModel.MULTIPLIER, 3);
        }

        private void SetPutStrike(PositionMessage positionMessage)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }

            ViewModel.PutShortStrike = positionMessage.Contract.Strike;
        }

        private void SetPutPrice(PositionMessage positionMessage)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }

            ViewModel.PutShortPrice = Math.Round(positionMessage.AverageCost / MainWindowViewModel.MULTIPLIER, 3);
        }

        private void SetBearHedgeStrike(PositionMessage positionMessage)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }

            ViewModel.BearHedgeStrike = positionMessage.Contract.Strike - 1;
        }

        private void SetBullHedgeStrike(PositionMessage positionMessage)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }

            ViewModel.BullHedgeStrike = positionMessage.Contract.Strike + 1;
        }

    }
}
