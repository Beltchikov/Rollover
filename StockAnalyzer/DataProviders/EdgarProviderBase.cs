using System;
using System.Collections.Generic;
using System.Linq;

namespace StockAnalyzer.DataProviders
{
    public class EdgarProviderBase
    {
        protected IEnumerable<string> TableForMultipleSymbols(List<string> symbols, List<List<string>> symbolDataList)
        {
            List<string> uniqueDatesStringsListSorted = symbolDataList
                            .Select(d => d[0])
                            .SelectMany(u => u.Split("\t"))
                            .Distinct()
                            .OrderBy(s => s)
                            .ToList();
            List<string> resultList = new() { "Symbol\t" + uniqueDatesStringsListSorted.Aggregate((r, n) => r + "\t" + n) };
           
            string dataRow = "";
            for (int i = 0; i < symbols.Count; i++)
            {
                string symbol = symbols[i];
                dataRow = symbol;
                List<List<string>> symbolData = symbolDataList[i].Select(l => l.Split("\t").ToList()).ToList();

                foreach (var date in uniqueDatesStringsListSorted)
                {
                    int ii = symbolData.First().IndexOf(date);
                    string data = ii >= 0 ? symbolData.Last()[ii] : null!;
                    dataRow += ("\t" + data);
                }

                resultList.Add(dataRow);
            }

            return resultList;
        }
    }
}