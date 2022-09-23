using IBApi;
using IbClient;
using Microsoft.AspNetCore.Mvc;

namespace TwsWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private IBClient _ibClient;
        private EReaderMonitorSignal _signal;
        bool _isConnected;
        private int _requestId;
        private double _price;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;

            _signal = new EReaderMonitorSignal();
            _ibClient = new IBClient(_signal);

            _ibClient.NextValidId += _ibClient_NextValidId;
            _ibClient.TickPrice += _ibClient_TickPrice;

            _ibClient.ClientSocket.eConnect(
                   "localhost",
                   4001,
                   1);

            // The EReader Thread
            var reader = new EReader(_ibClient.ClientSocket, _signal);
            reader.Start();
            new Thread(() =>
            {
                while (_ibClient.ClientSocket.IsConnected())
                {
                    _signal.waitForSignal();
                    reader.processMsgs();
                }
            })
            { IsBackground = true }
            .Start();

        }

        private void _ibClient_NextValidId(IbClient.messages.ConnectionStatusMessage obj)
        {
            _isConnected = obj.IsConnected;
        }

        private void _ibClient_TickPrice(IbClient.messages.TickPriceMessage obj)
        {
            if(_isConnected)
            {
                _price = obj.Price;
            }
        }

        [HttpGet(Name = "Price")]
        public double Get(string exchange = "ARCA", int conId = 756733)
        {
            Contract contract = new Contract
            {
                Exchange = exchange,
                ConId = conId
            };

            string genericTickList = string.Empty;
            bool snapshot = true; // set it to false to receive permanent stream of data

            _price = 0;
            _requestId++;
            _ibClient.ClientSocket.reqMktData(_requestId, contract, genericTickList, snapshot, false, new List<TagValue>());

            while(_price == 0) { }
            return _price;


            //return 12.4;
        }
    }
}