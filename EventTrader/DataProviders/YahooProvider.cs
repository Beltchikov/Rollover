using Dsmn.WebScraping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Dsmn.DataProviders
{
    public class YahooProvider : IYahooProvider
    {
        readonly IBrowserWrapper _browserWrapper;
        readonly XmlNamespaceManager _xmlNamespaceManager;
        readonly string urlEpsTemplate = $"https://finance.yahoo.com/quote/TICKER/analysis?p=TICKER";
        
        public event Action<string> Status = null!;

        public YahooProvider(IBrowserWrapper browserWrapper)
        {
            _browserWrapper = browserWrapper;
            _xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
            _xmlNamespaceManager.AddNamespace("empty", "http://bel.com/2023/bel-schema");
        }

        public async Task<List<string>> LastEpsAsync(List<string> tickerList, int delay)
        {
            var result = new List<string>();

            int cnt = 1;
            foreach (string ticker in tickerList)
            {
                await Task.Run(() =>
                {
                    Status.Invoke($"Retrieving last EPS for {ticker} {cnt++}/{tickerList.Count}");
                    var url = urlEpsTemplate.Replace("TICKER", ticker);

                    if (!_browserWrapper.Navigate(url))
                    {
                        AddDataToResultList(result, ticker, null);
                    }
                    else
                    {
                        var text = _browserWrapper.CurrentHtml;
                        var lines = text.Split("\r\n").ToList();
                        var line = lines.FirstOrDefault(l => l.Contains("Earnings History"));
                        var line2 = line?.Substring(line.IndexOf("EPS Actual"));
                        var line3 = line2?.Substring(0, line2.IndexOf("</tr>"));
                        var line4 = line3?.Substring(line3.LastIndexOf("Ta(end)"));

                        AddDataToResultList(result, ticker, line4);
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
                await Task.Run(() =>
                {
                    Status.Invoke($"Retrieving expected EPS for {ticker} {cnt++}/{tickerList.Count}");
                    var url = urlEpsTemplate.Replace("TICKER", ticker);

                    if (!_browserWrapper.Navigate(url))
                    {
                        AddDataToResultList(result, ticker, null);
                    }
                    else
                    {
                        var text = _browserWrapper.CurrentHtml;
                        var lines = text.Split("\r\n").ToList();
                        var line = lines.FirstOrDefault(l => l.Contains("Avg. Estimate"));
                        var line2 = line?.Substring(line.IndexOf("<tbody>"), line.IndexOf("</tbody>") - line.IndexOf("<tbody>"));
                        var line3 = line2?.Substring(line2.IndexOf("Avg. Estimate"));
                        var line4 = line3?.Substring(line3.IndexOf("<td class=\"Ta(end)\">"));

                        AddDataToResultList(result, ticker, line4);
                    }

                    Thread.Sleep(delay);
                });
                
            }

            return result;
        }

        private static string RegexMatch(string text, string regexPattern, int index)
        {
            var rx = new Regex(regexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matches = rx.Matches(text).ToList();
            return matches.Count <= 0 
                ? string.Empty 
                : matches[index].ToString();
        }

        private static void AddDataToResultList(List<string> result, string ticker, string? text)
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
