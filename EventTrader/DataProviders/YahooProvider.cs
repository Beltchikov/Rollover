using Eomn.WebScraping;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Eomn.DataProviders
{
    public class YahooProvider : BrowserProviderBase, IYahooProvider
    {
        readonly string urlEpsTemplate = $"https://finance.yahoo.com/quote/TICKER/analysis?p=TICKER";
        public YahooProvider(IBrowserWrapper browserWrapper) : base(browserWrapper) {}

        public async Task<List<string>> LastEpsAsync(List<string> tickerList, int delay)
        {
            var result = new List<string>();

            int cnt = 1;
            foreach (string ticker in tickerList)
            {
                var tickerTrimmed = ticker.Trim();  
                await Task.Run(() =>
                {
                    TriggerStatus($"Retrieving last EPS for {tickerTrimmed} {cnt++}/{tickerList.Count}");
                    var url = urlEpsTemplate.Replace("TICKER", tickerTrimmed);

                    if (!_browserWrapper.Navigate(url))
                    {
                        AddDataToResultList(result, tickerTrimmed, null);
                    }
                    else
                    {
                        var text = _browserWrapper.CurrentHtml;
                        var lines = text.Split("\r\n").ToList();
                        var line = lines.FirstOrDefault(l => l.Contains("Earnings History"));
                        var line2 = line?.Substring(line.IndexOf("EPS Actual"));
                        var line3 = line2?.Substring(0, line2.IndexOf("</tr>"));
                        var line4 = line3?.Substring(line3.LastIndexOf("Ta(end)"));

                        AddDataToResultList(result, tickerTrimmed, line4);
                    }

                    Thread.Sleep(delay);
                });
            }

            return result;
        }

        public async Task<List<string>> ExpectedEpsAsync(List<string> tickerList, int delay)
        {
            var result = new List<string>();

            int cnt = 1;
            foreach (string ticker in tickerList)
            {
                var tickerTrimmed = ticker.Trim();

                await Task.Run(() =>
                {
                    TriggerStatus($"Retrieving expected EPS for {tickerTrimmed} {cnt++}/{tickerList.Count}");
                    var url = urlEpsTemplate.Replace("TICKER", tickerTrimmed);

                    if (!_browserWrapper.Navigate(url))
                    {
                        AddDataToResultList(result, tickerTrimmed, null);
                    }
                    else
                    {
                        var text = _browserWrapper.CurrentHtml;
                        var lines = text.Split("\r\n").ToList();
                        var line = lines.FirstOrDefault(l => l.Contains("Avg. Estimate"));
                        var line2 = line?.Substring(line.IndexOf("<tbody>"), line.IndexOf("</tbody>") - line.IndexOf("<tbody>"));
                        var line3 = line2?.Substring(line2.IndexOf("Avg. Estimate"));
                        var line4 = line3?.Substring(line3.IndexOf("<td class=\"Ta(end)\">"));

                        AddDataToResultList(result, tickerTrimmed, line4);
                    }

                    Thread.Sleep(delay);
                });
                
            }

            return result;
        }

        private void AddDataToResultList(List<string> result, string ticker, string? text)
        {
            if (text != null)
            {
                var data = RegexMatch(text, @"-?\d[\.\d]+", 0);
                result.Add($"{ticker}\t{data}");
            }
            else
            {
                result.Add($"{ticker}\t ");
            }
        }
    }
}
