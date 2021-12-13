using System;

namespace Rollover.Input
{
    public class InputLoop : IInputLoop
    {
        public void Run(IConsoleWrapper consoleWrapper, IInputQueue inputQueue)
        {
            while (true)
            {
                var input = inputQueue.Dequeue();
                if (input == null)
                {
                    continue;
                }

                consoleWrapper.WriteLine(input);

                if (input.Equals("q", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }
            }
        }
    }
}
