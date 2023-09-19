using Dsmn.WebScraping;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dsmn.DataProviders
{
    public class OptionStratProvider : ProviderBase, IOptionStratProvider
    {
        readonly string urlWarningTemplate = $"https://optionstrat.com/build/bull-call-spread/TICKER";

        public OptionStratProvider(IBrowserWrapper browserWrapper) : base(browserWrapper) { }

        public async Task<List<string>> HasCriticalWarningsAsync(List<string> tickerList, int delay)
        {
            var result = new List<string>();

            int cnt = 1;
            foreach (string ticker in tickerList)
            {
                await Task.Run(() =>
                {
                    TriggerStatus($"Retrieving critical warnings EPS for {ticker} {cnt++}/{tickerList.Count}");
                    var url = urlWarningTemplate.Replace("TICKER", ticker);

                    //if (!_browserWrapper.Navigate(url))
                    //{
                    //    AddDataToResultList(result, ticker, null);
                    //}
                    //else
                    //{
                    //    var text = _browserWrapper.CurrentHtml;
                    //    var lines = text.Split("\r\n").ToList();
                    //    var line = lines.FirstOrDefault(l => l.Contains("Earnings History"));
                    //    var line2 = line?.Substring(line.IndexOf("EPS Actual"));
                    //    var line3 = line2?.Substring(0, line2.IndexOf("</tr>"));
                    //    var line4 = line3?.Substring(line3.LastIndexOf("Ta(end)"));

                    //    AddDataToResultList(result, ticker, line4);
                    //}

                    Thread.Sleep(delay);
                });
            }

            return result;
        }
    }
}
