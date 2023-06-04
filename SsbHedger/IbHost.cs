using IBApi;
using IbClient;
using IbClient.messages;
using SsbHedger.Model;
using SsbHedger.SsbConfiguration;
using SsbHedger.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SsbHedger
{
    public class IbHost : IIbHost
    {
        private readonly int TIMEOUT = 2000;
        private readonly int REQ_MKT_DATA_SHORT_PUT_ID = 3001;
        private readonly int REQ_MKT_DATA_SHORT_CALL_ID = 3002;
        private readonly int REQ_MKT_DATA_UNDERLYING = 3003;
        private readonly int NEXT_PUT_OPTION_REQ_ID = 4001;
        private readonly int NEXT_CALL_OPTION_REQ_ID = 4002;
        private readonly int CALL_OPTION_IV_REQ_ID = 5001;
        private readonly int PUT_OPTION_IV_REQ_ID = 5002;
        private readonly int CHECK_OPTION_NEXT_STRIKE_REQ_ID = 10000;
        private readonly int CHECK_OPTION_SECOND_STRIKE_REQ_ID = 20000;

        IConfiguration _configuration;
        IIBClient _ibClient;

        int _reqIdHistoricalData = 1000;
        int _reqContractDetails = 2000;
        Dictionary<string, Contract> _contractDict = null!;
        Contract _contractUnderlying = null!;
        string _durationString = "1 D";
        string _barSizeSetting = "5 mins";
        string _whatToShow = "BID";
        int _useRTH = 0;
        bool _keepUpToDate = false;
        private IPositionMessageBuffer _positionMessageBuffer;
        private Contract? _currentPutContract;
        private Contract? _currentCallContract;
        Thread _alertThread = null!;
        private IAtmStrikeUtility _atmStrikeUtility;

        public IbHost(
            IConfiguration configuration,
            IPositionMessageBuffer positionMessageBuffer,
            IAtmStrikeUtility atmStrikeUtility)
        {
            _configuration = configuration;
            _positionMessageBuffer = positionMessageBuffer;
            _atmStrikeUtility = atmStrikeUtility;

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
            _ibClient.TickOptionCommunication += _ibClient_TickOptionCommunication;

            _contractDict = new Dictionary<string, Contract>
            {
                {"SPY", new Contract(){Symbol = "SPY", SecType = "STK", Currency="USD", Exchange = "SMART"} }
            };

            _contractUnderlying = _contractDict[(string)_configuration.GetValue(Configuration.UNDERLYING_SYMBOL)];
        }

        public int Timeout => TIMEOUT;

        public MainWindowViewModel? ViewModel { get; set; }
        public AtmStrikes AtmStrikesCandidate { get; set; }

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

        public void ReqMktDataNextPutOption(double putStike)
        {
            if (_currentPutContract == null)
            {
                return;
            }

            var contract = CopyContractWithOtherStrike(_currentPutContract, putStike);
            _ibClient.ClientSocket.reqMktData(
               NEXT_PUT_OPTION_REQ_ID,
               contract,
               "",
               false,
               false,
               new List<TagValue>());
        }

        public void CancelMktDataNextPutOption()
        {
            _ibClient.ClientSocket.cancelMktData(NEXT_PUT_OPTION_REQ_ID);
        }

        public void ReqMktDataNextCallOption(double callStike)
        {
            if (_currentCallContract == null)
            {
                return;
            }

            var contract = CopyContractWithOtherStrike(_currentCallContract, callStike);

            _ibClient.ClientSocket.reqMktData(
               NEXT_CALL_OPTION_REQ_ID,
               contract,
               "",
               false,
               false,
               new List<TagValue>());
        }

        public void CancelMktDataNextCalllOption()
        {
            _ibClient.ClientSocket.cancelMktData(NEXT_CALL_OPTION_REQ_ID);
        }


        public void ReqMktDataCallOptionIV(double callStike)
        {
            if (_currentCallContract == null)
            {
                return;
            }

            var contract = CopyContractWithOtherStrike(_currentCallContract, callStike);

            _ibClient.ClientSocket.reqMktData(
               CALL_OPTION_IV_REQ_ID,
               contract,
               "",
               false,
               false,
               new List<TagValue>());
        }


        public void CancelMktCallOptionIV()
        {
            _ibClient.ClientSocket.cancelMktData(CALL_OPTION_IV_REQ_ID);
        }

        public void ReqMktDataPutOptionIV(double putStike)
        {
            if (_currentPutContract == null)
            {
                return;
            }

            var contract = CopyContractWithOtherStrike(_currentPutContract, putStike);
            _ibClient.ClientSocket.reqMktData(
               PUT_OPTION_IV_REQ_ID,
               contract,
               "",
               false,
               false,
               new List<TagValue>());
        }

        public void CancelMktPutOptionIV()
        {
            _ibClient.ClientSocket.cancelMktData(PUT_OPTION_IV_REQ_ID);
        }

        public void ReqMktUnderlying()
        {
            if (_contractUnderlying == null)
            {
                return;
            }

            _ibClient.ClientSocket.reqMktData(
               REQ_MKT_DATA_UNDERLYING,
               _contractUnderlying,
               "",
               false,
               false,
               new List<TagValue>());
        }

        public void CancelMktUnderlying()
        {
            _ibClient.ClientSocket.cancelMktData(REQ_MKT_DATA_UNDERLYING);
        }

        public void ReqCheckNextOptionsStrike(double atmStrikeCandidate)
        {
            if (_currentCallContract == null)
            {
                return;
            }

            var contract = CopyContractWithOtherStrike(_currentCallContract, atmStrikeCandidate);

            _ibClient.ClientSocket.reqMktData(
               CHECK_OPTION_NEXT_STRIKE_REQ_ID,
               contract,
               "",
               true,
               false,
               new List<TagValue>());
        }

        public void ReqCheckSecondOptionsStrike(double atmStrikeCandidate)
        {
            if (_currentCallContract == null)
            {
                return;
            }

            var contract = CopyContractWithOtherStrike(_currentCallContract, atmStrikeCandidate);

            _ibClient.ClientSocket.reqMktData(
               CHECK_OPTION_SECOND_STRIKE_REQ_ID,
               contract,
               "",
               true,
               false,
               new List<TagValue>());
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
            ViewModel.PositionsInfoMessage = "";

            // old code
            //if (positionMessage.Position != 0 && positionMessage.Contract != null)
            //{
            //    // short call
            //    if (positionMessage.Contract.Right == "C" && positionMessage.Position < 0)
            //    {
            //        SetSize(positionMessage);
            //        SetCallStrike(positionMessage);
            //        SetCallPrice(positionMessage);

            //        var contractForHedge = CopyContractWithOtherStrikeAndRight(positionMessage.Contract, ViewModel.BullHedgeStrike);
            //        _reqContractDetails++;
            //        _ibClient.ClientSocket.reqContractDetails(_reqContractDetails, contractForHedge);
            //    }
            //    // short put
            //    if (positionMessage.Contract.Right == "P" && positionMessage.Position < 0)
            //    {
            //        SetSize(positionMessage);
            //        SetPutStrike(positionMessage);
            //        SetPutPrice(positionMessage);

            //        var contractForHedge = CopyContractWithOtherStrikeAndRight(positionMessage.Contract, ViewModel.BearHedgeStrike);
            //        _reqContractDetails++;
            //        _ibClient.ClientSocket.reqContractDetails(_reqContractDetails, contractForHedge);
            //    }
            //}
            // end old code

            if (positionMessage.Position != 0 && positionMessage.Contract != null)
            {
                if (positionMessage.Contract.Right == "P")
                {
                    _currentPutContract = positionMessage.Contract;
                }
                if (positionMessage.Contract.Right == "C")
                {
                    _currentCallContract = positionMessage.Contract;
                }


                _positionMessageBuffer.AddMessage(positionMessage);
            }
        }

        private void _ibClient_PositionEnd()
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }

            //SetRight(_positionMessageBuffer);
            //SetSize(_positionMessageBuffer);
            //SetStrikes(_positionMessageBuffer);
            //_positionMessageBuffer.Reset();

            //_ibClient.ClientSocket.reqMktData(
            //    REQ_MKT_DATA_SPY,
            //    _contractUnderlying,
            //    "",
            //    false,
            //    false,
            //    new List<TagValue>());

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

            int reqMktDataId = contractDetailsMessage.ContractDetails.Contract.Right == "C"
                ? REQ_MKT_DATA_SHORT_CALL_ID
                : REQ_MKT_DATA_SHORT_PUT_ID;
            _ibClient.ClientSocket.reqMktData(
                reqMktDataId,
                contractDetailsMessage.ContractDetails.Contract,
                "",
                false,
                false,
                new List<TagValue>());
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
            //if (ViewModel == null)
            //{
            //    throw new ApplicationException("Unexpected! ViewModel is null");
            //}

            //ViewModel.Messages.Add(new Message(0,
            //    $"TickString: {tickerId} {tickType} {value}"));
        }

        private void _ibClient_TickSize(TickSizeMessage tickSizeMessage)
        {
            //if (ViewModel == null)
            //{
            //    throw new ApplicationException("Unexpected! ViewModel is null");
            //}

            //ViewModel.Messages.Add(new Message(0,
            //    $"TickSize: {tickSizeMessage.Field} {tickSizeMessage.Size}"));
        }

        private void _ibClient_TickPrice(TickPriceMessage tickPriceMessage)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }

            if (tickPriceMessage.Field == 1)  // bid. Use 2 for ask
            {
                if (tickPriceMessage.RequestId == REQ_MKT_DATA_UNDERLYING)
                {
                    ViewModel.UnderlyingPrice = tickPriceMessage.Price;
                    //_atmStrikeUtility.SetAtmStrikesInViewModel(this, tickPriceMessage.Price);
                }
            }

            //ViewModel.Messages.Add(new Message(0,
            //    $"TickPrice: {tickPriceMessage.Field} {tickPriceMessage.Price}"));

            //if (tickPriceMessage.Field == 2)  // ask. Use 1 for bid
            //{
            //    if (tickPriceMessage.RequestId == REQ_MKT_DATA_SHORT_CALL_ID)
            //    {
            //        ViewModel.BearHedgePrice = tickPriceMessage.Price;
            //    }
            //}
            //if (tickPriceMessage.Field == 1)  // bid. Use 2 for ask
            //{
            //    if (tickPriceMessage.RequestId == REQ_MKT_DATA_SPY)
            //    {
            //        ViewModel.SpyPrice = tickPriceMessage.Price;
            //    }
            //    if (tickPriceMessage.RequestId == NEXT_PUT_OPTION_REQ_ID)
            //    {
            //        ViewModel.SpyPrice = tickPriceMessage.Price;
            //    }
            //}
        }

        private void _ibClient_TickOptionCommunication(TickOptionMessage message)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }

            ViewModel.Messages.Add(new Message(0,
                $"TickOptionCommunication: RequestId:{message.RequestId} Field:{message.Field} " +
                $"ImpliedVolatility:{message.ImpliedVolatility} Delta:{message.Delta} " +
                $"OptPrice:{message.OptPrice} PvDividend:{message.PvDividend} " +
                $"Gamma: {message.Gamma} Vega:{message.Vega} " +
                $"Theta:{message.Theta} UndPrice: {message.UndPrice}"));

            if (message.RequestId == NEXT_PUT_OPTION_REQ_ID)
            {
                ViewModel.NextPutDelta = Math.Round(message.Delta, 3);
            }
            if (message.RequestId == NEXT_CALL_OPTION_REQ_ID)
            {
                ViewModel.NextCallDelta = Math.Round(message.Delta, 3);

            }
            if (message.RequestId == CHECK_OPTION_NEXT_STRIKE_REQ_ID)
            {
                ViewModel.NextAtmStrike = AtmStrikesCandidate.NextAtmStrike;
            }
            if (message.RequestId == CHECK_OPTION_SECOND_STRIKE_REQ_ID)
            {
                ViewModel.SecondAtmStrike = AtmStrikesCandidate.SecondAtmStrike;
            }
            if (message.RequestId == CALL_OPTION_IV_REQ_ID)
            {
                // Computed Greeks and implied volatility based on the underlying stock price and the option model price. Correspond to greeks shown in TWS. See Option Greeks
                // https://interactivebrokers.github.io/tws-api/tick_types.html
                if (message.Field == 13)
                {
                    ViewModel.IvCall = Math.Round(message.ImpliedVolatility, 4);
                }
            }
            if (message.RequestId == PUT_OPTION_IV_REQ_ID)
            {
                // Computed Greeks and implied volatility based on the underlying stock price and the option model price. Correspond to greeks shown in TWS. See Option Greeks
                // https://interactivebrokers.github.io/tws-api/tick_types.html
                if (message.Field == 13)
                {
                    ViewModel.IvPut = Math.Round(message.ImpliedVolatility, 4);
                }
            }

            // Delta alert
            if (Math.Abs(ViewModel.NextPutDelta) <= ViewModel.DeltaThreshold / 100
                || Math.Abs(ViewModel.NextCallDelta) <= ViewModel.DeltaThreshold / 100)
            {
                _alertThread = new Thread(new ThreadStart(AlertFunctionDelta)) { IsBackground = true };
                _alertThread.Start();
            }

            // Volatility alert
            if (ViewModel.IvAverage > ViewModel.IvThreshold)
            {
                _alertThread = new Thread(new ThreadStart(AlertFunctionIv)) { IsBackground = true };
                _alertThread.Start();
            }
        }

        private void AlertFunctionDelta()
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }

            while (ViewModel.DeltaAlertActive)
            {
                SoundPlayer player = new SoundPlayer(Properties.Resources.DeltaAlert);
                player.Play();
                Thread.Sleep(10000);
            }
        }

        private void AlertFunctionIv()
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }

            while (ViewModel.VolatilityAlertActive)
            {
                SoundPlayer player = new SoundPlayer(Properties.Resources.DeltaAlert);
                player.Play();
                Thread.Sleep(10000);
            }
        }

        private Contract CopyContractWithOtherStrikeAndRight(Contract contract, double newStrike)
        {
            return new Contract
            {
                Symbol = contract.Symbol,
                SecType = contract.SecType,
                LastTradeDateOrContractMonth = contract.LastTradeDateOrContractMonth,
                Strike = newStrike,
                Right = contract.Right == "C" ? "P" : "C",
                Multiplier = contract.Multiplier,
                Exchange = "SMART",
                Currency = contract.Currency
            };
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

        private void SetRight(IPositionMessageBuffer positionMessageBuffer)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }

            if (positionMessageBuffer.Messages.Count > 0 && ViewModel.Right0 != positionMessageBuffer.Messages[0]?.Contract?.Right)
            {
                ViewModel.Right0 = positionMessageBuffer.Messages[0]?.Contract?.Right;
            }
            if (positionMessageBuffer.Messages.Count > 1 && ViewModel.Right1 != positionMessageBuffer.Messages[1]?.Contract?.Right)
            {
                ViewModel.Right1 = positionMessageBuffer.Messages[1]?.Contract?.Right;
            }
            if (positionMessageBuffer.Messages.Count > 2 && ViewModel.Right2 != positionMessageBuffer.Messages[2]?.Contract?.Right)
            {
                ViewModel.Right2 = positionMessageBuffer.Messages[2]?.Contract?.Right;
            }
            if (positionMessageBuffer.Messages.Count > 3 && ViewModel.Right3 != positionMessageBuffer.Messages[3]?.Contract?.Right)
            {
                ViewModel.Right3 = positionMessageBuffer.Messages[3]?.Contract?.Right;
            }
            if (positionMessageBuffer.Messages.Count > 4 && ViewModel.Right4 != positionMessageBuffer.Messages[4]?.Contract?.Right)
            {
                ViewModel.Right4 = positionMessageBuffer.Messages[4]?.Contract?.Right;
            }
        }

        private void SetSize(IPositionMessageBuffer positionMessageBuffer)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }

            int? firstCallSize = positionMessageBuffer.FirstCallSize();
            int? secondCallSize = positionMessageBuffer.SecondCallSize();

            if (firstCallSize.HasValue && ViewModel.Size1 != -firstCallSize)
            {
                ViewModel.Size1 = -firstCallSize.Value;
            }
            if (secondCallSize.HasValue && ViewModel.Size2 != -secondCallSize)
            {
                ViewModel.Size2 = -secondCallSize.Value;
            }
        }

        private void SetStrikes(IPositionMessageBuffer positionMessageBuffer)
        {
            if (ViewModel == null)
            {
                throw new ApplicationException("Unexpected! ViewModel is null");
            }

            double? firstCallStrike = positionMessageBuffer.FirstCallStrike();
            double? secondCallStrike = positionMessageBuffer.SecondCallStrike();

            if (firstCallStrike.HasValue && ViewModel.Strike1 != firstCallStrike)
            {
                ViewModel.Strike1 = firstCallStrike.Value;
            }
            if (secondCallStrike.HasValue && ViewModel.Strike2 != secondCallStrike)
            {
                ViewModel.Strike2 = secondCallStrike.Value;
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

        public void GetStrikes(string v1, string v2, int v3)
        {
            throw new NotImplementedException();
        }
    }
}
