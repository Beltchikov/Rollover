using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace StockAnalyzer.DataProviders.FinancialStatements.Tws.Accounts
{
    public class TotalSharesOutstanding
    {
        /// <summary>
        /// </summary>
        /// <param name="periodsElement">AnnualPeriods or InterimPeriods</param>
        /// <param name="coaCode">Common shares: QTCO, Preferred shares: QTPO</param>
        /// <param name="periodsAgo"></param>
        /// <returns></returns>
        public static double FromPeriodsElement(XElement? periodsElement, string coaCode, int periodsAgo)
        {
            var fiscalPeriodElements = periodsElement?.Descendants("FiscalPeriod");
            var fiscalPeriodElement = fiscalPeriodElements?.Skip(periodsAgo).FirstOrDefault();
            var statementElements = fiscalPeriodElement?.Descendants("Statement");
            var balStatementElement = statementElements?.Where(e => e?.Attribute("Type")?.Value == "BAL");
            var lineItemElements = balStatementElement?.Descendants("lineItem");
            var sharesOutLineItemElement = lineItemElements?.Where(e => e?.Attribute("coaCode")?.Value == coaCode).FirstOrDefault();
            var shareOutAsString = sharesOutLineItemElement?.Value;
            var sharesOut = Convert.ToDouble(shareOutAsString, CultureInfo.InvariantCulture);
            return sharesOut;
        }
    }
}
