using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StockAnalyzer.WebScraping;

namespace StockAnalyzer.DataProviders
{
    public class SeekingAlphaProvider : BrowserProviderBase, ISeekingAlphaProvider
    {
        readonly string urlTemplate = $"https://seekingalpha.com/symbol/MSFT/peers/comparison?compare=MSFT,ORCL,NOW,PANW,CRWD,FTNT";
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
                        var text = _browserWrapper.CurrentHtml;
                        var lines = text.Split("\r\n").ToList();
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