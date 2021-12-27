using System.Collections.Generic;

namespace Rollover.Input
{
    public interface IInputProcessor
    {
        List<string> Convert(string input);
        List<string> ConvertMessage(object v);
    }
}