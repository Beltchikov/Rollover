using System.Collections.Generic;

namespace Rollover.Ib
{
    public interface IMessageProcessor
    {
        List<string> ConvertMessage(object obj);
    }
}