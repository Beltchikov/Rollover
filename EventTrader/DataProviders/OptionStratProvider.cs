using Dsmn.WebScraping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Dsmn.DataProviders
{
    public class OptionStratProvider : IOptionStratProvider
    {
        readonly IBrowserWrapper _browserWrapper;
        readonly XmlNamespaceManager _xmlNamespaceManager;
        readonly string urlEpsTemplate = $"https://optionstrat.com/build/bull-call-spread/TICKER";

        public event Action<string> Status = null!;

        public async Task<List<string>> HasCriticalWarningsAsync(List<string> tickerList, int delay)
        {
            var result = new List<string>();

            int cnt = 1;
            foreach (string ticker in tickerList)
            {
                await Task.Run(() =>
                {
                    Status.Invoke($"Retrieving critical warnings EPS for {ticker} {cnt++}/{tickerList.Count}");
                    var url = urlEpsTemplate.Replace("TICKER", ticker);

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
