using System.Collections.Generic;

namespace Rollover.Ib
{
    public class Result<T>
    {
        public T Value { get; init; }
        public bool Success { get; init; }
        public List<string> Errors { get; init; }

    }
}
