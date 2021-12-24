using Rollover.Ib;
using System;
using System.Diagnostics;

namespace Rollover.Input
{
    public class InputLoop : IInputLoop
    {
        private IInputProcessor _inputProcessor;

        public InputLoop(
            IInputProcessor inputProcessor,
            IConnectedCondition connectedCondition)
        {
            _inputProcessor = inputProcessor;
        }
        
        public void Run(IConsoleWrapper consoleWrapper, IInputQueue inputQueue, IRepository requestSender)
        {
            while (true)
            {
                var input = inputQueue.Dequeue();
                if (input == null)
                {
                    continue;
                }

                var outputList = _inputProcessor.Convert(input, requestSender);
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
