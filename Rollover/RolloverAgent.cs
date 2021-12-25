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
        private IRepository _repository;
        private IInputLoop _inputLoop;

        public RolloverAgent(
            IConfigurationManager configurationManager,
            IConsoleWrapper consoleWrapper,
            IInputQueue inputQueue,
            IRepository repository, 
            IInputLoop inputLoop)
        {
            _configurationManager = configurationManager;
            _consoleWrapper = consoleWrapper;
            _inputQueue = inputQueue;
            _repository = repository;
            _inputLoop = inputLoop;
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
            var connected = _repository.Connect(configuration.Host, configuration.Port, configuration.ClientId);
            if (!connected)
            {
                _consoleWrapper.WriteLine("Can not connect!");
                return;
            }
 
            // List positions
            var positionList = _repository.AllPositions();
            positionList.ForEach(p => _consoleWrapper.WriteLine(p));
            _consoleWrapper.WriteLine(Reducer.ENTER_SYMBOL_TO_TRACK);

            // Start input loop
            _inputLoop.Run(_consoleWrapper, _inputQueue);

            // Disconnect
            _repository.Disconnect();
        }
    }
}