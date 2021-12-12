using System;

namespace Rollover.Ib
{
    public interface IbClientWrapper
    {
        public event Action<int, int, string, Exception> Error;
    }
}