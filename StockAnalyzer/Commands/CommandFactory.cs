﻿using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StockAnalyzer.Commands
{
    public class CommandFactory
    {
        const int PROGRESS_BAR_DELAY = 400;

        public static ICommand? CreateBatchProcessing(string commandName, IEdgarConsumer edgarConsumer)
        {
            List<string> companyConceptArray = commandName switch
            {
                "RevenueCommand" => new List<string>
                    {
                        "RevenueFromContractWithCustomerExcludingAssessedTax",
                        "SalesRevenueNet",
                        "Revenues",
                        "OperatingRevenue"
                    },
                "CogsCommand" => new List<string>
                    {
                        "CostOfGoodsAndServicesSold"
                    },
                "OperatingCostCommand" => new List<string>
                    {
                        "OperatingExpenses"
                    },
                "FinancingCostCommand" => new List<string>
                    {
                        "InterestIncomeExpenseNonoperatingNet",
                        "InterestExpense",
                        "FinanceCost",
                        "BorrowingCost",
                        "DebtInterestExpense"
                    },
                "TaxCommand" => new List<string>
                    {
                        "IncomeTaxExpenseBenefit"
                    },
                // https://data.sec.gov/api/xbrl/companyconcept/CIK0000200406/us-gaap/NetIncomeLoss.json
                "NetIncomeCommand" => new List<string>
                    {
                        "NetIncomeLoss",
                        "ProfitLoss"
                    },
                // https://data.sec.gov/api/xbrl/companyconcept/CIK0000PROGRESS_BAR_DELAY406/us-gaap/StockholdersEquity.json
                "EquityCommand" => new List<string>
                    {
                        "StockholdersEquity",
                        "Equity",
                        "EquityAttributableToOwnersOfParent",
                        "EquityAttributableToParent",
                        "TotalEquity",
                        "EquityAttributableToNoncontrollingInterest"
                    },
                // https://data.sec.gov/api/xbrl/companyconcept/CIK0000PROGRESS_BAR_DELAY406/us-gaap/LongTermDebt.json
                "LongTermDebtCommand" => new List<string>
                    {
                        "LongTermDebt",
                        "NoncurrentLiabilities",
                        "LongtermBorrowings",
                        "Borrowings",
                        "DebtNoncurrent",
                        "LongTermObligations"
                    },
                // https://data.sec.gov/api/xbrl/companyconcept/CIK0000200406/us-gaap/PaymentsOfDividends.json
                "DividendsCommand" => new List<string>
                    {
                        "DividendsCommonStockCash",
                        "DividendsCash",
                        "Dividends",
                        "PaymentsOfDividends",
                        "DividendsPaid"
                    },
                _ => throw new NotImplementedException(),
            };

            return new RelayCommand(
                async () =>
                {
                    Ui ui = new();
                    ui.Disable(edgarConsumer, PROGRESS_BAR_DELAY);

                    await EdgarBatchProcessor.RunBatchProcessingAsync(
                        edgarConsumer,
                        companyConceptArray,
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
