using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace StockAnalyzer.Commands
{
    public class CommandFactory
    {
        public static ICommand? Create(string commandName, IEdgarConsumer edgarConsumer)
        {
            List<string> companyConceptArray = commandName switch
            {
                // https://data.sec.gov/api/xbrl/companyconcept/CIK0000200406/us-gaap/StockholdersEquity.json
                "EquityCommand" => new List<string>
                    {
                        "StockholdersEquity",
                        "Equity",
                        "EquityAttributableToOwnersOfParent",
                        "EquityAttributableToParent",
                        "TotalEquity",
                        "EquityAttributableToNoncontrollingInterest"
                    },
                // https://data.sec.gov/api/xbrl/companyconcept/CIK0000200406/us-gaap/LongTermDebt.json
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
                // https://data.sec.gov/api/xbrl/companyconcept/CIK0000200406/us-gaap/NetIncomeLoss.json
                "NetIncomeCommand" => new List<string>
                    {
                        "NetIncomeLoss",
                        "ProfitLoss"
                    },
                _ => throw new NotImplementedException(),
            };

            return new RelayCommand(
                async () => await EdgarBatchProcessor.RunBatchProcessingAsync(
                    edgarConsumer,
                    companyConceptArray,
                    edgarConsumer.EdgarProvider.BatchProcessing));
        }
    }
}
