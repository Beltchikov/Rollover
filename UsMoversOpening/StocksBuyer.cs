using System;
using System.Linq;

namespace UsMoversOpening
{
    public class StocksBuyer : IStocksBuyer
    {
        public bool SendOrders()
        {
            throw new NotImplementedException();
        }

        public bool Triggered(string timeToBuyString)
        {
            var timeToBuyArray = timeToBuyString.Split(":");
            var hourToBuy = Convert.ToInt32(timeToBuyArray.First());
            var minuteToBuy = Convert.ToInt32(timeToBuyArray.Last());

            var timeToBuy = new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                hourToBuy,
                minuteToBuy,
                0);

            return DateTime.Now > timeToBuy;
        }
    }
}
