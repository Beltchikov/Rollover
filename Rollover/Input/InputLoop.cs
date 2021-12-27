using Rollover.Ib;
using System;

namespace Rollover.Input
{
    public class InputLoop : IInputLoop
    {
        private IInputProcessor _inputProcessor;

        public InputLoop(IInputProcessor inputProcessor)
        {
            _inputProcessor = inputProcessor;
        }
        
        public void Run(IConsoleWrapper consoleWrapper, IInputQueue inputQueue, IIbClientQueue ibClientQueue)
        {
            while (true)
            {
                var input = inputQueue.Dequeue();
                var message = ibClientQueue.Dequeue();
                if (input == null && message == null)
                {
                    continue;
                }

                var outputList = _inputProcessor.Convert(input);
                var messageList = _inputProcessor.ConvertMessage(message);
                outputList.AddRange(messageList);

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
