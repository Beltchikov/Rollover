using System;

namespace SsbHedger.Utilities
{
    public interface ILastTradeDateConverter
    {
        string FromDateTime(DateTime dateTime);
    }
}