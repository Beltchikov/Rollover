using Rollover.Configuration;
using Rollover.Ib;
using Rollover.Input;
using System.Threading;

namespace Rollover
{
    public class TwsConnector : ITwsConnector
    {
        private IConfigurationManager _configurationManager;
        private IConsoleWrapper _consoleWrapper;
        private IInputQueue _inputQueue;
        private IRepository _repository;

        public TwsConnector(
            IConfigurationManager configurationManager, 
            IConsoleWrapper consoleWrapper, 
            IInputQueue inputQueue, 
            IRepository repository)
        {
            _configurationManager = configurationManager;
            _consoleWrapper = consoleWrapper;
            _inputQueue = inputQueue;
            _repository = repository;
        }

        public void Connect()
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
        }
    }
}