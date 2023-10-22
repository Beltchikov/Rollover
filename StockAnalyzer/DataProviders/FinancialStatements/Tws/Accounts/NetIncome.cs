using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StockAnalyzer.DataProviders.FinancialStatements.Tws.Accounts
{
    public class NetIncome
    {
        public static double FromFiscalPeriodElement(IEnumerable<XElement>? fiscalPeriodElement)
        {
            double netIncome;
            var incStatementElement = fiscalPeriodElement?.Where(e => e?.Attribute("Type")?.Value == "INC");
            var lineItemElementsInc = incStatementElement?.Descendants("lineItem");
            var nincLineItemElement = lineItemElementsInc?.Where(e => e?.Attribute("coaCode")?.Value == "NINC").FirstOrDefault();
            var netIncomeAsString = nincLineItemElement?.Value;
            netIncome = Convert.ToDouble(netIncomeAsString, CultureInfo.InvariantCulture);
            return netIncome;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="periodsElement">AnnualPeriods or InterimPeriods</param>
        /// <param name="periodsAgo"></param>
        /// <returns></returns>
        public static double FromPeriodsElement(XElement? periodsElement, int periodsAgo)
        {
            var fiscalPeriodElements = periodsElement?.Descendants("FiscalPeriod");
            var fiscalPeriodElement = fiscalPeriodElements?.Skip(periodsAgo).FirstOrDefault();
            var statementElements = fiscalPeriodElement?.Descendants("Statement");
            var incStatementElement = statementElements?.Where(e => e?.Attribute("Type")?.Value == "INC");
            var lineItemElementsInc = incStatementElement?.Descendants("lineItem");
            var nincLineItemElement = lineItemElementsInc?.Where(e => e?.Attribute("coaCode")?.Value == "NINC").FirstOrDefault();
            var netIncomeAsString = nincLineItemElement?.Value;
            double netIncome = Convert.ToDouble(netIncomeAsString, CultureInfo.InvariantCulture);
            return netIncome;
        }
    }
}
