using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace StockAnalyzer.DataProviders.FinancialStatements.Tws.Accounts
{
    public class DividendsPaid
    {
        public static double FromFiscalPeriodElement(IEnumerable<XElement>? fiscalPeriodElement)
        {
            var casStatementElement = fiscalPeriodElement?.Where(e => e?.Attribute("Type")?.Value == "CAS");
            var lineItemElementsCas = casStatementElement?.Descendants("lineItem");
            var fcdpLineItemElement = lineItemElementsCas?.Where(e => e?.Attribute("coaCode")?.Value == "FCDP").FirstOrDefault();
            var divPaidNegative = fcdpLineItemElement?.Value;
            double divPaid = Convert.ToDouble(divPaidNegative, CultureInfo.InvariantCulture) * -1;
            return divPaid;
        }

        /// <summary>
        /// ExtractDividendsPaid
        /// </summary>
        /// <param name="periodsElement">AnnualPeriods or InterimPeriods</param>
        /// <param name="periodsAgo"></param>
        /// <returns></returns>
        public static double FromPeriodsElement(XElement? periodsElement, int periodsAgo)
        {
            var fiscalPeriodElements = periodsElement?.Descendants("FiscalPeriod");
            var fiscalPeriodElement = fiscalPeriodElements?.Skip(periodsAgo).FirstOrDefault();
            var statementElements = fiscalPeriodElement?.Descendants("Statement");
            var casStatementElement = statementElements?.Where(e => e?.Attribute("Type")?.Value == "CAS");
            var lineItemElementsCas = casStatementElement?.Descendants("lineItem");
            var fcdpLineItemElement = lineItemElementsCas?.Where(e => e?.Attribute("coaCode")?.Value == "FCDP").FirstOrDefault();
            var divPaidNegative = fcdpLineItemElement?.Value;
            double divPaid = Convert.ToDouble(divPaidNegative, CultureInfo.InvariantCulture) * -1;
            return divPaid;
        }
    }
}
