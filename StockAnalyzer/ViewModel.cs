using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IbClient.IbHost;
using StockAnalyzer.Commands;
using StockAnalyzer.DataProviders;
using StockAnalyzer.DataProviders.Types;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace StockAnalyzer
{
    public class ViewModel : ObservableObject, IIbConsumer, IEdgarConsumer
    {
        private const int TIMEOUT_SIMPLE_BROWSER = 0;
        private const int TIMEOUT_TWS = 1000;
        private readonly string REPORT_SNAPSHOT = "ReportSnapshot";
        private readonly string REPORTS_FIN_STATEMENTS = "ReportsFinStatements";
        private readonly int INVESTMENT_AMOUNT = 6400;
        private ObservableCollection<string> _resultCollectionEarningsForWeek = null!;
        private string _htmlSourceEarningsForWeek = null!;
        private double _marketCap;

        IEdgarProvider _edgarProvider;
        private ObservableCollection<string> _tickerCollectionEdgar = null!;
        private string _messageEdgar = null!;

        private ObservableCollection<string> _tickerCollectionYahoo = null!;
        private ObservableCollection<string> _tickersAlphaList = null!;

        private ObservableCollection<string> _resultCollectionEdgar = null!;
        private ObservableCollection<string> _resultCollectionYahooEps = null!;
        private ObservableCollection<string> _resultCollectionAlpha = null!;
        private string _messageYahoo = null!;
        private int _decimalSeparatorSelectedIndexYahoo;

        private string _host = "localhost";
        private int _port = 4001;
        private int _clientId = 1;
        private bool _connectedToTws;
        private ObservableCollection<string> _twsMessageColllection = new ObservableCollection<string>();
        private ObservableCollection<string> _tickerCollectionTwsContractDetails = null!;
        private ObservableCollection<string> _resultCollectionTwsContractIds = null!;
        private ObservableCollection<string> _contractStringsTwsFinStatements = null!;
        private ObservableCollection<string> _resultCollectionTwsFinStatements = null!;
        private double _riskFreeRate;

        private ObservableCollection<string> _tickerCollectionTwsSummary = null!;
        private ObservableCollection<string> _resultCollectionTwsSummary = null!;

        private int _progressBarValue;
        private Brush _backgroundResults = null!;

        public ICommand EquityCommand { get; }
        public ICommand LongTermDebtCommand { get; }
        public ICommand DividendsCommand { get; }
        public ICommand NetIncomeCommand { get; }
        public ICommand LastEpsCommand { get; }
        public ICommand ExpectedEpsCommand { get; }
        public ICommand EarningsForWeekCommand { get; }
        public ICommand ConnectToTwsCommand { get; }
        public ICommand ContractIdsCommand { get; }
        public ICommand RoeCommand { get; }
        public ICommand PayoutRatioYCommand { get; }
        public ICommand PayoutRatioQCommand { get; }
        public ICommand SharesOutYCommand { get; }
        public ICommand SharesOutQCommand { get; }
        public ICommand CurrentPriceCommand { get; }
        public ICommand MarginCommand { get; }
        public ICommand RiskAndReturnCommand { get; }
        public ICommand TwsSummaryCommand { get; }
        public ICommand ComparePeersCommand { get; }
        public ICommand InterpolateCommand { get; }

        public ViewModel(
            IInvestingProvider investingProvider,
            IYahooProvider yahooProvider,
            ITwsProvider twsProvider,
            ISeekingAlphaProvider seekingAlphaProvider,
            IEdgarProvider edgarProvider,
            IIbHost ibHost)
        {
            _edgarProvider = edgarProvider;

            yahooProvider.Status += YahooProvider_Status;
            twsProvider.Status += TwsProvider_Status;

            // https://data.sec.gov/api/xbrl/companyconcept/CIK0000200406/us-gaap/StockholdersEquity.json
            EquityCommand = new RelayCommand(async ()
                => await EdgarBatchProcessor.RunBatchProcessingAsync(this, new List<string>
                {
                    "StockholdersEquity",
                    "Equity",
                    "EquityAttributableToOwnersOfParent",
                    "EquityAttributableToParent",
                    "TotalEquity",
                    "EquityAttributableToNoncontrollingInterest"
                },
                edgarProvider.BatchProcessing));

            // https://data.sec.gov/api/xbrl/companyconcept/CIK0000200406/us-gaap/LongTermDebt.json
            LongTermDebtCommand = new RelayCommand(async ()
                => await EdgarBatchProcessor.RunBatchProcessingAsync(this, new List<string>
                {
                    "LongTermDebt",
                    "NoncurrentLiabilities",
                    "LongtermBorrowings",
                    "Borrowings",
                    "DebtNoncurrent",
                    "LongTermObligations"
                },
                edgarProvider.BatchProcessing));

            // https://data.sec.gov/api/xbrl/companyconcept/CIK0000200406/us-gaap/PaymentsOfDividends.json
            DividendsCommand = new RelayCommand(async ()
                => await EdgarBatchProcessor.RunBatchProcessingAsync(this, new List<string>
                {
                    "DividendsCommonStockCash",
                    "DividendsCash",
                    "Dividends",
                    "PaymentsOfDividends",
                    "DividendsPaid"
                },
                edgarProvider.BatchProcessing));

            // https://data.sec.gov/api/xbrl/companyconcept/CIK0000200406/us-gaap/PaymentsOfDividends.json
            NetIncomeCommand = new RelayCommand(async ()
                => await EdgarBatchProcessor.RunBatchProcessingAsync(this, new List<string>
                {
                    "NetIncomeLoss",
                    "ProfitLoss"
                },
                edgarProvider.BatchProcessing));

            InterpolateCommand = new RelayCommand(() =>
            {
                ResultCollectionEdgar = new ObservableCollection<string>(edgarProvider.InterpolateDataForMissingDates(ResultCollectionEdgar.ToList()));
            });

            LastEpsCommand = new RelayCommand(async () =>
            {
                DecimalSeparatorSelectedIndexYahoo = 0;
                ResultCollectionYahooEps = new ObservableCollection<string>(await yahooProvider.LastEpsAsync(
                    TickerCollectionYahoo.ToList(),
                    TIMEOUT_SIMPLE_BROWSER));
            });

            ExpectedEpsCommand = new RelayCommand(async () =>
            {
                DecimalSeparatorSelectedIndexYahoo = 0;
                ResultCollectionYahooEps = new ObservableCollection<string>(await yahooProvider.ExpectedEpsAsync(
                    TickerCollectionYahoo.ToList(),
                    TIMEOUT_SIMPLE_BROWSER));
            });

            EarningsForWeekCommand = new RelayCommand(() =>
            {
                if (HtmlSourceEarningsForWeek == null || HtmlSourceEarningsForWeek == string.Empty)
                {
                    MessageBox.Show("HTML Source can not be empty!");
                    return;
                }
                ResultCollectionEarningsForWeek = new ObservableCollection<string>(investingProvider.GetEarningsData(HtmlSourceEarningsForWeek, MarketCap));

                // Test HTML Source
                //ResultListEarningsForWeek = new ObservableCollection<string>(investingProvider.GetEarningsData(TestData.TestStringEarningData(), MarketCap));
            });

            ConnectToTwsCommand = new RelayCommand(() =>
            {
                ibHost.Consumer = ibHost.Consumer ?? this;
                if (!ibHost.Consumer.ConnectedToTws)
                {
                    ibHost.ConnectAndStartReaderThread(Host, Port, ClientId, 1000);
                }
                else
                {
                    ibHost.Disconnect();
                }
            });

            ContractIdsCommand = new RelayCommand(async () =>
            {
                ibHost.Consumer ??= this;
                ConnectToTwsIfNeeded();
                List<string> contractStringsList = ContractStringsTwsContractDetails.ToList();
                var contractDetailsList = await twsProvider.ContractDetailsListFromContractStringsList(
                                    contractStringsList,
                                    TIMEOUT_TWS);
                ResultCollectionTwsContractIds = new ObservableCollection<string>(twsProvider.ConIdsFromContractDetailsList(contractDetailsList));
            });

            RoeCommand = new RelayCommand(async () =>
            {
                ibHost.Consumer ??= this;
                ConnectToTwsIfNeeded();
                List<string> contractStringsList = ContractStringsTwsFinStatements.ToList();
                List<DataStringWithTicker> fundamentalDataListRoe = await twsProvider.FundamentalDataFromContractStrings(
                                    contractStringsList,
                                    REPORT_SNAPSHOT,
                                    TIMEOUT_TWS);
                ResultCollectionTwsFinStatements = new ObservableCollection<string>(
                    twsProvider.RoeFromFundamentalDataList(fundamentalDataListRoe));
            });

            PayoutRatioYCommand = new RelayCommand(async () =>
            {
                ibHost.Consumer ??= this;
                ConnectToTwsIfNeeded();
                List<string> contractStringsList = ContractStringsTwsFinStatements.ToList();

                List<DataStringWithTicker> fundamentalDataListPayoutRatio = await twsProvider.FundamentalDataFromContractStrings(
                                    contractStringsList,
                                    REPORTS_FIN_STATEMENTS,
                                    TIMEOUT_TWS);
                ResultCollectionTwsFinStatements = new ObservableCollection<string>(
                    twsProvider.PayoutRatioYFromFundamentalDataList(fundamentalDataListPayoutRatio));
            });

            PayoutRatioQCommand = new RelayCommand(async () =>
            {
                ibHost.Consumer ??= this;
                ConnectToTwsIfNeeded();
                List<string> contractStringsList = ContractStringsTwsFinStatements.ToList();

                List<DataStringWithTicker> fundamentalDataListPayoutRatio = await twsProvider.FundamentalDataFromContractStrings(
                                    contractStringsList,
                                    REPORTS_FIN_STATEMENTS,
                                    TIMEOUT_TWS);
                ResultCollectionTwsFinStatements = new ObservableCollection<string>(
                    twsProvider.QuarterlyDataFromFundamentalDataList(
                        fundamentalDataListPayoutRatio,
                        twsProvider.PayoutRatioTwiceAYearCalculations,
                        twsProvider.PayoutRatioQuarterlyCalculations,
                        "Extracting Payout Ratio (Q) from the fundamental data list"));
            });

            SharesOutYCommand = new RelayCommand(async () =>
            {
                ibHost.Consumer ??= this;
                ConnectToTwsIfNeeded();
                List<string> contractStringsList = ContractStringsTwsFinStatements.ToList();

                List<DataStringWithTicker> fundamentalDataListFinStatements = await twsProvider.FundamentalDataFromContractStrings(
                                    contractStringsList,
                                    REPORTS_FIN_STATEMENTS,
                                    TIMEOUT_TWS);
                ResultCollectionTwsFinStatements = new ObservableCollection<string>(
                    twsProvider.SharesOutYFromFundamentalDataList(fundamentalDataListFinStatements));
            });

            SharesOutQCommand = new RelayCommand(async () =>
            {
                ibHost.Consumer ??= this;
                ConnectToTwsIfNeeded();
                List<string> contractStringsList = ContractStringsTwsFinStatements.ToList();

                List<DataStringWithTicker> fundamentalDataListFinStatements = await twsProvider.FundamentalDataFromContractStrings(
                                    contractStringsList,
                                    REPORTS_FIN_STATEMENTS,
                                    TIMEOUT_TWS);
                ResultCollectionTwsFinStatements = new ObservableCollection<string>(
                   twsProvider.QuarterlyDataFromFundamentalDataList(
                       fundamentalDataListFinStatements,
                       twsProvider.SharesOutTwiceAYearCalculations,
                       twsProvider.SharesOutQuarterlyCalculations,
                       "Extracting Total Shares Outstanding (Q) from the fundamental data list"));
            });


            CurrentPriceCommand = new RelayCommand(async () =>
            {
                ibHost.Consumer ??= this;
                ConnectToTwsIfNeeded();
                List<string> contractStringsList = ContractStringsTwsFinStatements.ToList();
                List<string> currentPriceString = await twsProvider.CurrentPriceFromContractStrings(
                                                    contractStringsList,
                                                    TIMEOUT_TWS);
                ResultCollectionTwsFinStatements = new ObservableCollection<string>(currentPriceString);
            });

            MarginCommand = new RelayCommand(async () =>
            {
                ibHost.Consumer ??= this;
                ConnectToTwsIfNeeded();
                List<string> contractStringsList = ContractStringsTwsFinStatements.ToList();
                List<string> marginListWithTicker = await twsProvider.MarginListFromContractStrings(
                                    contractStringsList,
                                    TIMEOUT_TWS, INVESTMENT_AMOUNT);
                ResultCollectionTwsFinStatements = new ObservableCollection<string>(marginListWithTicker);
            });

            RiskAndReturnCommand = new RelayCommand(async () =>
            {
                ibHost.Consumer ??= this;
                ConnectToTwsIfNeeded();
                List<string> contractStringsList = ContractStringsTwsFinStatements.ToList();
                List<string> riskAndReturnListWithTicker = await twsProvider.RiskAndReturnFromContractStrings(
                                    contractStringsList,
                                    TIMEOUT_TWS);
                ResultCollectionTwsFinStatements = new ObservableCollection<string>(riskAndReturnListWithTicker);
            });

            TwsSummaryCommand = new RelayCommand(async () =>
            {
                ibHost.Consumer ??= this;
                ConnectToTwsIfNeeded();
                List<string> contractStringsList = ContractStringsTwsSummary.ToList();
                List<DataStringWithTicker> fundamentalDataListSummary = await twsProvider.FundamentalDataFromContractStrings(
                                    contractStringsList,
                                    REPORT_SNAPSHOT,
                                    TIMEOUT_TWS);
                ResultCollectionTwsSummary = new ObservableCollection<string>(
                    twsProvider.DesriptionOfCompanyFromFundamentalDataList(fundamentalDataListSummary));
            });

            ComparePeersCommand = new RelayCommand(() =>
            {
                MessageBox.Show("All Chrome windows must be closed!");
                ResultCollectionAlpha = new ObservableCollection<string>(seekingAlphaProvider.PeersComparison(
                                    TickersAlphaList.ToList(),
                                    TIMEOUT_SIMPLE_BROWSER));
            });

            MarketCap = 00.1;
            TickerCollectionEdgar = new ObservableCollection<string>(TestStringEdgar().ToList());

            TickerCollectionYahoo = new ObservableCollection<string>((" SKX\r\nPFS\r\nSLCA\r\n WT").Split("\r\n").ToList());
            ContractStringsTwsContractDetails = new ObservableCollection<string>(("ALD1;EUR;STK;SBF\r\nBWLPG;NOK;STK\r\nPFS\r\nSLCA").Split("\r\n").ToList());
            ContractStringsTwsFinStatements = new ObservableCollection<string>(("ALD1;EUR;STK;SBF\r\nBWLPG;NOK;\r\nPFS\r\nSLCA").Split("\r\n").ToList());
            ContractStringsTwsSummary = new ObservableCollection<string>(("BWLPG ;NOK ;STK ; SMART\r\nPFS\r\nSLCA").Split("\r\n").ToList());
            TickersAlphaList = new ObservableCollection<string>("MSFT	ORCL	NOW	PANW	CRWD	FTNT".Split("\t").ToList()
                .Select(t => t.Trim()));
            RiskFreeRate = 5.5;
            BackgroundResults = new SolidColorBrush(Colors.White);
        }

        #region Edgar

        public IEdgarProvider EdgarProvider => _edgarProvider;

        public ObservableCollection<string> TickerCollectionEdgar
        {
            get => _tickerCollectionEdgar;
            set
            {
                SetProperty(ref _tickerCollectionEdgar, value);
            }
        }
        public ObservableCollection<string> ResultCollectionEdgar
        {
            get => _resultCollectionEdgar;
            set
            {
                SetProperty(ref _resultCollectionEdgar, value);
            }
        }

        public int ProgressBarValue
        {
            get => _progressBarValue;
            set
            {
                SetProperty(ref _progressBarValue, value);
            }
        }

        public Brush BackgroundResults
        {
            get => _backgroundResults;
            set
            {
                SetProperty(ref _backgroundResults, value);
            }
        }

        public string MessageEdgar
        {
            get => _messageEdgar;
            set
            {
                SetProperty(ref _messageEdgar, value);
            }
        }

        public void AddMessageEdgar(string message)
        {
            if (string.IsNullOrEmpty(MessageEdgar))
            {
                MessageEdgar = message;
            }
            else
            {
                MessageEdgar = message + "\r\n" + MessageEdgar;
            }
        }

        private IEnumerable<string> TestStringEdgar()
        {
            //return (" NVDA\r\nGOOG\r\nMSFT\r\nAAPL\r\nAMZN\r\nMETA\r\nTSLA").Split("\r\n").ToList(); ;
            //return (" KO\r\nPEP\r\nBUD").Split("\r\n").ToList();
            return (" KO\r\nBEOB\r\nPEP\r\nBUD").Split("\r\n").ToList();
        }

        #endregion

        #region Yahoo
        public ObservableCollection<string> TickerCollectionYahoo
        {
            get => _tickerCollectionYahoo;
            set
            {
                SetProperty(ref _tickerCollectionYahoo, value);
            }
        }

        public ObservableCollection<string> ResultCollectionYahooEps
        {
            get => _resultCollectionYahooEps;
            set
            {
                SetProperty(ref _resultCollectionYahooEps, value);
            }
        }

        public string MessageYahoo
        {
            get => _messageYahoo;
            set
            {
                SetProperty(ref _messageYahoo, value);
            }
        }
        
        public int DecimalSeparatorSelectedIndexYahoo
        {
            get => _decimalSeparatorSelectedIndexYahoo;
            set
            {
                SetProperty(ref _decimalSeparatorSelectedIndexYahoo, value);
            }
        }

        private void YahooProvider_Status(string message)
        {
            MessageYahoo = message;
        }

        #endregion  Yahoo

        #region Investing

        public string HtmlSourceEarningsForWeek
        {
            get => _htmlSourceEarningsForWeek;
            set
            {
                SetProperty(ref _htmlSourceEarningsForWeek, value);
            }
        }

        public double MarketCap
        {
            get => _marketCap;
            set
            {
                SetProperty(ref _marketCap, value);
            }
        }

        public ObservableCollection<string> ResultCollectionEarningsForWeek
        {
            get => _resultCollectionEarningsForWeek;
            set
            {
                SetProperty(ref _resultCollectionEarningsForWeek, value);
            }
        }

        #endregion Investing

        #region TWS

        public string Host
        {
            get => _host;
            set
            {
                SetProperty(ref _host, value);
            }
        }

        public int Port
        {
            get => _port;
            set
            {
                SetProperty(ref _port, value);
            }
        }

        public int ClientId
        {
            get => _clientId;
            set
            {
                SetProperty(ref _clientId, value);
            }
        }
        public bool ConnectedToTws
        {
            get => _connectedToTws;
            set
            {
                SetProperty(ref _connectedToTws, value);
            }
        }

        public ObservableCollection<string> TwsMessageCollection
        {
            get => _twsMessageColllection;
            set
            {
                SetProperty(ref _twsMessageColllection, value);
            }
        }

        public ObservableCollection<string> ContractStringsTwsContractDetails
        {
            get => _tickerCollectionTwsContractDetails;
            set
            {
                SetProperty(ref _tickerCollectionTwsContractDetails, value);
            }
        }

        public ObservableCollection<string> ResultCollectionTwsContractIds
        {
            get => _resultCollectionTwsContractIds;
            set
            {
                SetProperty(ref _resultCollectionTwsContractIds, value);
            }
        }

        public ObservableCollection<string> ContractStringsTwsFinStatements
        {
            get => _contractStringsTwsFinStatements;
            set
            {
                SetProperty(ref _contractStringsTwsFinStatements, value);
            }
        }

        public ObservableCollection<string> ResultCollectionTwsFinStatements
        {
            get => _resultCollectionTwsFinStatements;
            set
            {
                SetProperty(ref _resultCollectionTwsFinStatements, value);
            }
        }

        public double RiskFreeRate
        {
            get => _riskFreeRate;
            set
            {
                SetProperty(ref _riskFreeRate, value);
            }
        }

        private void TwsProvider_Status(string message)
        {
            TwsMessageCollection.Add(message);
        }

        private void ConnectToTwsIfNeeded()
        {
            if (!ConnectedToTws)
            {
                ConnectToTwsCommand?.Execute(null);
            }
        }

        #endregion TWS

        #region TWS Summary

        #region Seeking Alpha

        public ObservableCollection<string> TickersAlphaList
        {
            get => _tickersAlphaList;
            set
            {
                SetProperty(ref _tickersAlphaList, value);
            }
        }

        public ObservableCollection<string> ResultCollectionAlpha
        {
            get => _resultCollectionAlpha;
            set
            {
                SetProperty(ref _resultCollectionAlpha, value);
            }
        }


        #endregion

        public ObservableCollection<string> ContractStringsTwsSummary
        {
            get => _tickerCollectionTwsSummary;
            set
            {
                SetProperty(ref _tickerCollectionTwsSummary, value);
            }
        }

        public ObservableCollection<string> ResultCollectionTwsSummary
        {
            get => _resultCollectionTwsSummary;
            set
            {
                SetProperty(ref _resultCollectionTwsSummary, value);
            }
        }

        #endregion
    }
}
