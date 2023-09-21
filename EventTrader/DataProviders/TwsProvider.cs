using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Dsmn.DataProviders
{
    public class TwsProvider : ITwsProvider
    {
        public async Task<List<string>> BidAskSpread(List<string> tickerList, int timeout)
        {
            //var result = new List<string>();

            //int cnt = 1;
            //foreach (string ticker in tickerList)
            //{
            //    await Task.Run(() =>
            //    {
            //        TriggerStatus($"Retrieving last EPS for {ticker} {cnt++}/{tickerList.Count}");
            //        var url = urlEpsTemplate.Replace("TICKER", ticker);

            //        if (!_browserWrapper.Navigate(url))
            //        {
            //            AddDataToResultList(result, ticker, null);
            //        }
            //        else
            //        {
            //            var text = _browserWrapper.CurrentHtml;
            //            var lines = text.Split("\r\n").ToList();
            //            var line = lines.FirstOrDefault(l => l.Contains("Earnings History"));
            //            var line2 = line?.Substring(line.IndexOf("EPS Actual"));
            //            var line3 = line2?.Substring(0, line2.IndexOf("</tr>"));
            //            var line4 = line3?.Substring(line3.LastIndexOf("Ta(end)"));

            //            AddDataToResultList(result, ticker, line4);
            //        }

            //        Thread.Sleep(delay);
            //    });
            //}

            //return result;



            await Task.Run(() =>
            {
                MessageBox.Show("BidAskSpread");

            });

            //todo
            return new List<string>();
        }
    }
}
