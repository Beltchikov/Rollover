using System;

namespace Rollover.Ib
{
    public class MultipleContractDetailsMessage : Exception
    {
        public MultipleContractDetailsMessage(string message) : base(message)
        {
        }
    }
}