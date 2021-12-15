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
        private IRequestSender _requestSender;
        private IInputLoop _inputLoop;

        public RolloverAgent(
            IConfigurationManager configurationManager,
            IConsoleWrapper consoleWrapper,
            IInputQueue inputQueue,
            IRequestSender requestSender, 
            IInputLoop inputLoop)
        {
            _configurationManager = configurationManager;
            _consoleWrapper = consoleWrapper;
            _inputQueue = inputQueue;
            _requestSender = requestSender;
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

            // Register response handlers
            _requestSender.RegisterResponseHandlers(_inputQueue, new SynchronizationContext());

            // Connect
            _requestSender.Connect(configuration.Host, configuration.Port, configuration.ClientId);
            var connected = _inputLoop.CheckConnectionMessages(_consoleWrapper, _inputQueue, configuration.Timeout);
            if (!connected)
            {
                _consoleWrapper.WriteLine("Can not connect!");
                return;
            }

            // Start input loop
            _inputLoop.Run(_consoleWrapper, _inputQueue);

            // Disconnect
            _requestSender.Disconnect();
        }
    }
}