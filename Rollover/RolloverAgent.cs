using System;
using System.Threading;

namespace Rollover
{
    public class RolloverAgent : IRolloverAgent
    {
        private IConfigurationManager _configurationManager;
        private IConsoleWrapper _consoleWrapper;
        private string _input;

        public RolloverAgent(IConfigurationManager configurationManager, IConsoleWrapper inputQueue)
        {
            _configurationManager = configurationManager;
            _consoleWrapper = inputQueue;
        }


        public void Run()
        {
            var configuration = _configurationManager.GetConfiguration();

            while (true)
            {
                new Thread(() => { _input = _consoleWrapper.ReadLine();})
                { IsBackground = true}
                .Start();
                

                // queue.dequeue

                //var input = console.readline();
                if (_input != null && _input.Equals("q", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }
            }
        }
    }
}