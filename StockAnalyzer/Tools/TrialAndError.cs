using IBApi;
using System;
using System.Threading.Tasks;

namespace StockAnalyzer.Tools
{
    public class TrialAndError
    {
        /// <summary>
        /// PositiveCorrelation
        /// </summary>
        /// <param name="maintenanceMarginFromQty"></param>
        /// <param name="timeout"></param>
        /// <param name="contract"></param>
        /// <param name="initialQty"></param>
        /// <param name="targetMargin"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        /// <param name="precision">In percent/// </param>
        public static async Task<double> PositiveCorrelation(
            Func<int, Contract, int, Task<double>> maintenanceMarginFromQty,
            int timeout,
            Contract contract,
            int initialQty,
            double targetMargin,
            int precision)
        {
            var qty = initialQty;
            
            double lowestMarginKoef = (100d - (double)precision) / 100d;
            double highestMarginKoef = (100d + (double)precision) / 100d;
            int lowestMargin = (int)Math.Round(targetMargin * lowestMarginKoef, 0);
            int highestMargin = (int)Math.Round(targetMargin * highestMarginKoef, 0);
            double margin = 0;

            do
            {
                if (margin != 0)
                {
                    qty = (int)Math.Round((targetMargin * qty) / margin, 0);
                }
                margin = await maintenanceMarginFromQty(timeout, contract, qty);
            } while (!(lowestMargin <= margin && margin <= highestMargin));


            // TODO

            return margin;
        }
    }
}