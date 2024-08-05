using System;
using System.Collections.Generic;
using System.Linq;

namespace StockAnalyzer.DataProviders
{
    public class EdgarProviderBase 
    {
        protected IEnumerable<string> TableForMultipleSymbols(List<string> symbols, List<List<string>> symbolDataList)
        {
            List<string> resultList = new();


            //List<string> datesStringsList = symbolDataList.Select(d => d[0]).ToList();
            //List<string> datesStringsListSplitted = datesStringsList.SelectMany(u => u.Split("\t")).ToList();
            //List<string> uniqueDatesStringsList = datesStringsListSplitted.Distinct().ToList();
            //List<string> uniqueDatesStringsListSorted = uniqueDatesStringsList.OrderBy(s=>s).ToList();
            
            List<string> uniqueDatesStringsListSorted = symbolDataList
                .Select(d => d[0])
                .SelectMany(u => u.Split("\t"))
                .Distinct()
                .OrderBy(s => s)
                .ToList();
            string header = "Symbol\t" + uniqueDatesStringsListSorted.Aggregate((r, n) => r + "\t" + n);
            resultList.Add(header);




            return resultList;

            //List<string> dateAndEquityLines = (await StockholdersEquity(symbol)).ToList();
            //allDateLines.Add(dateAndEquityLines.First());
            //allEquityLines.Add(dateAndEquityLines.Last());
        }
    }
}