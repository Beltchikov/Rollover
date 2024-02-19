using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using StockAnalyzer.WebScraping;

namespace StockAnalyzer.DataProviders
{
    public class SeekingAlphaProvider : BrowserProviderBase, ISeekingAlphaProvider
    {
        readonly string urlTemplate = $"https://seekingalpha.com/symbol/TICKER/peers/comparison";
        //https://seekingalpha.com/symbol/MSFT/peers/comparison?compare=MSFT,ORCL,NOW,PANW,CRWD,FTNT
        //https://seekingalpha.com/symbol/TICKER/peers/comparison
        public SeekingAlphaProvider(IBrowserWrapper browserWrapper) : base(browserWrapper) {}

        public async Task<IEnumerable<string>> PeersComparison(List<string> tickerList, int delay)
        {
            var result = new List<string>();

            int cnt = 1;
            foreach (string ticker in tickerList)
            {
                var tickerTrimmed = ticker.Trim();  
                await Task.Run(() =>
                {
                    TriggerStatus($"Retrieving last EPS for {tickerTrimmed} {cnt++}/{tickerList.Count}");
                    var url = urlTemplate.Replace("TICKER", tickerTrimmed);

                    if (!_browserWrapper.Navigate(url))
                    {
                        //AddDataToResultList(result, tickerTrimmed, null);
                    }
                    else
                    {
                        var xDocument = _browserWrapper.XDocument;
                        var anchorWithTicker = xDocument.Descendants("a")
                            .FirstOrDefault(d=>d.Attributes("href").First().Value.Contains(ticker) && d.Value == ticker);
                        var trElement = anchorWithTicker?.Parent?.Parent;
                        var allThElementsValues = trElement?.Elements()
                            .Where(e => !string.IsNullOrWhiteSpace(e.Value.Trim()))
                            .Select(e => e.Value.Trim())
                            .ToArray();
                        var allValuesString = allThElementsValues == null ? "": string.Join("\t", allThElementsValues);   
                        //var table = trElement?.Parent?.Parent;
                        if(allThElementsValues != null)
                        {
                            result.Add(allValuesString);
                        }
                        
                        // var text = _browserWrapper.CurrentHtml;
                        // var lines = text.Split("\r\n").ToList();
                        // var line = lines.FirstOrDefault(l => l.Contains("Earnings History"));
                        // var line2 = line?.Substring(line.IndexOf("EPS Actual"));
                        // var line3 = line2?.Substring(0, line2.IndexOf("</tr>"));
                        // var line4 = line3?.Substring(line3.LastIndexOf("Ta(end)"));

                        // AddDataToResultList(result, tickerTrimmed, line4);
                    }

                    Thread.Sleep(delay);
                });
            }

            return result;
        }
    }
}