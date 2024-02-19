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
        public SeekingAlphaProvider(IBrowserWrapper browserWrapper) : base(browserWrapper) { }

        public async Task<IEnumerable<string>> PeersComparison(string ticker, int delay)
        {
            string urlTemplate = $"https://seekingalpha.com/symbol/TICKER/peers/comparison";
            //https://seekingalpha.com/symbol/TICKER
            //https://seekingalpha.com/symbol/TICKER/peers/comparison

            var result = new List<string>();
            bool errorOccured = false;
            string symbolLine = "";

            var tickerTrimmed = ticker.Trim();
            await Task.Run(() =>
            {
                TriggerStatus($"Retrieving peers for {tickerTrimmed}");
                var url = urlTemplate.Replace("TICKER", tickerTrimmed);

                // TODO check with LastWebException
                if (!_browserWrapper.Navigate(url))
                {
                    errorOccured = true;
                    result.Add($"PeersComparison\t_browserWrapper.Navigate returned false for url {url}");
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
                    symbolLine = allThElementsValues == null ? "" : $"{string.Join("\t", allThElementsValues)}";
                    result.Add("Symbol\t" + symbolLine);
                    result.Add("Company\t" + symbolLine);

                    var table = trElement?.Parent?.Parent;
                    result.Add(ExtractDataLineTrAnchorSpan(table, 3, "Industry"));
                    result.Add(ExtractDataLineTrDiv(table, 4, "Market Cap"));
                }
            });

            if (!errorOccured)
            {
                result.AddRange(await MoreData(symbolLine, delay));
            }

            return result;
        }

        private async Task<List<string>> MoreData(string symbolLine, int delay)
        {
            var result = new List<string>();
            string urlTemplate = $"https://seekingalpha.com/symbol/TICKER";

            foreach (var ticker in symbolLine.Split("\t"))
            {
                Thread.Sleep(delay);
                await Task.Run(() =>
                {
                    var tickerTrimmed = ticker.Trim();
                    TriggerStatus($"Retrieving more data for {tickerTrimmed}");
                    var url = urlTemplate.Replace("TICKER", tickerTrimmed);

                    _browserWrapper.Navigate(url);
                    if (_browserWrapper.LastWebException != null)
                    {
                        result.Add($"Can not navigate to {url}. {_browserWrapper.LastWebException}");
                    }
                    var sections = _browserWrapper.XDocument.Descendants("section")
                                        .FirstOrDefault(d => d.Attributes("data-test-id").First().Value == "symbol-chart");
                    // TODO
                    if(sections != null) result.Add(sections.Value);

                });


            }
            return result;
        }

        private string ExtractDataLineTrDiv(XElement? table, int rowIndex, string firstColumnData)
        {
            var row = table?.Descendants("tr").ToArray()[rowIndex];
            var rowColumns = row?.Descendants();
            var rowValues = rowColumns?.Select(s => s.Descendants("div").FirstOrDefault()?.Value)
                .Where(d => !string.IsNullOrWhiteSpace(d));
            var line = rowValues == null ? firstColumnData : $"{firstColumnData}\t{string.Join("\t", rowValues)}";
            return line;
        }

        private string ExtractDataLineTrAnchorSpan(XElement? table, int rowIndex, string firstColumnData)
        {
            var row = table?.Descendants("tr").ToArray()[rowIndex];
            var rowColumns = row?.Descendants();
            var anchorElements = rowColumns?.Select(s => s.Descendants("a"));
            var spanElementValues = anchorElements?.Select(s => s.Descendants("span").FirstOrDefault()?.Value)
                .Where(d => !string.IsNullOrWhiteSpace(d));

            var line = spanElementValues == null ? firstColumnData : $"{firstColumnData}\t{string.Join("\t", spanElementValues)}";
            return line;
        }
    }
}