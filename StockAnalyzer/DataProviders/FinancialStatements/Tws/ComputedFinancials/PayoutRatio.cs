using System;

namespace StockAnalyzer.DataProviders.FinancialStatements.Tws.ComputedFinancials
{
    public class PayoutRatio
    {
        public static double FromNetIncomeAndDividends(double netIncome, double divPaid)
        {
            netIncome = netIncome == 0 ? 1 : netIncome;
            double payoutRatio = (divPaid / netIncome) * 100;
            payoutRatio = Math.Round(payoutRatio, 1);
            return payoutRatio;
        }
    }
}
