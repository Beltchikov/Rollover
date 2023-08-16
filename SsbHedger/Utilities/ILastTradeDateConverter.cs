using System;

namespace SsbHedger.Utilities
{
    public interface ILastTradeDateConverter
    {
        DateTime DateTimeFromDte(int dte);
        string FromDateTime(DateTime dateTime);
    }
}