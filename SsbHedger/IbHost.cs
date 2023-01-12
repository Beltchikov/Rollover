using IbClient;
using SsbHedger.Model;
using System;
using System.Linq;
using IbClient.messages;
using SsbHedger.SsbConfiguration;
using IBApi;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SsbHedger
{
    public class IbHost : IIbHost
    {
        private readonly int TIMEOUT = 2000;
        IConfiguration _configuration;
        IIBClient _ibClient;
       
        int _reqIdHistoricalData = 1000;
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

            _contractDict = new Dictionary<string, Contract>
            {
                {"SPY", new Contract(){Symbol = "SPY", SecType = "STK", Currency="USD", Exchange = "SMART"} }
            };

            _contractUnderlying = _contractDict[(string)_configuration.GetValue(Configuration.UNDERLYING_SYMBOL)];

        }

        public MainWindowViewModel? ViewModel { get; set; }

        public async Task<bool> ConnectAndStartReaderThread()
        {
            return await Task.Run(() => {
                if (ViewModel == null)
                {
                    throw new ApplicationException("Unexpected! ViewModel is null");
                }
                _ibClient.ConnectAndStartReaderThread(
                    (string)_configuration.GetValue(Configuration.HOST),
                    (int)_configuration.GetValue(Configuration.PORT),
                    (int)_configuration.GetValue(Configuration.CLIENT_ID));

                var startTime = DateTime.Now;
                while ((DateTime.Now - startTime).Milliseconds < TIMEOUT && !ViewModel.Connected) { }
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

        public void ApplyDefaultHistoricalData()
        {
            throw new NotImplementedException();
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
    }
}
