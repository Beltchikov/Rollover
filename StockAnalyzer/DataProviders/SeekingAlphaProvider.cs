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
        public SeekingAlphaProvider(IBrowserWrapper browserWrapper) : base(browserWrapper) { }

        public async Task<IEnumerable<string>> PeersComparison(string ticker, int delay)
        {
            var result = new List<string>();

            var tickerTrimmed = ticker.Trim();
            await Task.Run(() =>
            {
                TriggerStatus($"Retrieving peers for {tickerTrimmed}");
                var url = urlTemplate.Replace("TICKER", tickerTrimmed);

                if (!_browserWrapper.Navigate(url))
                {
                    //AddDataToResultList(result, tickerTrimmed, null);
                }
                else
                {
                    var anchorWithTicker = _browserWrapper.XDocument.Descendants("a")
                        .FirstOrDefault(d => d.Attributes("href").First().Value.Contains(ticker) && d.Value == ticker);
                    var trElement = anchorWithTicker?.Parent?.Parent;
                    var allThElementsValues = trElement?.Elements()
                        .Where(e => !string.IsNullOrWhiteSpace(e.Value.Trim()))
                        .Select(e => e.Value.Trim())
                        .ToArray();
                    var symbolLine = allThElementsValues == null ? "Symbol" : $"Symbol\t{string.Join("\t", allThElementsValues)}";
                    result.Add(symbolLine);
                    
                    //var table = trElement?.Parent?.Parent;
                   
                }

                Thread.Sleep(delay);
            });


            return result;
        }
    }
}