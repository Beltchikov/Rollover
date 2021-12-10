using System;

namespace Rollover
{
    public class InputQueue : IInputQueue
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }
    }
}
