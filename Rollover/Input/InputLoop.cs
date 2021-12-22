using Rollover.Ib;
using Rollover.Tracking;
using System;
using System.Diagnostics;

namespace Rollover.Input
{
    public class InputLoop : IInputLoop
    {
        private IInputProcessor _inputProcessor;
        private IConnectedCondition _connectedCondition;
        private IPortfolio _portfolio;
        private ITrackedSymbols _trackedSymbols;
        IInputQueue _inputQueue;

        public InputLoop(
            IInputProcessor inputProcessor,
            IConnectedCondition connectedCondition,
            IPortfolio portfolio,
            ITrackedSymbols trackedSymbols)
        {
            _inputProcessor = inputProcessor;
            _connectedCondition = connectedCondition;
            _portfolio = portfolio;
            _trackedSymbols = trackedSymbols;
        }

        public bool CheckConnectionMessages(IConsoleWrapper consoleWrapper, IInputQueue inputQueue, int timeout)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
           
            while (stopWatch.Elapsed.TotalMilliseconds < timeout)
            {
                var input = inputQueue.Dequeue();
                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                consoleWrapper.WriteLine(input);

                _connectedCondition.AddInput(input);
                if (_connectedCondition.IsConnected())
                {
                    return true;
                }
            }

            return false;
        }

        public void Run(IConsoleWrapper consoleWrapper, IInputQueue inputQueue)
        {
            _inputQueue = inputQueue;
           
            while (true)
            {
                var input = inputQueue.Dequeue();
                if (input == null)
                {
                    continue;
                }

                var outputList = _inputProcessor.Convert(input, _portfolio, _trackedSymbols);
                outputList.ForEach(consoleWrapper.WriteLine);

                if (input.Equals("q", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }
            }

            consoleWrapper.WriteLine("Goodbye!");
        }
    }
}
