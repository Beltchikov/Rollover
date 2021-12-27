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

        public RolloverAgent(
            IConfigurationManager configurationManager,
            IConsoleWrapper consoleWrapper,
            IInputQueue inputQueue,
            IIbClientQueue ibClientQueue,
            IRepository repository,
            IInputLoop inputLoop)
        {
            _configurationManager = configurationManager;
            _consoleWrapper = consoleWrapper;
            _inputQueue = inputQueue;
            _repository = repository;
            _inputLoop = inputLoop;
            _ibClientQueue = ibClientQueue;
        }


        public void Run()
        {
            // Read configuration
            var configuration = _configurationManager.GetConfiguration();
            var msg = $"Rollover: Host:{configuration.Host} Port:{configuration.Port} ClientId:{configuration.ClientId} Input Q to quit.";
            _consoleWrapper.WriteLine(msg);

            // Start input thread
            new Thread(() => {
                while (true) { _inputQueue.Enqueue(_consoleWrapper.ReadLine()); }
            })
            { IsBackground = true }
            .Start();

            // Connect
            var connectedTuple = _repository.Connect(configuration.Host, configuration.Port, configuration.ClientId);
            if (!connectedTuple.Item1)
            {
                connectedTuple.Item2.ForEach(m => _consoleWrapper.WriteLine(m));
                _consoleWrapper.WriteLine("Can not connect!");
                return;
            }
            connectedTuple.Item2.ForEach(m => _consoleWrapper.WriteLine(m));

            // List positions
            var positionList = _repository.AllPositions();
            positionList.ForEach(p => _consoleWrapper.WriteLine(p));
            _consoleWrapper.WriteLine(Reducer.ENTER_SYMBOL_TO_TRACK);

            // Start input loop
            _inputLoop.Run(_consoleWrapper, _inputQueue, _ibClientQueue);

            // Disconnect
            _repository.Disconnect();
        }
    }
}