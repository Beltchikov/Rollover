﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace StockAnalyzer.Commands
{
    public class NetIncome
    {
        public static async Task RunAsync(IEdgarConsumer edgarConsumer)
        {
            bool waiting = true;
            Cursor previousCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.Wait;
            Mouse.OverrideCursor = Cursors.Wait;
            edgarConsumer.BackgroundResults = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC4C5C5"));

            _ = Task.Run(() =>
            {
                while (waiting)
                {
                    edgarConsumer.ProgressBarValue += 1;
                    Thread.Sleep(200);
                };
            });

            edgarConsumer.ResultCollectionEdgar = new ObservableCollection<string>(Enumerable.Empty<string>());

            // https://data.sec.gov/api/xbrl/companyconcept/CIK0000200406/us-gaap/PaymentsOfDividends.json
            edgarConsumer.ResultCollectionEdgar = new ObservableCollection<string>(
                                await edgarConsumer.EdgarProvider.BatchProcessing(
                                    edgarConsumer.TickerCollectionEdgar.ToList(),
                                    new string[] { "NetIncomeLoss" },
                                    edgarConsumer.EdgarProvider.CompanyConceptOrError));

            //// https://data.sec.gov/api/xbrl/companyconcept/CIK0000200406/us-gaap/NetIncomeLoss.json
            //ResultCollectionEdgar = new ObservableCollection<string>(
            //    await edgarProvider.BatchProcessing(
            //        TickerCollectionEdgar.ToList(),
            //        new string[] { "NetIncomeLoss" },
            //        edgarProvider.CompanyConceptOrError));


            waiting = false;
            Mouse.OverrideCursor = previousCursor;
            edgarConsumer.BackgroundResults = new SolidColorBrush(Colors.White);
            edgarConsumer.ProgressBarValue = 0;
        }
    }
}