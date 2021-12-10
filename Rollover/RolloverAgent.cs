using System;

namespace Rollover
{
    public class RolloverAgent : IRolloverAgent
    {
        private IConfigurationManager _configurationManager;
        private IInputQueue _inputQueue;

        public RolloverAgent(IConfigurationManager configurationManager, IInputQueue inputQueue)
        {
            _configurationManager = configurationManager;
            _inputQueue = inputQueue;
        }


        public void Run()
        {
            var configuration = _configurationManager.GetConfiguration();

            while (true)
            {
                var input = _inputQueue.ReadLine();

                // queue.dequeue

                //var input = console.readline();
                if (input != null && input.Equals("q", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }
            }
        }
    }
}