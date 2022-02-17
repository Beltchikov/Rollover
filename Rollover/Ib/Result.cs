using System.Collections.Generic;

namespace Rollover.Ib
{
    public record Result<T>(bool Success, T Value, IEnumerable<string> Messages);
}
