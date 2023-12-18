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
        public static async Task<MarginResult> PositiveCorrelation(
            Func<int, Contract, int, Task<int>> maintenanceMarginFromQty,
            int timeout,
            Contract contract,
            int initialQty,
            int targetMargin,
            int precision)
        {
            var qty = initialQty;
            
            double lowestMarginKoef = (100d - (double)precision) / 100d;
            double highestMarginKoef = (100d + (double)precision) / 100d;
            int lowestMargin = (int)Math.Round(targetMargin * lowestMarginKoef, 0);
            int highestMargin = (int)Math.Round(targetMargin * highestMarginKoef, 0);
            int margin = 0;

            int trialCount = 1;
            do
            {
                if (margin != 0)
                {
                    qty = (int)Math.Round((double)(targetMargin * (double)qty) / margin, 0);
                }
                margin = await maintenanceMarginFromQty(timeout, contract, qty);
                trialCount++;
            } while (!(lowestMargin <= margin && margin <= highestMargin) && (margin != 0));

            return new MarginResult(margin, qty, --trialCount);
        }
    }
}