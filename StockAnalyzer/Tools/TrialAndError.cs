using IBApi;
using StockAnalyzer.DataProviders.Types;
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
            Func<int, Contract, int, Task<IntWithError>> maintenanceMarginFromQty,
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
            IntWithError marginOrError = new IntWithError(0,"");

            int trialCount = 1;
            do
            {
                if (marginOrError.Value != 0)
                {
                    qty = (int)Math.Round((double)(targetMargin * (double)qty) / marginOrError.Value, 0);
                }
                marginOrError = await maintenanceMarginFromQty(timeout, contract, qty);
                trialCount++;
            } while (!(lowestMargin <= marginOrError.Value && marginOrError.Value <= highestMargin) && (marginOrError.Value != 0));

            if (marginOrError.Value > 0)
            {
                return new MarginResult(marginOrError.Value, "", qty, --trialCount);
            }
            else
            {
                return new MarginResult(marginOrError.Value, marginOrError.ErrorMessage, qty, --trialCount);
            }
        }
    }
}