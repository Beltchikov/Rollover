using System;

namespace Rollover.Ib
{
    public class NoMarketDataException : Exception
    {
        public NoMarketDataException(string message) : base(message)
        {
        }
    }
}