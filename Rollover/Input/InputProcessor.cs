using System;
using System.Collections.Generic;

namespace Rollover.Input
{
    public class InputProcessor : IInputProcessor
    {
        public List<string> Convert(string input, string state)
        {
            switch(state)
            {
                case "Connected":
                    return new List<string> { input };
                case "WaitingForSymbol":
                    return new List<string> { input };
                default:
                    return new List<string>();

            }
        }
    }
}
