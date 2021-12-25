using Rollover.Ib;
using System;

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
        
        public void Run(IConsoleWrapper consoleWrapper, IInputQueue inputQueue)
        {
            while (true)
            {
                var input = inputQueue.Dequeue();
                if (input == null)
                {
                    continue;
                }

                var outputList = _inputProcessor.Convert(input);
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
