using System;

namespace Rollover.Input
{
    public class InputLoop : IInputLoop
    {
        public void Run(IConsoleWrapper consoleWrapper, IInputQueue inputQueue)
        {
            throw new NotImplementedException();

            //while (true)
            //{
            //    var input = _inputQueue.Dequeue();
            //    if (input == null)
            //    {
            //        continue;
            //    }

            //    _consoleWrapper.WriteLine(input);

            //    if (input.Equals("q", StringComparison.InvariantCultureIgnoreCase))
            //    {
            //        break;
            //    }
            //}
        }
    }
}
