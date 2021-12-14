using System;

namespace Rollover.Input
{
    public class InputLoop : IInputLoop
    {
        private IOutputHelper _outputHelper;

        public InputLoop(IOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
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

        public bool CheckConnectionMessages(IConsoleWrapper consoleWrapper, IInputQueue inputQueue)
        {
            //throw new NotImplementedException();
            return false;
        }
    }
}
