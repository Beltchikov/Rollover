using System.Collections.Generic;

namespace Rollover.Input
{
    public interface IOutputHelper
    {
        List<string> Convert(string v);
    }
}