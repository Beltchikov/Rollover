using Rollover.Ib;
using Rollover.Tracking;
using System;

namespace Rollover.Input
{
    public class InputLoop : IInputLoop
    {
        private readonly IInputProcessor _inputProcessor;
        private readonly IMessageProcessor _messageProcessor;
        private readonly IRepository _repository;
        private readonly IOrderManager _orderManager;

        public InputLoop(
            IInputProcessor inputProcessor,
            IMessageProcessor messageProcessor,
            IRepository repository,
            IOrderManager orderManager)
        {
            _inputProcessor = inputProcessor;
            _messageProcessor = messageProcessor;
            _repository = repository;
            _orderManager = orderManager;
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
