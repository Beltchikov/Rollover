using System.Diagnostics;

namespace PortfolioTrader.Algos
{
    public static class LimitPrice
    {
        public static double PercentageOfPriceOrFixed(bool isLong, double price)
        {
            double buffer = 0;
            if (price < 100)
            {
                return price; // No buffer
            }
            if (price >= 100 && price <= 300)
            {
                double percentageBuffer = 0.01;
                buffer = Math.Ceiling(price * percentageBuffer);
                buffer = Math.Round(buffer / 100, 2);
            }
            else
            {
                buffer = 0.03; // constant buffer, always 3 ct.
            }

            return isLong
                    ? Math.Round(price + buffer,2)
                    : Math.Round(price - buffer,2);
        }

        internal static double Always5Cents(bool isLong, double price)
        {
            double buffer = 0.05;
            return isLong
                    ? Math.Round(price + buffer, 2)
                    : Math.Round(price - buffer, 2);
        }
    }
}
