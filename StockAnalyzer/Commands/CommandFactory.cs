using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static StockAnalyzer.Commands.EdgarBatchProcessor;

namespace StockAnalyzer.Commands
{
    public class CommandFactory
    {
        const int PROGRESS_BAR_DELAY = 400;

        public static ICommand? CreateBatchProcessingComputed(string commandName, IEdgarConsumer edgarConsumer)
        {
            if (commandName == "FreeCashFlowCommand")
            {
                
            }

            return new RelayCommand(
               async () =>
               {
                   Ui ui = new();
                   ui.Disable(edgarConsumer, PROGRESS_BAR_DELAY);

                   MessageBox.Show("FreeCashFlow");

                   //await RunBatchProcessingAsync(
                   //    edgarConsumer,
                   //    simpleAccountingAttribute,
                   //    edgarConsumer.EdgarProvider.BatchProcessing);

                   ui.Enable(edgarConsumer);
               });
        }

        public static ICommand? CreateBatchProcessingSimple(string commandName, IEdgarConsumer edgarConsumer)
        {
            SimpleAccountingAttribute simpleAccountingAttribute = commandName switch
            {
                "RevenueCommand" => new SimpleAccountingAttribute(
                    "Revenue",
                    new List<string> {
                        "RevenueFromContractWithCustomerExcludingAssessedTax",
                        "SalesRevenueNet",
                        "Revenues",
                        "OperatingRevenue"
                    }),
                "CogsCommand" => new SimpleAccountingAttribute(
                    "CostOfRevenue",
                    new List<string>
                    {
                        "CostOfGoodsAndServicesSold", // TODO check
                        "CostOfRevenue"
                    }),
                "OperatingCostCommand" => new SimpleAccountingAttribute(
                    "OperatingExpenses",
                    new List<string>
                    {
                        "OperatingExpenses",
                        "CostOfGoodsAndServicesSold" // TODO check
                    }),
                "FinancingCostCommand" => new SimpleAccountingAttribute(
                    "InterestExpense",
                    new List<string>
                    {
                        "InterestIncomeExpenseNonoperatingNet",
                        "InterestExpense",
                        "FinanceCost",
                        "BorrowingCost",
                        "DebtInterestExpense"
                    }),
                "TaxCommand" => new SimpleAccountingAttribute(
                    "Tax",
                    new List<string>
                    {
                        "IncomeTaxExpenseBenefit"
                    }),
                // https://data.sec.gov/api/xbrl/companyconcept/CIK0000200406/us-gaap/NetIncomeLoss.json
                "NetIncomeCommand" => new SimpleAccountingAttribute(
                    "NetIncome",
                    new List<string>
                    {
                        "NetIncomeLoss",
                        "ProfitLoss"
                    }),
                // https://data.sec.gov/api/xbrl/companyconcept/CIK0000PROGRESS_BAR_DELAY406/us-gaap/StockholdersEquity.json
                "EquityCommand" => new SimpleAccountingAttribute(
                    "Equity",
                    new List<string>
                    {
                        "StockholdersEquity",
                        "Equity",
                        "EquityAttributableToOwnersOfParent",
                        "EquityAttributableToParent",
                        "TotalEquity",
                        "EquityAttributableToNoncontrollingInterest"
                    }),
                // https://data.sec.gov/api/xbrl/companyconcept/CIK0000PROGRESS_BAR_DELAY406/us-gaap/LongTermDebt.json
                "LongTermDebtCommand" => new SimpleAccountingAttribute(
                    "LongTermDebt",
                    new List<string>
                    {
                        "LongTermDebt",
                        "NoncurrentLiabilities",
                        "LongtermBorrowings",
                        "Borrowings",
                        "DebtNoncurrent",
                        "LongTermObligations"
                    }),
                // https://data.sec.gov/api/xbrl/companyconcept/CIK0000200406/us-gaap/PaymentsOfDividends.json
                "DividendsCommand" => new SimpleAccountingAttribute(
                    "Dividends",
                    new List<string>
                    {
                        "DividendsCommonStockCash",
                        "DividendsCash",
                        "Dividends",
                        "PaymentsOfDividends",
                        "DividendsPaid"
                    }),
                _ => throw new NotImplementedException()
            };

            return new RelayCommand(
                async () =>
                {
                    Ui ui = new();
                    ui.Disable(edgarConsumer, PROGRESS_BAR_DELAY);

                    await RunBatchProcessingAsync(
                        edgarConsumer,
                        simpleAccountingAttribute, 
                        edgarConsumer.EdgarProvider.BatchProcessing);

                    ui.Enable(edgarConsumer);
                });
        }

        public static ICommand? CreateInterpolate(IEdgarConsumer edgarConsumer)
        {
            return new RelayCommand(
               async () =>
               {
                   List<string> resultList = edgarConsumer.ResultCollectionEdgar.ToList();

                   Ui ui = new();
                   ui.Disable(edgarConsumer, PROGRESS_BAR_DELAY);

                   await Task.Run(() =>
                   {
                       edgarConsumer.ResultCollectionEdgar = new ObservableCollection<string>(
                           edgarConsumer.EdgarProvider.InterpolateDataForMissingDates(resultList));
                   });

                   ui.Enable(edgarConsumer);
               });
        }

        public static ICommand CreateCagr(IEdgarConsumer edgarConsumer)
        {
            return new RelayCommand(
                async () =>
                {
                    List<string> resultList = edgarConsumer.ResultCollectionEdgar.ToList();

                    Ui ui = new();
                    ui.Disable(edgarConsumer, PROGRESS_BAR_DELAY);

                    await Task.Run(() =>
                    {
                        edgarConsumer.ResultCollectionEdgar = new ObservableCollection<string>(
                            edgarConsumer.EdgarProvider.Cagr(resultList, 10));
                    });

                    ui.Enable(edgarConsumer);
                });
        }
    }
}
