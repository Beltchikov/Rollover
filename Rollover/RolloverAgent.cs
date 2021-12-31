using Rollover.Configuration;
using Rollover.Ib;
using Rollover.Input;
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

        public RolloverAgent(
            IConfigurationManager configurationManager,
            IConsoleWrapper consoleWrapper,
            IInputQueue inputQueue,
            IIbClientQueue ibClientQueue,
            IRepository repository,
            IInputLoop inputLoop, 
            ITwsConnector twsConnector)
        {
            _configurationManager = configurationManager;
            _consoleWrapper = consoleWrapper;
            _inputQueue = inputQueue;
            _repository = repository;
            _inputLoop = inputLoop;
            _ibClientQueue = ibClientQueue;
            _twsConnector = twsConnector;
        }

        public IRepository Repository => _repository;

        public void Run()
        {
            _twsConnector.Connect();

            var positionMessageList = _repository.AllPositions();
            positionMessageList.ForEach(p => _consoleWrapper.WriteLine(p.Contract.LocalSymbol));

            _inputLoop.Run(_consoleWrapper, _inputQueue, _ibClientQueue);
            _repository.Disconnect();
        }
    }
}