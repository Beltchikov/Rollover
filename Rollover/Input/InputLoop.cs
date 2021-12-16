using Rollover.Ib;
using System;
using System.Diagnostics;

namespace Rollover.Input
{
    public class InputLoop : IInputLoop
    {
        private IOutputHelper _outputHelper;
        private IConnectedCondition _connectedCondition;

        public InputLoop(IOutputHelper outputHelper, IConnectedCondition connectedCondition)
        {
            _outputHelper = outputHelper;
            _connectedCondition = connectedCondition;
        }

        public void Run(IConsoleWrapper consoleWrapper, IInputQueue inputQueue)
        {
            while (true)
            {
                var input = inputQueue.Dequeue();
                if (input == null)
                {
                    continue;
                }

                var outputList = _outputHelper.Convert(input);
                outputList.ForEach(o => consoleWrapper.WriteLine(o));

                if (input.Equals("q", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }
            }

            consoleWrapper.WriteLine("Goodbye!");
        }

        public bool CheckConnectionMessages(IConsoleWrapper consoleWrapper, IInputQueue inputQueue, int timeout)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            while (stopWatch.Elapsed.TotalMilliseconds < timeout)
            {
                var input = inputQueue.Dequeue();
                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                var outputList = _outputHelper.Convert(input);
                outputList.ForEach(o => consoleWrapper.WriteLine(o));

                _connectedCondition.AddInput(input);
                if (_connectedCondition.IsConnected())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
