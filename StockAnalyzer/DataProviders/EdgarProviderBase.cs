using System.Collections.Generic;

namespace StockAnalyzer.DataProviders
{
    public class EdgarProviderBase 
    {
        protected IEnumerable<string> TableForMultipleSymbols(List<string> symbols, List<List<string>> symbolDataList)
        {
            List<string> resultList = new();
            return resultList;

            //List<string> dateAndEquityLines = (await StockholdersEquity(symbol)).ToList();
            //allDateLines.Add(dateAndEquityLines.First());
            //allEquityLines.Add(dateAndEquityLines.Last());
        }
    }
}