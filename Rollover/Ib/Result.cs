using System.Collections.Generic;

namespace Rollover.Ib
{
    public record Result<T>
    {
        public Result()
        {
            Errors = new List<string>();
        }

        public T Value { get; init; }
        public bool Success { get; init; }
        public List<string> Errors { get; init; }
    }
}
