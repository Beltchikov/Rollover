using Rollover.Configuration;
using Rollover.Ib;
using Rollover.Input;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rollover
{
    public class RolloverAgent : IRolloverAgent
    {
        private IConfigurationManager _configurationManager;
        private IConsoleWrapper _consoleWrapper;
        private IInputQueue _inputQueue;
        private IRequestSender _requestSender;

        public RolloverAgent(
            IConfigurationManager configurationManager,
            IConsoleWrapper consoleWrapper,
            IInputQueue inputQueue,
            IRequestSender requestSender)
        {
            _configurationManager = configurationManager;
            _consoleWrapper = consoleWrapper;
            _inputQueue = inputQueue;
            _requestSender = requestSender;
        }


        public void Run()
        {
            // Read configuration
            var configuration = _configurationManager.GetConfiguration();
            var msg = $"Rollover: Host:{configuration.Host} Port:{configuration.Port} ClientId:{configuration.ClientId} Input Q to quit.";
            _consoleWrapper.WriteLine(msg);

            // Start input thread
            new Thread(() => { _inputQueue.Enqueue(_consoleWrapper.ReadLine()); })
            { IsBackground = true }
            .Start();

            // Register response handlers
            _requestSender.RegisterResponseHandlers(_inputQueue, new SynchronizationContext());

            // Connect
            _requestSender.Connect(configuration.Host, configuration.Port, configuration.ClientId);

            // Start input loop
            while (true)
            {
                var input = _inputQueue.Dequeue();
                if(input == null)
                {
                    continue;
                }

                _consoleWrapper.WriteLine(input);

                if (input.Equals("q", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }
            }
        }
    }
}