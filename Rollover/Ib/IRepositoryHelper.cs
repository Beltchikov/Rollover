using IBSampleApp.messages;
using System;

namespace Rollover.Ib
{
    public interface IRepositoryHelper
    {
        bool IsInTradingHours(string tradingHoursString, DateTime dateTime);
    }
}