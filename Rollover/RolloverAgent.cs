using System;
using System.Threading;

namespace Rollover
{
    public class RolloverAgent : IRolloverAgent
    {
        private IConfigurationManager _configurationManager;
        private IInputQueue _inputQueue;
        private string _input;

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
                new Thread(() => { _input = _inputQueue.ReadLine();})
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