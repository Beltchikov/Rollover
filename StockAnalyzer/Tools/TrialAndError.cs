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
        public static Task<double> PositiveCorrelation(Func<int, Contract, int, Task<double>> maintenanceMarginFromQty, int timeout, Contract contract, int initialQty, double targetMargin, int precision)
        {
            var qty = initialQty;
            var margin = maintenanceMarginFromQty(timeout, contract, qty);

            // TODO

            return margin;
        }
    }
}