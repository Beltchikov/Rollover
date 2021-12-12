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
        private IIbClientWrapper _ibClient;

        public RolloverAgent(
            IConfigurationManager configurationManager,
            IConsoleWrapper consoleWrapper,
            IInputQueue inputQueue,
            IIbClientWrapper ibClient)
        {
            _configurationManager = configurationManager;
            _consoleWrapper = consoleWrapper;
            _inputQueue = inputQueue;
            _ibClient = ibClient;
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
            _ibClient.RegisterResponseHandlers();

            // Connect
            _ibClient.Connect(configuration.Host, configuration.Port, configuration.ClientId);

            var reader = _ibClient.ReaderFactory();
            reader.Start();

            new Thread(() =>  // https://devblogs.microsoft.com/pfxteam/await-synchronizationcontext-and-console-apps/
            {
                while (_ibClient.IsConnected())
                {
                    _ibClient.WaitForSignal();
                    reader.processMsgs();
                }
            })
            { IsBackground = true }
            .Start();

            // Start input loop
            while (true)
            {
                var input = _inputQueue.Dequeue();
                if (input != null && input.Equals("q", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }
            }
        }
    }
}