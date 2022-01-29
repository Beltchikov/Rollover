using Rollover.Configuration;
using Rollover.Ib;
using Rollover.Input;
using Rollover.Tracking;
using System;
using System.Linq;
using System.Threading;

namespace Rollover
{
    public class RolloverAgent : IRolloverAgent
    {
        private IConfigurationManager _configurationManager;
        private IConsoleWrapper _consoleWrapper;
        private IInputQueue _inputQueue;
        private IIbClientQueue _ibClientQueue;
        private IRepository _repository;
        private IInputLoop _inputLoop;
        private ITwsConnector _twsConnector;
        private IPortfolio _portfolio;
        private ITrackedSymbols _trackedSymbols;
        private readonly IOrderManager _orderManager;

        public RolloverAgent(
            IConfigurationManager configurationManager,
            IConsoleWrapper consoleWrapper,
            IInputQueue inputQueue,
            IIbClientQueue ibClientQueue,
            IRepository repository,
            IInputLoop inputLoop,
            ITwsConnector twsConnector,
            IPortfolio portfolio,
            ITrackedSymbols trackedSymbols, IOrderManager orderManager)
        {
            _configurationManager = configurationManager;
            _consoleWrapper = consoleWrapper;
            _inputQueue = inputQueue;
            _repository = repository;
            _inputLoop = inputLoop;
            _ibClientQueue = ibClientQueue;
            _twsConnector = twsConnector;
            _portfolio = portfolio;
            _trackedSymbols = trackedSymbols;
            _orderManager = orderManager;
        }

        public void Run()
        {
            // Start input thread
            new Thread(() => {
                while (true) { _inputQueue.Enqueue(_consoleWrapper.ReadLine()); }
            })
            { IsBackground = true }
            .Start();

            // Read configuration
            var configuration = _configurationManager.GetConfiguration();
            var msg = $"Rollover tries to connect. Host:{configuration.Host} Port:{configuration.Port} ClientId:{configuration.ClientId} Input Q to quit.";
            _consoleWrapper.WriteLine(msg);
            
            // Connect
            _twsConnector.Connect(configuration.Host, configuration.Port, configuration.ClientId);

            _consoleWrapper.WriteLine("Retrieving positions... Please wait.");
            var positionMessageList = _repository.AllPositions().OrderBy(p => p.Contract.LocalSymbol).ToList();
            positionMessageList.ForEach(p =>
            {
                _portfolio.Add(p);
                _consoleWrapper.WriteLine(p.Contract.LocalSymbol);
            });

            _trackedSymbols.Summary().ForEach(l => _consoleWrapper.WriteLine(l));
            _consoleWrapper.WriteLine(Constants.ENTER_SYMBOL_TO_TRACK);

            var timer = new Timer(
                e => _orderManager.RolloverIfNextStrike(_trackedSymbols),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(_configurationManager.GetConfiguration().PriceRequestIntervalInSeconds));

            _inputLoop.Run(_consoleWrapper, _inputQueue, _ibClientQueue);
            _repository.Disconnect();
        }
    }
}