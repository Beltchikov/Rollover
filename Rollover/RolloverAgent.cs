using Rollover.Configuration;
using Rollover.Input;
using System;
using System.Threading;

namespace Rollover
{
    public class RolloverAgent : IRolloverAgent
    {
        private IConfigurationManager _configurationManager;
        private IConsoleWrapper _consoleWrapper;
        private IInputQueue _inputQueue;

        public RolloverAgent(
            IConfigurationManager configurationManager, 
            IConsoleWrapper consoleWrapper,
            IInputQueue inputQueue)
        {
            _configurationManager = configurationManager;
            _consoleWrapper = consoleWrapper;
            _inputQueue = inputQueue;
        }


        public void Run()
        {
            var configuration = _configurationManager.GetConfiguration();
            var msg = $"Rollover: Host:{configuration.Host} Port:{configuration.Port} ClientId:{configuration.ClientId} Input Q to quit.";
            _consoleWrapper.WriteLine(msg);

            new Thread(() => { _inputQueue.Enqueue(_consoleWrapper.ReadLine()); })
            { IsBackground = true }
            .Start();

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