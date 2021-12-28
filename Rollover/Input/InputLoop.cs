using Rollover.Ib;
using System;

namespace Rollover.Input
{
    public class InputLoop : IInputLoop
    {
        private IInputProcessor _inputProcessor;
        private IMessageProcessor _messageProcessor;

        public InputLoop(IInputProcessor inputProcessor, IMessageProcessor messageProcessor)
        {
            _inputProcessor = inputProcessor;
            _messageProcessor = messageProcessor;
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
                var messageList = _messageProcessor.ConvertMessage(message);
                outputList.AddRange(messageList);

                outputList.ForEach(consoleWrapper.WriteLine);

                if (input != null && input.Equals("q", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }
            }

            consoleWrapper.WriteLine("Goodbye!");
        }
    }
}
