using Rollover.Configuration;
using Rollover.Ib;
using Rollover.Input;
using Rollover.Tracking;
using System.Linq;

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

        public RolloverAgent(
            IConfigurationManager configurationManager,
            IConsoleWrapper consoleWrapper,
            IInputQueue inputQueue,
            IIbClientQueue ibClientQueue,
            IRepository repository,
            IInputLoop inputLoop,
            ITwsConnector twsConnector,
            IPortfolio portfolio, 
            ITrackedSymbols trackedSymbols)
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
        }

        public void Run()
        {
            _twsConnector.Connect();

            var positionMessageList = _repository.AllPositions().OrderBy(p => p.Contract.LocalSymbol).ToList();
            positionMessageList.ForEach(p =>
            {
                _portfolio.Add(p);
                _consoleWrapper.WriteLine(p.Contract.LocalSymbol);
            });

            _trackedSymbols.Summary().ForEach(l => _consoleWrapper.WriteLine(l));
            _consoleWrapper.WriteLine(Constants.ENTER_SYMBOL_TO_TRACK);

            _inputLoop.Run(_consoleWrapper, _inputQueue, _ibClientQueue);
            _repository.Disconnect();
        }
    }
}