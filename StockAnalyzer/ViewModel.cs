using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IbClient.IbHost;
using StockAnalyzer.DataProviders;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace StockAnalyzer
{
    public class ViewModel : ObservableObject, IIbConsumer
    {
        private const int TIMEOUT_SIMPLE_BROWSER = 0;
        private const int TIMEOUT_TWS = 3000;
        private readonly string REPORT_SNAPSHOT = "ReportSnapshot";

        private ObservableCollection<string> _resultCollectionEarningsForWeek = null!;
        private string _htmlSourceEarningsForWeek = null!;
        private double _marketCap;

        private ObservableCollection<string> _tickerCollectionYahoo = null!;
        private ObservableCollection<string> _resultCollectionYahooEps = null!;
        private string _messageYahoo = null!;
        private int _decimalSeparatorSelectedIndexYahoo;

        private string _host = "localhost";
        private int _port = 4001;
        private int _clientId = 1;
        private bool _connectedToTws;
        private ObservableCollection<string> _twsMessageColllection = new ObservableCollection<string>();
        private ObservableCollection<string> _tickerCollectionTwsContractDetails = null!;
        private ObservableCollection<string> _resultCollectionTwsContractIds = null!;
        private ObservableCollection<string> _tickerColllectionTwsRoe = null!;
        private ObservableCollection<string> _resultColllectionTwsRoe = null!;

        private ObservableCollection<string> _tickerCollectionTwsSummary = null!;
        private ObservableCollection<string> _resultCollectionTwsSummary = null!;

        public ICommand LastEpsCommand { get; }
        public ICommand ExpectedEpsCommand { get; }
        public ICommand EarningsForWeekCommand { get; }
        public ICommand ConnectToTwsCommand { get; }
        public ICommand ContractIdsCommand { get; }
        public ICommand RoeCommand { get; }
        public ICommand TwsSummaryCommand { get; }

        public ViewModel(
            IInvestingProvider investingProvider,
            IYahooProvider yahooProvider,
            ITwsProvider twsProvider,
            IIbHost ibHost)
        {
            yahooProvider.Status += YahooProvider_Status;
            twsProvider.Status += TwsProvider_Status;

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
                //ResultListEarningsForWeek = new ObservableCollection<string>(investingProvider.GetEarningsData(testStringEarningData, MarketCap));
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
                var contractDetailsList = await twsProvider.GetContractDetails(
                                    ContractStringsTwsContractDetails.ToList(),
                                    TIMEOUT_TWS);
                ResultCollectionTwsContractIds = new ObservableCollection<string>(twsProvider.ExtractIdsFromContractDetailsList(contractDetailsList));
            });

            RoeCommand = new RelayCommand(async () =>
            {
                ibHost.Consumer ??= this;
                ConnectToTwsIfNeeded();
                List<string> fundamentalDataListRoe = await twsProvider.GetFundamentalData(
                                    ContractStringsTwsRoe.ToList(),
                                    REPORT_SNAPSHOT,
                                    TIMEOUT_TWS);
                ResultCollectionTwsRoe = new ObservableCollection<string>(twsProvider.ExtractRoeFromFundamentalDataList(fundamentalDataListRoe));
            });

            TwsSummaryCommand = new RelayCommand(async () =>
            {
                ibHost.Consumer ??= this;
                ConnectToTwsIfNeeded();
                List<string> fundamentalDataListSummary = await twsProvider.GetFundamentalData(
                                    ContractStringsTwsSummary.ToList(),
                                    REPORT_SNAPSHOT,
                                    TIMEOUT_TWS);
                ResultCollectionTwsSummary = new ObservableCollection<string>(twsProvider.ExtractSummaryFromFundamentalDataList(fundamentalDataListSummary));
            });

            MarketCap = 0.1;
            TickerCollectionYahoo = new ObservableCollection<string>((" SKX\r\nPFS\r\nSLCA\r\n WT").Split("\r\n").ToList());
            ContractStringsTwsContractDetails = new ObservableCollection<string>((" SKX\r\nPFS\r\nSLCA").Split("\r\n").ToList());
            ContractStringsTwsRoe = new ObservableCollection<string>((" SKX\r\nPFS\r\nSLCA").Split("\r\n").ToList());
            ContractStringsTwsSummary = new ObservableCollection<string>((" SKX\r\nPFS\r\nSLCA").Split("\r\n").ToList());
        }

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

        public ObservableCollection<string> ContractStringsTwsRoe
        {
            get => _tickerColllectionTwsRoe;
            set
            {
                SetProperty(ref _tickerColllectionTwsRoe, value);
            }
        }

        public ObservableCollection<string> ResultCollectionTwsRoe
        {
            get => _resultColllectionTwsRoe;
            set
            {
                SetProperty(ref _resultColllectionTwsRoe, value);
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

        #region Test Data

        string testStringEarningData = @"<table id=""earningsCalendarData"" class=""genTbl closedTbl ecoCalTbl earnings persistArea js-earnings-table"" tablesorter="""">
<thead>
<tr>
<th></th>
<th class=""left pointer"">Company<span sort_default="""" class=""headerSortDefault""></span></th>
<th>EPS</th>
<th class=""noBold leftStrong"">/&nbsp;&nbsp;Forecast</th>
<th>Revenue</th>
<th class=""noBold leftStrong"">/&nbsp;&nbsp;Forecast</th>
<th class=""pointer"">Market Cap<span sort_default="""" class=""headerSortDefault""></span></th>
<th class=""noWrap right last time pointer"">Time<span sort_default="""" class=""headerSortDefault""></span></th>

<th></th>
</tr>
</thead>
<thead class=""floatingHeader"" style=""display: none; top: 96.75px;"">
<tr>
<th class=""pointer"" style=""width: 30px;""></th>
<th class=""left"" style=""width: 198px;"">Company<span sort_default="""" class=""headerSortDefault""></span></th>
<th style=""width: 26px;"">EPS</th>
<th class=""noBold leftStrong"" style=""width: 61px;"">/&nbsp;&nbsp;Forecast</th>
<th style=""width: 54px;"">Revenue</th>
<th class=""noBold leftStrong pointer"" style=""width: 77px;"">/&nbsp;&nbsp;Forecast</th>
<th class=""pointer"" style=""width: 80px;"">Market Cap<span sort_default="""" class=""headerSortDefault""></span></th>
<th class=""noWrap right last time pointer"" style=""width: 45px;"">Time<span sort_default="""" class=""headerSortDefault""></span></th>

<th class=""pointer"" style=""width: 24px;""></th>
</tr>
</thead>
<tbody>        <tr tablesorterdivider="""">
            <td colspan=""9"" class=""theDay"">Monday, October 30, 2023</td>
        </tr>
                        <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""HSBC Holdings PLC ADR"" _p_pid=""20871"" _r_pid=""20871"">
                        <span class=""earnCalCompanyName middle"">HSBC ADR</span>&nbsp;(<a href=""/equities/hsbc-holdings-plc-earnings"" class=""bold middle"" target=""_blank"">HSBC</a>)
                    </td>
                    <td class="" pid-20871-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.48</td>
                    <td class="" pid-20871-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;16.4B</td>
                    <td class=""right"">151.87B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20871""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Glencore PLC"" _p_pid=""13811"" _r_pid=""52803"">
                        <span class=""earnCalCompanyName middle"">Glencore</span>&nbsp;(<a href=""/equities/glencore-earnings?cid=52803"" class=""bold middle"" target=""_blank"">GLCNF</a>)
                    </td>
                    <td class="" pid-52803-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-52803-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">68.52B</td>
                    <td class=""right time"" data-value=""1""><span class=""marketOpen genToolTip oneliner reverseToolTip"" data-tooltip=""Before market open""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""52803""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Glencore PLC ADR"" _p_pid=""941913"" _r_pid=""941913"">
                        <span class=""earnCalCompanyName middle"">Glencore ADR</span>&nbsp;(<a href=""/equities/glencore-plc-earnings"" class=""bold middle"" target=""_blank"">GLNCY</a>)
                    </td>
                    <td class="" pid-941913-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-941913-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">68.52B</td>
                    <td class=""right time"" data-value=""1""><span class=""marketOpen genToolTip oneliner reverseToolTip"" data-tooltip=""Before market open""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""941913""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""ON Semiconductor Corporation"" _p_pid=""16812"" _r_pid=""16812"">
                        <span class=""earnCalCompanyName middle"">ON Semiconductor</span>&nbsp;(<a href=""/equities/on-semiconductor-earnings"" class=""bold middle"" target=""_blank"">ON</a>)
                    </td>
                    <td class="" pid-16812-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.34</td>
                    <td class="" pid-16812-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.14B</td>
                    <td class=""right"">40.48B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16812""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Bank Mandiri Persero Tbk PT ADR"" _p_pid=""942119"" _r_pid=""942119"">
                        <span class=""earnCalCompanyName middle"">Bank Mandiri Persero ADR</span>&nbsp;(<a href=""/equities/pt-bank-mandiri-persero-tbk-earnings"" class=""bold middle"" target=""_blank"">PPERY</a>)
                    </td>
                    <td class="" pid-942119-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.1855</td>
                    <td class="" pid-942119-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.23B</td>
                    <td class=""right"">36.46B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""942119""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Waste Connections Inc"" _p_pid=""29728"" _r_pid=""29728"">
                        <span class=""earnCalCompanyName middle"">Waste Connections</span>&nbsp;(<a href=""/equities/waste-connections-inc-earnings"" class=""bold middle"" target=""_blank"">WCN</a>)
                    </td>
                    <td class="" pid-29728-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.14</td>
                    <td class="" pid-29728-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.07B</td>
                    <td class=""right"">35.44B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""29728""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Komatsu Ltd"" _p_pid=""941998"" _r_pid=""941998"">
                        <span class=""earnCalCompanyName middle"">Komatsu</span>&nbsp;(<a href=""/equities/komatsu-ltd-earnings"" class=""bold middle"" target=""_blank"">KMTUY</a>)
                    </td>
                    <td class="" pid-941998-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.6599</td>
                    <td class="" pid-941998-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;6.23B</td>
                    <td class=""right"">27.85B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""941998""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Fanuc Corporation"" _p_pid=""941886"" _r_pid=""941886"">
                        <span class=""earnCalCompanyName middle"">Fanuc Corporation</span>&nbsp;(<a href=""/equities/fanuc-corporation-earnings"" class=""bold middle"" target=""_blank"">FANUY</a>)
                    </td>
                    <td class="" pid-941886-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.1063</td>
                    <td class="" pid-941886-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.34B</td>
                    <td class=""right"">25.47B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""941886""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Central Japan Railway Co"" _p_pid=""941800"" _r_pid=""941800"">
                        <span class=""earnCalCompanyName middle"">Central Japan Railway Co</span>&nbsp;(<a href=""/equities/central-japan-railway-co-earnings"" class=""bold middle"" target=""_blank"">CJPRY</a>)
                    </td>
                    <td class="" pid-941800-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.1818</td>
                    <td class="" pid-941800-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.75B</td>
                    <td class=""right"">25.25B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""941800""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Orix Corp Ads"" _p_pid=""7982"" _r_pid=""7982"">
                        <span class=""earnCalCompanyName middle"">Orix</span>&nbsp;(<a href=""/equities/orix-earnings"" class=""bold middle"" target=""_blank"">IX</a>)
                    </td>
                    <td class="" pid-7982-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.56</td>
                    <td class="" pid-7982-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;5.28B</td>
                    <td class=""right"">22.63B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""7982""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""SBA Communications Corp"" _p_pid=""39107"" _r_pid=""39107"">
                        <span class=""earnCalCompanyName middle"">SBA Communications</span>&nbsp;(<a href=""/equities/sba-communications-corp-earnings"" class=""bold middle"" target=""_blank"">SBAC</a>)
                    </td>
                    <td class="" pid-39107-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.2</td>
                    <td class="" pid-39107-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;680.55M</td>
                    <td class=""right"">21.8B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""39107""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Advanced Info Service Public"" _p_pid=""941710"" _r_pid=""941710"">
                        <span class=""earnCalCompanyName middle"">Advanced Info Service Public</span>&nbsp;(<a href=""/equities/advanced-info-service-public-earnings"" class=""bold middle"" target=""_blank"">AVIFY</a>)
                    </td>
                    <td class="" pid-941710-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.0684</td>
                    <td class="" pid-941710-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.29B</td>
                    <td class=""right"">18.83B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""941710""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Kyocera Corporation ADR"" _p_pid=""8329"" _r_pid=""8329"">
                        <span class=""earnCalCompanyName middle"">Kyocera ADR</span>&nbsp;(<a href=""/equities/kyocera-earnings"" class=""bold middle"" target=""_blank"">KYOCY</a>)
                    </td>
                    <td class="" pid-8329-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.7543</td>
                    <td class="" pid-8329-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;3.59B</td>
                    <td class=""right"">18.21B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""8329""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Hologic Inc"" _p_pid=""6515"" _r_pid=""6515"">
                        <span class=""earnCalCompanyName middle"">Hologic</span>&nbsp;(<a href=""/equities/hologic-inc-earnings"" class=""bold middle"" target=""_blank"">HOLX</a>)
                    </td>
                    <td class="" pid-6515-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.8429</td>
                    <td class="" pid-6515-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;941.17M</td>
                    <td class=""right"">17.2B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""6515""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""First Quantum Minerals Ltd"" _p_pid=""14037"" _r_pid=""52937"">
                        <span class=""earnCalCompanyName middle"">First Quantum Minerals</span>&nbsp;(<a href=""/equities/first-quantum-minerals-earnings?cid=52937"" class=""bold middle"" target=""_blank"">FQVLF</a>)
                    </td>
                    <td class="" pid-52937-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.2732</td>
                    <td class="" pid-52937-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.91B</td>
                    <td class=""right"">16.4B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""52937""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Astra International Tbk PT"" _p_pid=""942118"" _r_pid=""942118"">
                        <span class=""earnCalCompanyName middle"">Astra Int</span>&nbsp;(<a href=""/equities/pt-astra-international-earnings"" class=""bold middle"" target=""_blank"">PTAIY</a>)
                    </td>
                    <td class="" pid-942118-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.1544</td>
                    <td class="" pid-942118-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;5.01B</td>
                    <td class=""right"">16.34B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""942118""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Akamai Technologies Inc"" _p_pid=""6460"" _r_pid=""6460"">
                        <span class=""earnCalCompanyName middle"">Akamai</span>&nbsp;(<a href=""/equities/akamai-technologies-inc-earnings"" class=""bold middle"" target=""_blank"">AKAM</a>)
                    </td>
                    <td class="" pid-6460-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.5</td>
                    <td class="" pid-6460-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;943.26M</td>
                    <td class=""right"">16.32B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""6460""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Loews Corp"" _p_pid=""13083"" _r_pid=""13083"">
                        <span class=""earnCalCompanyName middle"">Loews</span>&nbsp;(<a href=""/equities/loews-corporation-earnings"" class=""bold middle"" target=""_blank"">L</a>)
                    </td>
                    <td class="" pid-13083-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-13083-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">14.55B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""13083""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""TDK Corp ADR"" _p_pid=""942224"" _r_pid=""942224"">
                        <span class=""earnCalCompanyName middle"">TDK ADR</span>&nbsp;(<a href=""/equities/tdk-corp-earnings"" class=""bold middle"" target=""_blank"">TTDKY</a>)
                    </td>
                    <td class="" pid-942224-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.7431</td>
                    <td class="" pid-942224-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;3.74B</td>
                    <td class=""right"">14.08B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""942224""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Erste Group Bank AG PK"" _p_pid=""941875"" _r_pid=""941875"">
                        <span class=""earnCalCompanyName middle"">Erste Group Bank AG PK</span>&nbsp;(<a href=""/equities/erste-group-bank-ag-pk-earnings"" class=""bold middle"" target=""_blank"">EBKDY</a>)
                    </td>
                    <td class="" pid-941875-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.8645</td>
                    <td class="" pid-941875-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.83B</td>
                    <td class=""right"">14.08B</td>
                    <td class=""right time"" data-value=""1""><span class=""marketOpen genToolTip oneliner reverseToolTip"" data-tooltip=""Before market open""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""941875""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""The Mosaic Company"" _p_pid=""8278"" _r_pid=""8278"">
                        <span class=""earnCalCompanyName middle"">Mosaic</span>&nbsp;(<a href=""/equities/mosaic-company-earnings"" class=""bold middle"" target=""_blank"">MOS</a>)
                    </td>
                    <td class="" pid-8278-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.7949</td>
                    <td class="" pid-8278-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;3.17B</td>
                    <td class=""right"">11.9B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""8278""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""CNA Financial Corporation"" _p_pid=""20805"" _r_pid=""20805"">
                        <span class=""earnCalCompanyName middle"">CNA Financial</span>&nbsp;(<a href=""/equities/cna-financial-corp-earnings"" class=""bold middle"" target=""_blank"">CNA</a>)
                    </td>
                    <td class="" pid-20805-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.9367</td>
                    <td class="" pid-20805-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;3.35B</td>
                    <td class=""right"">10.94B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20805""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""West Japan Railway Co ADR"" _p_pid=""942286"" _r_pid=""942286"">
                        <span class=""earnCalCompanyName middle"">West Japan Railway ADR</span>&nbsp;(<a href=""/equities/west-japan-railway-co-earnings"" class=""bold middle"" target=""_blank"">WJRYY</a>)
                    </td>
                    <td class="" pid-942286-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.7992</td>
                    <td class="" pid-942286-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.49B</td>
                    <td class=""right"">10.67B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""942286""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Banco De Chile"" _p_pid=""32344"" _r_pid=""32344"">
                        <span class=""earnCalCompanyName middle"">Banco De Chile</span>&nbsp;(<a href=""/equities/banco-del-chile-earnings"" class=""bold middle"" target=""_blank"">BCH</a>)
                    </td>
                    <td class="" pid-32344-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.6381</td>
                    <td class="" pid-32344-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;833.73M</td>
                    <td class=""right"">10.06B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""32344""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Nitto Denko Corp"" _p_pid=""942075"" _r_pid=""942075"">
                        <span class=""earnCalCompanyName middle"">Nitto Denko Corp</span>&nbsp;(<a href=""/equities/nitto-denko-corp-earnings"" class=""bold middle"" target=""_blank"">NDEKY</a>)
                    </td>
                    <td class="" pid-942075-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.6122</td>
                    <td class="" pid-942075-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.65B</td>
                    <td class=""right"">9.76B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""942075""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""FMC Corporation"" _p_pid=""13842"" _r_pid=""13842"">
                        <span class=""earnCalCompanyName middle"">FMC</span>&nbsp;(<a href=""/equities/fmc-corp-earnings"" class=""bold middle"" target=""_blank"">FMC</a>)
                    </td>
                    <td class="" pid-13842-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.06</td>
                    <td class="" pid-13842-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.23B</td>
                    <td class=""right"">8.53B</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""13842""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Turkiye Garanti Bankasi AS"" _p_pid=""942261"" _r_pid=""942261"">
                        <span class=""earnCalCompanyName middle"">Turkiye Garanti Bankasi AS</span>&nbsp;(<a href=""/equities/turkiye-garanti-bankasi-as-earnings"" class=""bold middle"" target=""_blank"">TKGBY</a>)
                    </td>
                    <td class="" pid-942261-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.087</td>
                    <td class="" pid-942261-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.21B</td>
                    <td class=""right"">7.85B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""942261""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""AngloGold Ashanti Ltd ADR"" _p_pid=""20610"" _r_pid=""20610"">
                        <span class=""earnCalCompanyName middle"">AngloGold Ashanti ADR</span>&nbsp;(<a href=""/equities/anglogold-ashanti-ltd-earnings"" class=""bold middle"" target=""_blank"">AU</a>)
                    </td>
                    <td class="" pid-20610-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.33</td>
                    <td class="" pid-20610-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">7.7B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20610""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Chemed Corp"" _p_pid=""21102"" _r_pid=""21102"">
                        <span class=""earnCalCompanyName middle"">Chemed</span>&nbsp;(<a href=""/equities/chemed-corp-earnings"" class=""bold middle"" target=""_blank"">CHE</a>)
                    </td>
                    <td class="" pid-21102-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;5</td>
                    <td class="" pid-21102-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;560.37M</td>
                    <td class=""right"">7.69B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""21102""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Cognex Corporation"" _p_pid=""15703"" _r_pid=""15703"">
                        <span class=""earnCalCompanyName middle"">Cognex</span>&nbsp;(<a href=""/equities/cognex-corp-earnings"" class=""bold middle"" target=""_blank"">CGNX</a>)
                    </td>
                    <td class="" pid-15703-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.1403</td>
                    <td class="" pid-15703-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;194M</td>
                    <td class=""right"">7.31B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""15703""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Civitas Resources Inc"" _p_pid=""1179973"" _r_pid=""1179973"">
                        <span class=""earnCalCompanyName middle"">Civitas Resources</span>&nbsp;(<a href=""/equities/civitas-resources-earnings"" class=""bold middle"" target=""_blank"">CIVI</a>)
                    </td>
                    <td class="" pid-1179973-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.31</td>
                    <td class="" pid-1179973-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;963.45M</td>
                    <td class=""right"">7.29B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1179973""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Avis Budget Group Inc"" _p_pid=""39223"" _r_pid=""39223"">
                        <span class=""earnCalCompanyName middle"">Avis</span>&nbsp;(<a href=""/equities/avis-budget-earnings"" class=""bold middle"" target=""_blank"">CAR</a>)
                    </td>
                    <td class="" pid-39223-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;14.44</td>
                    <td class="" pid-39223-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;3.61B</td>
                    <td class=""right"">7.05B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""39223""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Trex Company Inc"" _p_pid=""24350"" _r_pid=""24350"">
                        <span class=""earnCalCompanyName middle"">Trex</span>&nbsp;(<a href=""/equities/trex-co.-inc-earnings"" class=""bold middle"" target=""_blank"">TREX</a>)
                    </td>
                    <td class="" pid-24350-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.4883</td>
                    <td class="" pid-24350-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;289.32M</td>
                    <td class=""right"">6.94B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""24350""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""GXO Logistics Inc"" _p_pid=""1176154"" _r_pid=""1176154"">
                        <span class=""earnCalCompanyName middle"">GXO Logistics</span>&nbsp;(<a href=""/equities/gxo-logistics-earnings"" class=""bold middle"" target=""_blank"">GXO</a>)
                    </td>
                    <td class="" pid-1176154-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.6918</td>
                    <td class="" pid-1176154-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.52B</td>
                    <td class=""right"">6.88B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1176154""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""BWX Technologies Inc"" _p_pid=""32336"" _r_pid=""32336"">
                        <span class=""earnCalCompanyName middle"">BWX Tech</span>&nbsp;(<a href=""/equities/babcock---wilcox-earnings"" class=""bold middle"" target=""_blank"">BWXT</a>)
                    </td>
                    <td class="" pid-32336-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.6311</td>
                    <td class="" pid-32336-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;574.7M</td>
                    <td class=""right"">6.77B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""32336""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Weatherford International PLC"" _p_pid=""8098"" _r_pid=""8098"">
                        <span class=""earnCalCompanyName middle"">Weatherford</span>&nbsp;(<a href=""/equities/weatherfgord-intl-earnings"" class=""bold middle"" target=""_blank"">WFRD</a>)
                    </td>
                    <td class="" pid-8098-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.45</td>
                    <td class="" pid-8098-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.27B</td>
                    <td class=""right"">6.53B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""8098""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Jiangsu Expressway Co Ltd ADR"" _p_pid=""1050172"" _r_pid=""1050172"">
                        <span class=""earnCalCompanyName middle"">Jiangsu Expressway ADR</span>&nbsp;(<a href=""/equities/jiangsu-expressway-adr-earnings"" class=""bold middle"" target=""_blank"">JEXYY</a>)
                    </td>
                    <td class="" pid-1050172-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.7441</td>
                    <td class="" pid-1050172-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;508.3M</td>
                    <td class=""right"">6.48B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""1050172""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Brixmor Property"" _p_pid=""48377"" _r_pid=""48377"">
                        <span class=""earnCalCompanyName middle"">Brixmor Property</span>&nbsp;(<a href=""/equities/brixmor-property-earnings"" class=""bold middle"" target=""_blank"">BRX</a>)
                    </td>
                    <td class="" pid-48377-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.201</td>
                    <td class="" pid-48377-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;306.94M</td>
                    <td class=""right"">6.43B</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""48377""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Texas Roadhouse Inc"" _p_pid=""17416"" _r_pid=""17416"">
                        <span class=""earnCalCompanyName middle"">Texas Roadhouse</span>&nbsp;(<a href=""/equities/texas-roadhouse-earnings"" class=""bold middle"" target=""_blank"">TXRH</a>)
                    </td>
                    <td class="" pid-17416-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.07</td>
                    <td class="" pid-17416-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.12B</td>
                    <td class=""right"">6.38B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""17416""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Appfolio Inc"" _p_pid=""955558"" _r_pid=""955558"">
                        <span class=""earnCalCompanyName middle"">Appfolio Inc</span>&nbsp;(<a href=""/equities/appfolio-inc-earnings"" class=""bold middle"" target=""_blank"">APPF</a>)
                    </td>
                    <td class="" pid-955558-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.1464</td>
                    <td class="" pid-955558-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;157.19M</td>
                    <td class=""right"">6.37B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""955558""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Transocean Ltd"" _p_pid=""8300"" _r_pid=""8300"">
                        <span class=""earnCalCompanyName middle"">Transocean</span>&nbsp;(<a href=""/equities/transocea-ltd-earnings"" class=""bold middle"" target=""_blank"">RIG</a>)
                    </td>
                    <td class="" pid-8300-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.2111</td>
                    <td class="" pid-8300-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;730.32M</td>
                    <td class=""right"">6.34B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""8300""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Credit Acceptance Corporation"" _p_pid=""15629"" _r_pid=""15629"">
                        <span class=""earnCalCompanyName middle"">Credit Acceptance</span>&nbsp;(<a href=""/equities/credit-acceptance-earnings"" class=""bold middle"" target=""_blank"">CACC</a>)
                    </td>
                    <td class="" pid-15629-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;8.24</td>
                    <td class="" pid-15629-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;476.19M</td>
                    <td class=""right"">6.03B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""15629""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Qantas Airways Ltd ADR"" _p_pid=""1054401"" _r_pid=""1054401"">
                        <span class=""earnCalCompanyName middle"">Qantas Airways ADR</span>&nbsp;(<a href=""/equities/qantas-airways-adr-earnings"" class=""bold middle"" target=""_blank"">QABSY</a>)
                    </td>
                    <td class="" pid-1054401-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-1054401-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">5.98B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""1054401""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Rambus Inc"" _p_pid=""6455"" _r_pid=""6455"">
                        <span class=""earnCalCompanyName middle"">Rambus</span>&nbsp;(<a href=""/equities/rambus-inc-earnings"" class=""bold middle"" target=""_blank"">RMBS</a>)
                    </td>
                    <td class="" pid-6455-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.408</td>
                    <td class="" pid-6455-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;132.02M</td>
                    <td class=""right"">5.86B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""6455""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Clariant AG"" _p_pid=""941819"" _r_pid=""941819"">
                        <span class=""earnCalCompanyName middle"">Clariant AG</span>&nbsp;(<a href=""/equities/clariant-ag-earnings"" class=""bold middle"" target=""_blank"">CLZNY</a>)
                    </td>
                    <td class="" pid-941819-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.3297</td>
                    <td class="" pid-941819-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.27B</td>
                    <td class=""right"">5.31B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""941819""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Timken Company"" _p_pid=""39303"" _r_pid=""39303"">
                        <span class=""earnCalCompanyName middle"">Timken</span>&nbsp;(<a href=""/equities/timken-co-earnings"" class=""bold middle"" target=""_blank"">TKR</a>)
                    </td>
                    <td class="" pid-39303-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.6</td>
                    <td class="" pid-39303-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.2B</td>
                    <td class=""right"">5.13B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""39303""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Nedbank Group"" _p_pid=""942066"" _r_pid=""942066"">
                        <span class=""earnCalCompanyName middle"">Nedbank Group Ltd</span>&nbsp;(<a href=""/equities/nedbank-group-ltd-pk-earnings"" class=""bold middle"" target=""_blank"">NDBKY</a>)
                    </td>
                    <td class="" pid-942066-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-942066-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">5.1B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""942066""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Kite Realty Group Trust"" _p_pid=""20539"" _r_pid=""20539"">
                        <span class=""earnCalCompanyName middle"">Kite Realty</span>&nbsp;(<a href=""/equities/kite-realty-group-trust-earnings"" class=""bold middle"" target=""_blank"">KRG</a>)
                    </td>
                    <td class="" pid-20539-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.0107</td>
                    <td class="" pid-20539-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;191.52M</td>
                    <td class=""right"">4.78B</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20539""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Affiliated Managers Group Inc"" _p_pid=""20280"" _r_pid=""20280"">
                        <span class=""earnCalCompanyName middle"">Affiliated Managers</span>&nbsp;(<a href=""/equities/affiliated-managers-group-inc-earnings"" class=""bold middle"" target=""_blank"">AMG</a>)
                    </td>
                    <td class="" pid-20280-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;3.82</td>
                    <td class="" pid-20280-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;515.3M</td>
                    <td class=""right"">4.64B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20280""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Vornado Realty Trust"" _p_pid=""7863"" _r_pid=""7863"">
                        <span class=""earnCalCompanyName middle"">Vornado</span>&nbsp;(<a href=""/equities/vornado-realty-earnings"" class=""bold middle"" target=""_blank"">VNO</a>)
                    </td>
                    <td class="" pid-7863-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.1557</td>
                    <td class="" pid-7863-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;450.36M</td>
                    <td class=""right"">4.63B</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""7863""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Banco Comercial Portugues SA ADR"" _p_pid=""1053859"" _r_pid=""1053859"">
                        <span class=""earnCalCompanyName middle"">Banco Comercial Portugues ADR</span>&nbsp;(<a href=""/equities/banco-comercial-portugues-adr-earnings"" class=""bold middle"" target=""_blank"">BPCGY</a>)
                    </td>
                    <td class="" pid-1053859-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.1651</td>
                    <td class="" pid-1053859-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.02B</td>
                    <td class=""right"">4.23B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""1053859""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""One Gas Inc"" _p_pid=""101884"" _r_pid=""101884"">
                        <span class=""earnCalCompanyName middle"">One Gas Inc</span>&nbsp;(<a href=""/equities/one-gas-inc-earnings"" class=""bold middle"" target=""_blank"">OGS</a>)
                    </td>
                    <td class="" pid-101884-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.436</td>
                    <td class="" pid-101884-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;391.77M</td>
                    <td class=""right"">4.16B</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""101884""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Boise Cascad Llc"" _p_pid=""41197"" _r_pid=""41197"">
                        <span class=""earnCalCompanyName middle"">Boise Cascad Llc</span>&nbsp;(<a href=""/equities/boise-cascad-llc-earnings"" class=""bold middle"" target=""_blank"">BCC</a>)
                    </td>
                    <td class="" pid-41197-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;3.39</td>
                    <td class="" pid-41197-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.84B</td>
                    <td class=""right"">3.95B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""41197""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Blackbaud Inc"" _p_pid=""15574"" _r_pid=""15574"">
                        <span class=""earnCalCompanyName middle"">Blackbaud</span>&nbsp;(<a href=""/equities/blackbaud-earnings"" class=""bold middle"" target=""_blank"">BLKB</a>)
                    </td>
                    <td class="" pid-15574-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.97</td>
                    <td class="" pid-15574-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;274.06M</td>
                    <td class=""right"">3.79B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""15574""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Essential Properties Realty Trust Inc"" _p_pid=""1081673"" _r_pid=""1081673"">
                        <span class=""earnCalCompanyName middle"">Essential Properties</span>&nbsp;(<a href=""/equities/essential-properties-realty-trust-earnings"" class=""bold middle"" target=""_blank"">EPRT</a>)
                    </td>
                    <td class="" pid-1081673-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.27</td>
                    <td class="" pid-1081673-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;85.59M</td>
                    <td class=""right"">3.76B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1081673""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Black Stone Minerals LP"" _p_pid=""953041"" _r_pid=""953041"">
                        <span class=""earnCalCompanyName middle"">Black Stone Minerals</span>&nbsp;(<a href=""/equities/black-stone-minerals-earnings"" class=""bold middle"" target=""_blank"">BSM</a>)
                    </td>
                    <td class="" pid-953041-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.4325</td>
                    <td class="" pid-953041-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;132.24M</td>
                    <td class=""right"">3.65B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""953041""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Insperity Inc"" _p_pid=""21037"" _r_pid=""21037"">
                        <span class=""earnCalCompanyName middle"">Insperity</span>&nbsp;(<a href=""/equities/insperity-inc-earnings"" class=""bold middle"" target=""_blank"">NSP</a>)
                    </td>
                    <td class="" pid-21037-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.8625</td>
                    <td class="" pid-21037-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.54B</td>
                    <td class=""right"">3.63B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""21037""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""CTEEP Companhia de Transmissao de Energia Eletrica Paulista ADR"" _p_pid=""1053960"" _r_pid=""1053960"">
                        <span class=""earnCalCompanyName middle"">CTEEP ADR</span>&nbsp;(<a href=""/equities/cteep-adr-earnings"" class=""bold middle"" target=""_blank"">CTPTY</a>)
                    </td>
                    <td class="" pid-1053960-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.1276</td>
                    <td class="" pid-1053960-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;242.08M</td>
                    <td class=""right"">3.58B</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""1053960""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""CVR Energy Inc"" _p_pid=""20985"" _r_pid=""20985"">
                        <span class=""earnCalCompanyName middle"">CVR Energy</span>&nbsp;(<a href=""/equities/cvr-energy-inc-earnings"" class=""bold middle"" target=""_blank"">CVI</a>)
                    </td>
                    <td class="" pid-20985-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.69</td>
                    <td class="" pid-20985-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.38B</td>
                    <td class=""right"">3.4B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20985""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Varonis Systems"" _p_pid=""100233"" _r_pid=""100233"">
                        <span class=""earnCalCompanyName middle"">Varonis Systems</span>&nbsp;(<a href=""/equities/varonis-systems-earnings"" class=""bold middle"" target=""_blank"">VRNS</a>)
                    </td>
                    <td class="" pid-100233-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.0262</td>
                    <td class="" pid-100233-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;133.4M</td>
                    <td class=""right"">3.35B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""100233""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Leggett &amp; Platt Incorporated"" _p_pid=""8023"" _r_pid=""8023"">
                        <span class=""earnCalCompanyName middle"">Leggett&amp;Platt</span>&nbsp;(<a href=""/equities/leggett---platt-earnings"" class=""bold middle"" target=""_blank"">LEG</a>)
                    </td>
                    <td class="" pid-8023-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.4075</td>
                    <td class="" pid-8023-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.27B</td>
                    <td class=""right"">3.3B</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""8023""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Otter Tail Corporation"" _p_pid=""16846"" _r_pid=""16846"">
                        <span class=""earnCalCompanyName middle"">Otter Tail</span>&nbsp;(<a href=""/equities/otter-tail-corp-earnings"" class=""bold middle"" target=""_blank"">OTTR</a>)
                    </td>
                    <td class="" pid-16846-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.52</td>
                    <td class="" pid-16846-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;325.59M</td>
                    <td class=""right"">3.21B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16846""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Boral Ltd ADR"" _p_pid=""1053855"" _r_pid=""1053855"">
                        <span class=""earnCalCompanyName middle"">Boral ADR</span>&nbsp;(<a href=""/equities/boral-adr-earnings"" class=""bold middle"" target=""_blank"">BOALY</a>)
                    </td>
                    <td class="" pid-1053855-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-1053855-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">3.1B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""1053855""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Sanmina Corporation"" _p_pid=""17110"" _r_pid=""17110"">
                        <span class=""earnCalCompanyName middle"">Sanmina</span>&nbsp;(<a href=""/equities/sanmina-sci-corp-earnings"" class=""bold middle"" target=""_blank"">SANM</a>)
                    </td>
                    <td class="" pid-17110-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.52</td>
                    <td class="" pid-17110-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.23B</td>
                    <td class=""right"">3.01B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""17110""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""American States Water Company"" _p_pid=""20391"" _r_pid=""20391"">
                        <span class=""earnCalCompanyName middle"">American States Water</span>&nbsp;(<a href=""/equities/american-states-water-comp-earnings"" class=""bold middle"" target=""_blank"">AWR</a>)
                    </td>
                    <td class="" pid-20391-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.8914</td>
                    <td class="" pid-20391-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;152M</td>
                    <td class=""right"">2.98B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20391""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Capstone Mining Corp"" _p_pid=""24505"" _r_pid=""1195630"">
                        <span class=""earnCalCompanyName middle"">Capstone Mining</span>&nbsp;(<a href=""/equities/capstone-mining-corp-earnings?cid=1195630"" class=""bold middle"" target=""_blank"">CSCCF</a>)
                    </td>
                    <td class="" pid-1195630-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.0266</td>
                    <td class="" pid-1195630-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;362.83M</td>
                    <td class=""right"">2.9B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""1195630""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Comstock Resources Inc"" _p_pid=""20570"" _r_pid=""20570"">
                        <span class=""earnCalCompanyName middle"">Comstock Resources</span>&nbsp;(<a href=""/equities/comstock-resources-inc-earnings"" class=""bold middle"" target=""_blank"">CRK</a>)
                    </td>
                    <td class="" pid-20570-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.0972</td>
                    <td class="" pid-20570-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;366.71M</td>
                    <td class=""right"">2.88B</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20570""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""NSK Ltd ADR"" _p_pid=""942082"" _r_pid=""942082"">
                        <span class=""earnCalCompanyName middle"">NSK ADR</span>&nbsp;(<a href=""/equities/nsk-ltd-earnings"" class=""bold middle"" target=""_blank"">NPSKY</a>)
                    </td>
                    <td class="" pid-942082-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-942082-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.34B</td>
                    <td class=""right"">2.85B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""942082""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Alliance Resource Partners LP"" _p_pid=""15449"" _r_pid=""15449"">
                        <span class=""earnCalCompanyName middle"">Alliance Resource</span>&nbsp;(<a href=""/equities/alliance-resource-earnings"" class=""bold middle"" target=""_blank"">ARLP</a>)
                    </td>
                    <td class="" pid-15449-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.34</td>
                    <td class="" pid-15449-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;674.62M</td>
                    <td class=""right"">2.74B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""15449""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""PriceSmart Inc"" _p_pid=""16976"" _r_pid=""16976"">
                        <span class=""earnCalCompanyName middle"">PriceSmart</span>&nbsp;(<a href=""/equities/pricesmart-earnings"" class=""bold middle"" target=""_blank"">PSMT</a>)
                    </td>
                    <td class="" pid-16976-2023-10-30-082023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.798</td>
                    <td class="" pid-16976-2023-10-30-082023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.09B</td>
                    <td class=""right"">2.29B</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16976""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Compania de Minas Buenaventura SAA ADR"" _p_pid=""32318"" _r_pid=""32318"">
                        <span class=""earnCalCompanyName middle"">Buenaventura Mining ADR</span>&nbsp;(<a href=""/equities/buenaventura-mining-earnings"" class=""bold middle"" target=""_blank"">BVN</a>)
                    </td>
                    <td class="" pid-32318-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.11</td>
                    <td class="" pid-32318-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">2.17B</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""32318""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Beijing Capital International Airport Co Ltd ADR"" _p_pid=""1053838"" _r_pid=""1053838"">
                        <span class=""earnCalCompanyName middle"">Beijing Capital Airport ADR</span>&nbsp;(<a href=""/equities/beijing-capital-airport-adr-earnings"" class=""bold middle"" target=""_blank"">BJCHY</a>)
                    </td>
                    <td class="" pid-1053838-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-1053838-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">2.14B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""1053838""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Alps Electric Co Ltd"" _p_pid=""1010666"" _r_pid=""1010666"">
                        <span class=""earnCalCompanyName middle"">Alps Electric</span>&nbsp;(<a href=""/equities/alps-electric-co-ltd-earnings"" class=""bold middle"" target=""_blank"">APELY</a>)
                    </td>
                    <td class="" pid-1010666-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.4639</td>
                    <td class="" pid-1010666-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.74B</td>
                    <td class=""right"">1.8B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""1010666""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Mercury General Corporation"" _p_pid=""20700"" _r_pid=""20700"">
                        <span class=""earnCalCompanyName middle"">Mercury General</span>&nbsp;(<a href=""/equities/mercury-general-corp-earnings"" class=""bold middle"" target=""_blank"">MCY</a>)
                    </td>
                    <td class="" pid-20700-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.01</td>
                    <td class="" pid-20700-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.06B</td>
                    <td class=""right"">1.57B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20700""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Kiniksa Pharmaceuticals Ltd"" _p_pid=""1075232"" _r_pid=""1075232"">
                        <span class=""earnCalCompanyName middle"">Kiniksa Pharma</span>&nbsp;(<a href=""/equities/kiniksa-pharmaceuticals-ltd-earnings"" class=""bold middle"" target=""_blank"">KNSA</a>)
                    </td>
                    <td class="" pid-1075232-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.1566</td>
                    <td class="" pid-1075232-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;62.85M</td>
                    <td class=""right"">1.17B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1075232""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Harmonic Inc"" _p_pid=""16273"" _r_pid=""16273"">
                        <span class=""earnCalCompanyName middle"">Harmonic</span>&nbsp;(<a href=""/equities/harmonic-inc-earnings"" class=""bold middle"" target=""_blank"">HLIT</a>)
                    </td>
                    <td class="" pid-16273-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.0067</td>
                    <td class="" pid-16273-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;134.5M</td>
                    <td class=""right"">1.06B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16273""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""TriCo Bancshares"" _p_pid=""17322"" _r_pid=""17322"">
                        <span class=""earnCalCompanyName middle"">TriCo</span>&nbsp;(<a href=""/equities/trico-bancshares-earnings"" class=""bold middle"" target=""_blank"">TCBK</a>)
                    </td>
                    <td class="" pid-17322-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.89</td>
                    <td class="" pid-17322-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;103.19M</td>
                    <td class=""right"">1.04B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""17322""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Astra Agro Lestari TBK"" _p_pid=""941749"" _r_pid=""941749"">
                        <span class=""earnCalCompanyName middle"">Astra Agro Lestari TBK</span>&nbsp;(<a href=""/equities/astra-agro-lestari-tbk-earnings"" class=""bold middle"" target=""_blank"">AAGRY</a>)
                    </td>
                    <td class="" pid-941749-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-941749-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">946.14M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""941749""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Alexanders Inc"" _p_pid=""20978"" _r_pid=""20978"">
                        <span class=""earnCalCompanyName middle"">Alexanders</span>&nbsp;(<a href=""/equities/alexanders-inc-earnings"" class=""bold middle"" target=""_blank"">ALX</a>)
                    </td>
                    <td class="" pid-20978-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-20978-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;52.9M</td>
                    <td class=""right"">941.99M</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20978""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Tetragon Financial Group Ltd"" _p_pid=""14164"" _r_pid=""22191"">
                        <span class=""earnCalCompanyName middle"">Tetragon</span>&nbsp;(<a href=""/equities/tetragon-fin-group-earnings?cid=22191"" class=""bold middle"" target=""_blank"">TGONF</a>)
                    </td>
                    <td class="" pid-22191-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-22191-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">909.49M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""22191""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""German American Bancorp Inc"" _p_pid=""16151"" _r_pid=""16151"">
                        <span class=""earnCalCompanyName middle"">German American Bancorp</span>&nbsp;(<a href=""/equities/german-american-b-earnings"" class=""bold middle"" target=""_blank"">GABC</a>)
                    </td>
                    <td class="" pid-16151-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.69</td>
                    <td class="" pid-16151-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;63.53M</td>
                    <td class=""right"">804.98M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16151""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Lsb Industries Inc"" _p_pid=""20831"" _r_pid=""20831"">
                        <span class=""earnCalCompanyName middle"">Lsb Industries</span>&nbsp;(<a href=""/equities/lsb-industries-inc-earnings"" class=""bold middle"" target=""_blank"">LXU</a>)
                    </td>
                    <td class="" pid-20831-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.0756</td>
                    <td class="" pid-20831-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;108.13M</td>
                    <td class=""right"">744.19M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20831""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Douglas Dynamics Inc"" _p_pid=""20713"" _r_pid=""20713"">
                        <span class=""earnCalCompanyName middle"">Douglas Dynamics</span>&nbsp;(<a href=""/equities/douglas-dynamics-inc-earnings"" class=""bold middle"" target=""_blank"">PLOW</a>)
                    </td>
                    <td class="" pid-20713-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.6533</td>
                    <td class="" pid-20713-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;172.4M</td>
                    <td class=""right"">690.9M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20713""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Loandepot Inc"" _p_pid=""1169367"" _r_pid=""1169367"">
                        <span class=""earnCalCompanyName middle"">Loandepot</span>&nbsp;(<a href=""/equities/loandepot-inc-earnings"" class=""bold middle"" target=""_blank"">LDI</a>)
                    </td>
                    <td class="" pid-1169367-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.0841</td>
                    <td class="" pid-1169367-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;263.63M</td>
                    <td class=""right"">574.96M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1169367""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Oil States International Inc"" _p_pid=""20335"" _r_pid=""20335"">
                        <span class=""earnCalCompanyName middle"">Oil States</span>&nbsp;(<a href=""/equities/oil-states-international-inc-earnings"" class=""bold middle"" target=""_blank"">OIS</a>)
                    </td>
                    <td class="" pid-20335-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.0425</td>
                    <td class="" pid-20335-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;189.23M</td>
                    <td class=""right"">526.56M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20335""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Heidrick &amp; Struggles International"" _p_pid=""16295"" _r_pid=""16295"">
                        <span class=""earnCalCompanyName middle"">Heidrick&amp;Struggles</span>&nbsp;(<a href=""/equities/heidrick---strugg-earnings"" class=""bold middle"" target=""_blank"">HSII</a>)
                    </td>
                    <td class="" pid-16295-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.68</td>
                    <td class="" pid-16295-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;250.62M</td>
                    <td class=""right"">504.8M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16295""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Diamond Hill Investment Group Inc"" _p_pid=""15910"" _r_pid=""15910"">
                        <span class=""earnCalCompanyName middle"">Diamond Hill</span>&nbsp;(<a href=""/equities/diamond-hill-inve-earnings"" class=""bold middle"" target=""_blank"">DHIL</a>)
                    </td>
                    <td class="" pid-15910-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-15910-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">477.44M</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""15910""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Orthofix Medical Inc"" _p_pid=""16797"" _r_pid=""16797"">
                        <span class=""earnCalCompanyName middle"">Orthofix</span>&nbsp;(<a href=""/equities/orthofix-internat-earnings"" class=""bold middle"" target=""_blank"">OFIX</a>)
                    </td>
                    <td class="" pid-16797-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.2107</td>
                    <td class="" pid-16797-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;185.93M</td>
                    <td class=""right"">459.23M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16797""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""TrueBlue Inc"" _p_pid=""21094"" _r_pid=""21094"">
                        <span class=""earnCalCompanyName middle"">TrueBlue</span>&nbsp;(<a href=""/equities/trueblue-inc-earnings"" class=""bold middle"" target=""_blank"">TBI</a>)
                    </td>
                    <td class="" pid-21094-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.2346</td>
                    <td class="" pid-21094-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;488.9M</td>
                    <td class=""right"">446.16M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""21094""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Northeast Bancorp"" _p_pid=""16708"" _r_pid=""16708"">
                        <span class=""earnCalCompanyName middle"">Northeast Bancorp</span>&nbsp;(<a href=""/equities/northeast-bancorp-earnings"" class=""bold middle"" target=""_blank"">NBN</a>)
                    </td>
                    <td class="" pid-16708-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.55</td>
                    <td class="" pid-16708-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;33.97M</td>
                    <td class=""right"">335.28M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16708""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Sterling Bancorp Inc"" _p_pid=""1055091"" _r_pid=""1055091"">
                        <span class=""earnCalCompanyName middle"">Sterling Bancorp</span>&nbsp;(<a href=""/equities/sterling-bancorp-inc-earnings"" class=""bold middle"" target=""_blank"">SBT</a>)
                    </td>
                    <td class="" pid-1055091-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.05</td>
                    <td class="" pid-1055091-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;17.28M</td>
                    <td class=""right"">301.52M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1055091""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""MVB Financial Corp"" _p_pid=""1056239"" _r_pid=""1056239"">
                        <span class=""earnCalCompanyName middle"">MVB Financial</span>&nbsp;(<a href=""/equities/mvb-financial-earnings"" class=""bold middle"" target=""_blank"">MVBF</a>)
                    </td>
                    <td class="" pid-1056239-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.6025</td>
                    <td class="" pid-1056239-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;37.07M</td>
                    <td class=""right"">276.27M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1056239""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Northrim BanCorp Inc"" _p_pid=""16752"" _r_pid=""16752"">
                        <span class=""earnCalCompanyName middle"">Northrim</span>&nbsp;(<a href=""/equities/northrim-bancorp-earnings"" class=""bold middle"" target=""_blank"">NRIM</a>)
                    </td>
                    <td class="" pid-16752-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.61</td>
                    <td class="" pid-16752-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;27.60M</td>
                    <td class=""right"">216.29M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16752""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Meridian Bank"" _p_pid=""1054834"" _r_pid=""1054834"">
                        <span class=""earnCalCompanyName middle"">Meridian Bank</span>&nbsp;(<a href=""/equities/meridian-bank-earnings"" class=""bold middle"" target=""_blank"">MRBK</a>)
                    </td>
                    <td class="" pid-1054834-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.3827</td>
                    <td class="" pid-1054834-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;26.79M</td>
                    <td class=""right"">115.08M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1054834""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""LINKBANCORP Inc"" _p_pid=""1194054"" _r_pid=""1194054"">
                        <span class=""earnCalCompanyName middle"">LINKBANCORP</span>&nbsp;(<a href=""/equities/linkbancorp-earnings"" class=""bold middle"" target=""_blank"">LNKB</a>)
                    </td>
                    <td class="" pid-1194054-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.13</td>
                    <td class="" pid-1194054-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;13.5M</td>
                    <td class=""right"">113.65M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1194054""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Intevac Inc"" _p_pid=""16409"" _r_pid=""16409"">
                        <span class=""earnCalCompanyName middle"">Intevac</span>&nbsp;(<a href=""/equities/intevac-earnings"" class=""bold middle"" target=""_blank"">IVAC</a>)
                    </td>
                    <td class="" pid-16409-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.14</td>
                    <td class="" pid-16409-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;9.75M</td>
                    <td class=""right"">82.85M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16409""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Permianville Royalty Trust"" _p_pid=""958157"" _r_pid=""958157"">
                        <span class=""earnCalCompanyName middle"">Permianville Royalty</span>&nbsp;(<a href=""/equities/enduro-royalty-trust-earnings"" class=""bold middle"" target=""_blank"">PVL</a>)
                    </td>
                    <td class="" pid-958157-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-958157-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">82.17M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""958157""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Sezzle Inc"" _p_pid=""1142326"" _r_pid=""1204599"">
                        <span class=""earnCalCompanyName middle"">Sezzle</span>&nbsp;(<a href=""/equities/sezzle-inc-earnings?cid=1204599"" class=""bold middle"" target=""_blank"">SEZL</a>)
                    </td>
                    <td class="" pid-1204599-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.38</td>
                    <td class="" pid-1204599-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;35.9M</td>
                    <td class=""right"">79.56M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1204599""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Better Plant Sciences Inc"" _p_pid=""1117878"" _r_pid=""1122203"">
                        <span class=""earnCalCompanyName middle"">Better Plant Sciences Inc</span>&nbsp;(<a href=""/equities/yield-growth-earnings?cid=1122203"" class=""bold middle"" target=""_blank"">VEGGD</a>)
                    </td>
                    <td class="" pid-1122203-2023-10-30-082023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-1122203-2023-10-30-082023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">295.48K</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""1122203""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Banco Espirito Santo S.A."" _p_pid=""941760"" _r_pid=""941760"">
                        <span class=""earnCalCompanyName middle"">Banco Espirito Santo</span>&nbsp;(<a href=""/equities/banco-espirto-santo-e-earnings"" class=""bold middle"" target=""_blank"">BKESY</a>)
                    </td>
                    <td class="" pid-941760-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-941760-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">5.6K</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""941760""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""City Bank"" _p_pid=""52117"" _r_pid=""52117"">
                        <span class=""earnCalCompanyName middle"">City Bank</span>&nbsp;(<a href=""/equities/city-bank-earnings"" class=""bold middle"" target=""_blank"">CTBK</a>)
                    </td>
                    <td class="" pid-52117-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-52117-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">15</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""52117""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Poniard Pharmaceuticals Inc"" _p_pid=""16861"" _r_pid=""16861"">
                        <span class=""earnCalCompanyName middle"">Poniard Pharma</span>&nbsp;(<a href=""/equities/poniard-pharmaceuticals-earnings"" class=""bold middle"" target=""_blank"">PARD</a>)
                    </td>
                    <td class="" pid-16861-2023-10-30-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-16861-2023-10-30-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">1</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""16861""></td>
                </tr>
                    <tr tablesorterdivider="""">
            <td colspan=""9"" class=""theDay"">Tuesday, October 31, 2023</td>
        </tr>
                        <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Mastercard Inc"" _p_pid=""7864"" _r_pid=""7864"">
                        <span class=""earnCalCompanyName middle"">Mastercard</span>&nbsp;(<a href=""/equities/mastercard-cl-a-earnings"" class=""bold middle"" target=""_blank"">MA</a>)
                    </td>
                    <td class="" pid-7864-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;3.21</td>
                    <td class="" pid-7864-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;6.53B</td>
                    <td class=""right"">378.98B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""7864""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Pfizer Inc"" _p_pid=""7989"" _r_pid=""7989"">
                        <span class=""earnCalCompanyName middle"">Pfizer</span>&nbsp;(<a href=""/equities/pfizer-earnings"" class=""bold middle"" target=""_blank"">PFE</a>)
                    </td>
                    <td class="" pid-7989-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.6267</td>
                    <td class="" pid-7989-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;14.89B</td>
                    <td class=""right"">184.57B</td>
                    <td class=""right time"" data-value=""1""><span class=""marketOpen genToolTip oneliner reverseToolTip"" data-tooltip=""Before market open""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""7989""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Advanced Micro Devices Inc"" _p_pid=""8274"" _r_pid=""8274"">
                        <span class=""earnCalCompanyName middle"">AMD</span>&nbsp;(<a href=""/equities/adv-micro-device-earnings"" class=""bold middle"" target=""_blank"">AMD</a>)
                    </td>
                    <td class="" pid-8274-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.6806</td>
                    <td class="" pid-8274-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;5.7B</td>
                    <td class=""right"">155.43B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""8274""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Amgen Inc"" _p_pid=""6466"" _r_pid=""6466"">
                        <span class=""earnCalCompanyName middle"">Amgen</span>&nbsp;(<a href=""/equities/amgen-inc-earnings"" class=""bold middle"" target=""_blank"">AMGN</a>)
                    </td>
                    <td class="" pid-6466-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;4.68</td>
                    <td class="" pid-6466-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;7.02B</td>
                    <td class=""right"">143.19B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""6466""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Anheuser Busch Inbev NV ADR"" _p_pid=""25318"" _r_pid=""25318"">
                        <span class=""earnCalCompanyName middle"">Anheuser Busch ADR</span>&nbsp;(<a href=""/equities/anheuser-busch-exch-earnings"" class=""bold middle"" target=""_blank"">BUD</a>)
                    </td>
                    <td class="" pid-25318-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.8534</td>
                    <td class="" pid-25318-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;15.92B</td>
                    <td class=""right"">112.4B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""25318""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""BP PLC ADR"" _p_pid=""21257"" _r_pid=""21257"">
                        <span class=""earnCalCompanyName middle"">BP ADR</span>&nbsp;(<a href=""/equities/bp-plc-earnings"" class=""bold middle"" target=""_blank"">BP</a>)
                    </td>
                    <td class="" pid-21257-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.3</td>
                    <td class="" pid-21257-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;53.12B</td>
                    <td class=""right"">109.14B</td>
                    <td class=""right time"" data-value=""1""><span class=""marketOpen genToolTip oneliner reverseToolTip"" data-tooltip=""Before market open""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""21257""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Mondelez International Inc"" _p_pid=""32372"" _r_pid=""32372"">
                        <span class=""earnCalCompanyName middle"">Mondelez</span>&nbsp;(<a href=""/equities/mondelez-international-inc-earnings"" class=""bold middle"" target=""_blank"">MDLZ</a>)
                    </td>
                    <td class="" pid-32372-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.7839</td>
                    <td class="" pid-32372-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;8.83B</td>
                    <td class=""right"">95.81B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""32372""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Petroleo Brasileiro Petrobras SA ADR"" _p_pid=""8028"" _r_pid=""8028"">
                        <span class=""earnCalCompanyName middle"">Petroleo Brasileiro Petrobras ADR</span>&nbsp;(<a href=""/equities/petroleo-bras-earnings"" class=""bold middle"" target=""_blank"">PBR</a>)
                    </td>
                    <td class="" pid-8028-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.793</td>
                    <td class="" pid-8028-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;25.04B</td>
                    <td class=""right"">94.61B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""8028""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Petroleo Brasileiro ADR Reptg 2 Pref"" _p_pid=""29595"" _r_pid=""29595"">
                        <span class=""earnCalCompanyName middle"">Petroleo Brasileiro ADR Reptg 2 Pref</span>&nbsp;(<a href=""/equities/petroleo-brasileiro-a-earnings"" class=""bold middle"" target=""_blank"">PBRa</a>)
                    </td>
                    <td class="" pid-29595-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.793</td>
                    <td class="" pid-29595-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;25.04B</td>
                    <td class=""right"">94.61B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""29595""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Eaton Corporation PLC"" _p_pid=""8291"" _r_pid=""8291"">
                        <span class=""earnCalCompanyName middle"">Eaton</span>&nbsp;(<a href=""/equities/eaton-earnings"" class=""bold middle"" target=""_blank"">ETN</a>)
                    </td>
                    <td class="" pid-8291-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.33</td>
                    <td class="" pid-8291-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;5.88B</td>
                    <td class=""right"">84.85B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""8291""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""HCA Holdings Inc"" _p_pid=""20931"" _r_pid=""20931"">
                        <span class=""earnCalCompanyName middle"">HCA</span>&nbsp;(<a href=""/equities/hca-holdings-inc-earnings"" class=""bold middle"" target=""_blank"">HCA</a>)
                    </td>
                    <td class="" pid-20931-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;4.04</td>
                    <td class="" pid-20931-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;15.84B</td>
                    <td class=""right"">68.61B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20931""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Marathon Petroleum Corp"" _p_pid=""13947"" _r_pid=""13947"">
                        <span class=""earnCalCompanyName middle"">Marathon Petroleum</span>&nbsp;(<a href=""/equities/marathon-petroleum-corp.-earnings"" class=""bold middle"" target=""_blank"">MPC</a>)
                    </td>
                    <td class="" pid-13947-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;7.11</td>
                    <td class="" pid-13947-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;37.52B</td>
                    <td class=""right"">61.36B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""13947""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Stellantis NV"" _p_pid=""307"" _r_pid=""941888"">
                        <span class=""earnCalCompanyName middle"">Stellantis NV</span>&nbsp;(<a href=""/equities/fiat-earnings?cid=941888"" class=""bold middle"" target=""_blank"">STLA</a>)
                    </td>
                    <td class="" pid-941888-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-941888-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;49.66B</td>
                    <td class=""right"">60.16B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""941888""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""McKesson Corporation"" _p_pid=""8014"" _r_pid=""8014"">
                        <span class=""earnCalCompanyName middle"">McKesson</span>&nbsp;(<a href=""/equities/mckesson-corp-earnings"" class=""bold middle"" target=""_blank"">MCK</a>)
                    </td>
                    <td class="" pid-8014-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;6.12</td>
                    <td class="" pid-8014-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;75.91B</td>
                    <td class=""right"">59.18B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""8014""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Mitsui &amp; Company Ltd"" _p_pid=""13572"" _r_pid=""13572"">
                        <span class=""earnCalCompanyName middle"">Mitsui &amp; Company</span>&nbsp;(<a href=""/equities/mitsui---company-ltd-earnings"" class=""bold middle"" target=""_blank"">MITSY</a>)
                    </td>
                    <td class="" pid-13572-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;14.55</td>
                    <td class="" pid-13572-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;22.69B</td>
                    <td class=""right"">55.61B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""13572""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""KKR &amp; Co LP"" _p_pid=""41279"" _r_pid=""41279"">
                        <span class=""earnCalCompanyName middle"">KKR &amp; Co</span>&nbsp;(<a href=""/equities/kkr---co-lp-earnings"" class=""bold middle"" target=""_blank"">KKR</a>)
                    </td>
                    <td class="" pid-41279-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.8123</td>
                    <td class="" pid-41279-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.33B</td>
                    <td class=""right"">53.31B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""41279""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Ecolab Inc"" _p_pid=""8339"" _r_pid=""8339"">
                        <span class=""earnCalCompanyName middle"">Ecolab</span>&nbsp;(<a href=""/equities/ecolab-inc-earnings"" class=""bold middle"" target=""_blank"">ECL</a>)
                    </td>
                    <td class="" pid-8339-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.52</td>
                    <td class="" pid-8339-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;4B</td>
                    <td class=""right"">49.02B</td>
                    <td class=""right time"" data-value=""1""><span class=""marketOpen genToolTip oneliner reverseToolTip"" data-tooltip=""Before market open""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""8339""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Banco Bilbao Viscaya Argentaria SA ADR"" _p_pid=""20173"" _r_pid=""20173"">
                        <span class=""earnCalCompanyName middle"">BBVA ADR</span>&nbsp;(<a href=""/equities/banco-bilbao-viscaya-argentaria-sa-earnings"" class=""bold middle"" target=""_blank"">BBVA</a>)
                    </td>
                    <td class="" pid-20173-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.3933</td>
                    <td class="" pid-20173-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;8.1B</td>
                    <td class=""right"">47.38B</td>
                    <td class=""right time"" data-value=""1""><span class=""marketOpen genToolTip oneliner reverseToolTip"" data-tooltip=""Before market open""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20173""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Public Storage"" _p_pid=""8158"" _r_pid=""8158"">
                        <span class=""earnCalCompanyName middle"">Public Storage</span>&nbsp;(<a href=""/equities/public-stg-mld-earnings"" class=""bold middle"" target=""_blank"">PSA</a>)
                    </td>
                    <td class="" pid-8158-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.84</td>
                    <td class="" pid-8158-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.15B</td>
                    <td class=""right"">46.55B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""8158""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Ambev SA ADR"" _p_pid=""39315"" _r_pid=""39315"">
                        <span class=""earnCalCompanyName middle"">Ambev SA</span>&nbsp;(<a href=""/equities/ambev-prf-adr-earnings"" class=""bold middle"" target=""_blank"">ABEV</a>)
                    </td>
                    <td class="" pid-39315-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.043</td>
                    <td class="" pid-39315-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;4.38B</td>
                    <td class=""right"">41.85B</td>
                    <td class=""right time"" data-value=""1""><span class=""marketOpen genToolTip oneliner reverseToolTip"" data-tooltip=""Before market open""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""39315""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Simon Property Group Inc"" _p_pid=""8210"" _r_pid=""8210"">
                        <span class=""earnCalCompanyName middle"">Simon Property</span>&nbsp;(<a href=""/equities/simon-prop-grp-earnings"" class=""bold middle"" target=""_blank"">SPG</a>)
                    </td>
                    <td class="" pid-8210-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.52</td>
                    <td class="" pid-8210-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.27B</td>
                    <td class=""right"">41.81B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""8210""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Japan Tobacco ADR"" _p_pid=""1053132"" _r_pid=""1053132"">
                        <span class=""earnCalCompanyName middle"">Japan Tobacco ADR</span>&nbsp;(<a href=""/equities/japan-tobacco-adr-earnings"" class=""bold middle"" target=""_blank"">JAPAY</a>)
                    </td>
                    <td class="" pid-1053132-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.2718</td>
                    <td class="" pid-1053132-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;5.03B</td>
                    <td class=""right"">40.99B</td>
                    <td class=""right time"" data-value=""1""><span class=""marketOpen genToolTip oneliner reverseToolTip"" data-tooltip=""Before market open""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""1053132""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""BASF SE ADR"" _p_pid=""941769"" _r_pid=""941769"">
                        <span class=""earnCalCompanyName middle"">BASF ADR</span>&nbsp;(<a href=""/equities/basf-se-pk-earnings"" class=""bold middle"" target=""_blank"">BASFY</a>)
                    </td>
                    <td class="" pid-941769-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.1312</td>
                    <td class="" pid-941769-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;19.42B</td>
                    <td class=""right"">40.95B</td>
                    <td class=""right time"" data-value=""1""><span class=""marketOpen genToolTip oneliner reverseToolTip"" data-tooltip=""Before market open""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""941769""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Hoya Corp"" _p_pid=""941942"" _r_pid=""941942"">
                        <span class=""earnCalCompanyName middle"">Hoya Corp</span>&nbsp;(<a href=""/equities/hoya-corp-earnings"" class=""bold middle"" target=""_blank"">HOCPY</a>)
                    </td>
                    <td class="" pid-941942-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.8524</td>
                    <td class="" pid-941942-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.31B</td>
                    <td class=""right"">36.8B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""941942""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""IDEXX Laboratories Inc"" _p_pid=""16336"" _r_pid=""16336"">
                        <span class=""earnCalCompanyName middle"">IDEXX Labs</span>&nbsp;(<a href=""/equities/idexx-laboratorie-earnings"" class=""bold middle"" target=""_blank"">IDXX</a>)
                    </td>
                    <td class="" pid-16336-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.37</td>
                    <td class="" pid-16336-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;931.19M</td>
                    <td class=""right"">36.17B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16336""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Verisk Analytics Inc"" _p_pid=""39340"" _r_pid=""39340"">
                        <span class=""earnCalCompanyName middle"">Verisk</span>&nbsp;(<a href=""/equities/verisk-analytics-inc-earnings"" class=""bold middle"" target=""_blank"">VRSK</a>)
                    </td>
                    <td class="" pid-39340-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.47</td>
                    <td class="" pid-39340-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;661.56M</td>
                    <td class=""right"">35.09B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""39340""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""MPLX LP"" _p_pid=""958154"" _r_pid=""958154"">
                        <span class=""earnCalCompanyName middle"">MPLX LP</span>&nbsp;(<a href=""/equities/mplx-lp-earnings"" class=""bold middle"" target=""_blank"">MPLX</a>)
                    </td>
                    <td class="" pid-958154-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.9302</td>
                    <td class="" pid-958154-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.73B</td>
                    <td class=""right"">35.07B</td>
                    <td class=""right time"" data-value=""1""><span class=""marketOpen genToolTip oneliner reverseToolTip"" data-tooltip=""Before market open""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""958154""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Prudential Financial Inc"" _p_pid=""8191"" _r_pid=""8191"">
                        <span class=""earnCalCompanyName middle"">Prudential Financial</span>&nbsp;(<a href=""/equities/prudential-fin-earnings"" class=""bold middle"" target=""_blank"">PRU</a>)
                    </td>
                    <td class="" pid-8191-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;3.24</td>
                    <td class="" pid-8191-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;12.71B</td>
                    <td class=""right"">35.04B</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""8191""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Cummins Inc"" _p_pid=""8218"" _r_pid=""8218"">
                        <span class=""earnCalCompanyName middle"">Cummins</span>&nbsp;(<a href=""/equities/cummins-inc-earnings"" class=""bold middle"" target=""_blank"">CMI</a>)
                    </td>
                    <td class="" pid-8218-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;4.81</td>
                    <td class="" pid-8218-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;8.23B</td>
                    <td class=""right"">32.69B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""8218""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Global Payments Inc"" _p_pid=""39277"" _r_pid=""39277"">
                        <span class=""earnCalCompanyName middle"">Global Payments</span>&nbsp;(<a href=""/equities/global-payments-earnings"" class=""bold middle"" target=""_blank"">GPN</a>)
                    </td>
                    <td class="" pid-39277-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.71</td>
                    <td class="" pid-39277-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.23B</td>
                    <td class=""right"">30.98B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""39277""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Public Service Enterprise Group Inc"" _p_pid=""7893"" _r_pid=""7893"">
                        <span class=""earnCalCompanyName middle"">Public Service Enterprise</span>&nbsp;(<a href=""/equities/publ-svc-enter-earnings"" class=""bold middle"" target=""_blank"">PEG</a>)
                    </td>
                    <td class="" pid-7893-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.772</td>
                    <td class="" pid-7893-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.19B</td>
                    <td class=""right"">30.21B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""7893""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Arch Capital Group Ltd"" _p_pid=""39321"" _r_pid=""39321"">
                        <span class=""earnCalCompanyName middle"">Arch Capital</span>&nbsp;(<a href=""/equities/arch-capital-group-earnings"" class=""bold middle"" target=""_blank"">ACGL</a>)
                    </td>
                    <td class="" pid-39321-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.3</td>
                    <td class="" pid-39321-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;3.25B</td>
                    <td class=""right"">30.02B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""39321""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""argenx NV ADR"" _p_pid=""1010655"" _r_pid=""1010655"">
                        <span class=""earnCalCompanyName middle"">argenx ADR</span>&nbsp;(<a href=""/equities/argenx-earnings"" class=""bold middle"" target=""_blank"">ARGX</a>)
                    </td>
                    <td class="" pid-1010655-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-1.45</td>
                    <td class="" pid-1010655-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;308.98M</td>
                    <td class=""right"">29.76B</td>
                    <td class=""right time"" data-value=""1""><span class=""marketOpen genToolTip oneliner reverseToolTip"" data-tooltip=""Before market open""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1010655""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""ONEOK Inc"" _p_pid=""13858"" _r_pid=""13858"">
                        <span class=""earnCalCompanyName middle"">ONEOK</span>&nbsp;(<a href=""/equities/oneok-earnings"" class=""bold middle"" target=""_blank"">OKE</a>)
                    </td>
                    <td class="" pid-13858-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.08</td>
                    <td class="" pid-13858-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;4.44B</td>
                    <td class=""right"">29.63B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""13858""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""BOC Hong Kong Holdings Ltd ADR"" _p_pid=""941782"" _r_pid=""941782"">
                        <span class=""earnCalCompanyName middle"">BOC Hong Kong ADR</span>&nbsp;(<a href=""/equities/boc-hong-kong-holdings-ltd-earnings"" class=""bold middle"" target=""_blank"">BHKLY</a>)
                    </td>
                    <td class="" pid-941782-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-941782-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">29.07B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""941782""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""WEC Energy Group Inc"" _p_pid=""13867"" _r_pid=""13867"">
                        <span class=""earnCalCompanyName middle"">WEC Energy</span>&nbsp;(<a href=""/equities/wisconsin-energy-corp-earnings"" class=""bold middle"" target=""_blank"">WEC</a>)
                    </td>
                    <td class="" pid-13867-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.01</td>
                    <td class="" pid-13867-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.13B</td>
                    <td class=""right"">26.95B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""13867""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Edison International"" _p_pid=""7998"" _r_pid=""7998"">
                        <span class=""earnCalCompanyName middle"">Edison</span>&nbsp;(<a href=""/equities/edison-intl-earnings"" class=""bold middle"" target=""_blank"">EIX</a>)
                    </td>
                    <td class="" pid-7998-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.55</td>
                    <td class="" pid-7998-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;5.75B</td>
                    <td class=""right"">26.09B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""7998""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Martin Marietta Materials Inc"" _p_pid=""20299"" _r_pid=""20299"">
                        <span class=""earnCalCompanyName middle"">Martin Marietta Materials</span>&nbsp;(<a href=""/equities/martin-marietta-materials-inc-earnings"" class=""bold middle"" target=""_blank"">MLM</a>)
                    </td>
                    <td class="" pid-20299-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;6.03</td>
                    <td class="" pid-20299-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2B</td>
                    <td class=""right"">25.12B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20299""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Ecopetrol SA ADR"" _p_pid=""32338"" _r_pid=""32338"">
                        <span class=""earnCalCompanyName middle"">Ecopetrol ADR</span>&nbsp;(<a href=""/equities/ecopetrol-sa-adr-earnings"" class=""bold middle"" target=""_blank"">EC</a>)
                    </td>
                    <td class="" pid-32338-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2056.44</td>
                    <td class="" pid-32338-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;32,760.84B</td>
                    <td class=""right"">24.3B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""32338""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Yum China Holdings Inc"" _p_pid=""992964"" _r_pid=""992964"">
                        <span class=""earnCalCompanyName middle"">Yum China Holdings</span>&nbsp;(<a href=""/equities/yum-china-holdings-inc-earnings"" class=""bold middle"" target=""_blank"">YUMC</a>)
                    </td>
                    <td class="" pid-992964-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.6639</td>
                    <td class="" pid-992964-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;3.12B</td>
                    <td class=""right"">23.82B</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""992964""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Xylem Inc"" _p_pid=""32532"" _r_pid=""32532"">
                        <span class=""earnCalCompanyName middle"">Xylem</span>&nbsp;(<a href=""/equities/xylem-earnings"" class=""bold middle"" target=""_blank"">XYL</a>)
                    </td>
                    <td class="" pid-32532-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.8826</td>
                    <td class="" pid-32532-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2B</td>
                    <td class=""right"">21.89B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""32532""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Straumann Holding AG ADR"" _p_pid=""1054431"" _r_pid=""1054431"">
                        <span class=""earnCalCompanyName middle"">Straumann ADR</span>&nbsp;(<a href=""/equities/straumann-adr-earnings"" class=""bold middle"" target=""_blank"">SAUHY</a>)
                    </td>
                    <td class="" pid-1054431-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-1054431-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;636.6M</td>
                    <td class=""right"">21.79B</td>
                    <td class=""right time"" data-value=""1""><span class=""marketOpen genToolTip oneliner reverseToolTip"" data-tooltip=""Before market open""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""1054431""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Markel Corporation"" _p_pid=""21127"" _r_pid=""21127"">
                        <span class=""earnCalCompanyName middle"">Markel</span>&nbsp;(<a href=""/equities/markel-corp-earnings"" class=""bold middle"" target=""_blank"">MKL</a>)
                    </td>
                    <td class="" pid-21127-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;19.52</td>
                    <td class="" pid-21127-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;3.72B</td>
                    <td class=""right"">20.24B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""21127""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Carlsberg AS"" _p_pid=""941794"" _r_pid=""941794"">
                        <span class=""earnCalCompanyName middle"">Carlsberg AS</span>&nbsp;(<a href=""/equities/carlsberg-as-earnings"" class=""bold middle"" target=""_blank"">CABGY</a>)
                    </td>
                    <td class="" pid-941794-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.582</td>
                    <td class="" pid-941794-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;3.11B</td>
                    <td class=""right"">19.59B</td>
                    <td class=""right time"" data-value=""1""><span class=""marketOpen genToolTip oneliner reverseToolTip"" data-tooltip=""Before market open""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""941794""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Westinghouse Air Brake Technologies Corp"" _p_pid=""29723"" _r_pid=""29723"">
                        <span class=""earnCalCompanyName middle"">Westinghouse Air Brake</span>&nbsp;(<a href=""/equities/westinghouse-air-brake-tech-earnings"" class=""bold middle"" target=""_blank"">WAB</a>)
                    </td>
                    <td class="" pid-29723-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.46</td>
                    <td class="" pid-29723-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.41B</td>
                    <td class=""right"">19.02B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""29723""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""UCB SA ADR"" _p_pid=""942264"" _r_pid=""942264"">
                        <span class=""earnCalCompanyName middle"">UCB ADR</span>&nbsp;(<a href=""/equities/ucb-sa-earnings"" class=""bold middle"" target=""_blank"">UCBJY</a>)
                    </td>
                    <td class="" pid-942264-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-942264-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.92B</td>
                    <td class=""right"">16.59B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""942264""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Waters Corporation"" _p_pid=""7881"" _r_pid=""7881"">
                        <span class=""earnCalCompanyName middle"">Waters</span>&nbsp;(<a href=""/equities/waters-corp-earnings"" class=""bold middle"" target=""_blank"">WAT</a>)
                    </td>
                    <td class="" pid-7881-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.58</td>
                    <td class="" pid-7881-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;727.71M</td>
                    <td class=""right"">15.9B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""7881""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""OMV AG PK"" _p_pid=""942090"" _r_pid=""942090"">
                        <span class=""earnCalCompanyName middle"">OMV AG PK</span>&nbsp;(<a href=""/equities/omv-ag-pk-earnings"" class=""bold middle"" target=""_blank"">OMVKY</a>)
                    </td>
                    <td class="" pid-942090-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.38</td>
                    <td class="" pid-942090-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;12.31B</td>
                    <td class=""right"">15.74B</td>
                    <td class=""right time"" data-value=""1""><span class=""marketOpen genToolTip oneliner reverseToolTip"" data-tooltip=""Before market open""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""942090""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Paycom Soft"" _p_pid=""101887"" _r_pid=""101887"">
                        <span class=""earnCalCompanyName middle"">Paycom Soft</span>&nbsp;(<a href=""/equities/paycom-soft-earnings"" class=""bold middle"" target=""_blank"">PAYC</a>)
                    </td>
                    <td class="" pid-101887-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.61</td>
                    <td class="" pid-101887-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;411.09M</td>
                    <td class=""right"">14.9B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""101887""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Magellan Midstream Partners LP"" _p_pid=""41031"" _r_pid=""41031"">
                        <span class=""earnCalCompanyName middle"">Magellan</span>&nbsp;(<a href=""/equities/magellan-midstream-partners-lp-earnings"" class=""bold middle"" target=""_blank"">MMP</a>)
                    </td>
                    <td class="" pid-41031-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.23</td>
                    <td class="" pid-41031-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;905.12M</td>
                    <td class=""right"">13.94B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""41031""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Revvity Inc"" _p_pid=""8279"" _r_pid=""8279"">
                        <span class=""earnCalCompanyName middle"">Revvity</span>&nbsp;(<a href=""/equities/perkinelmer-earnings"" class=""bold middle"" target=""_blank"">RVTY</a>)
                    </td>
                    <td class="" pid-8279-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.2</td>
                    <td class="" pid-8279-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;696.15M</td>
                    <td class=""right"">13.59B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""8279""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Bouygues SA ADR"" _p_pid=""1053858"" _r_pid=""1053858"">
                        <span class=""earnCalCompanyName middle"">Bouygues ADR</span>&nbsp;(<a href=""/equities/bouygues-adr-earnings"" class=""bold middle"" target=""_blank"">BOUYY</a>)
                    </td>
                    <td class="" pid-1053858-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.2844</td>
                    <td class="" pid-1053858-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;16.35B</td>
                    <td class=""right"">13.43B</td>
                    <td class=""right time"" data-value=""1""><span class=""marketOpen genToolTip oneliner reverseToolTip"" data-tooltip=""Before market open""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""1053858""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Ovintiv Inc"" _p_pid=""32530"" _r_pid=""32530"">
                        <span class=""earnCalCompanyName middle"">Ovintiv</span>&nbsp;(<a href=""/equities/encana-corporation-earnings"" class=""bold middle"" target=""_blank"">OVV</a>)
                    </td>
                    <td class="" pid-32530-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.58</td>
                    <td class="" pid-32530-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.72B</td>
                    <td class=""right"">13.06B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""32530""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Shionogi &amp; Co Ltd ADR"" _p_pid=""1054455"" _r_pid=""1054455"">
                        <span class=""earnCalCompanyName middle"">Shionogi ADR</span>&nbsp;(<a href=""/equities/shionogi-adr-earnings"" class=""bold middle"" target=""_blank"">SGIOY</a>)
                    </td>
                    <td class="" pid-1054455-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.1682</td>
                    <td class="" pid-1054455-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;478.98M</td>
                    <td class=""right"">13.02B</td>
                    <td class=""right time"" data-value=""1""><span class=""marketOpen genToolTip oneliner reverseToolTip"" data-tooltip=""Before market open""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""1054455""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Incyte Corporation"" _p_pid=""16360"" _r_pid=""16360"">
                        <span class=""earnCalCompanyName middle"">Incyte</span>&nbsp;(<a href=""/equities/incyte-corp-earnings"" class=""bold middle"" target=""_blank"">INCY</a>)
                    </td>
                    <td class="" pid-16360-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.9133</td>
                    <td class="" pid-16360-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;961.82M</td>
                    <td class=""right"">13.02B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16360""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Leidos Holdings Inc"" _p_pid=""19696"" _r_pid=""19696"">
                        <span class=""earnCalCompanyName middle"">Leidos</span>&nbsp;(<a href=""/equities/leidos-holdings-earnings"" class=""bold middle"" target=""_blank"">LDOS</a>)
                    </td>
                    <td class="" pid-19696-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.67</td>
                    <td class="" pid-19696-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;3.78B</td>
                    <td class=""right"">12.52B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""19696""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Franklin Resources Inc"" _p_pid=""8167"" _r_pid=""8167"">
                        <span class=""earnCalCompanyName middle"">Franklin Resources</span>&nbsp;(<a href=""/equities/franklin-res-earnings"" class=""bold middle"" target=""_blank"">BEN</a>)
                    </td>
                    <td class="" pid-8167-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.6175</td>
                    <td class="" pid-8167-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.52B</td>
                    <td class=""right"">12.48B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""8167""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Zebra Technologies Corporation"" _p_pid=""17608"" _r_pid=""17608"">
                        <span class=""earnCalCompanyName middle"">Zebra</span>&nbsp;(<a href=""/equities/zebra-tech-earnings"" class=""bold middle"" target=""_blank"">ZBRA</a>)
                    </td>
                    <td class="" pid-17608-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.8147</td>
                    <td class="" pid-17608-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;927.12M</td>
                    <td class=""right"">11.66B</td>
                    <td class=""right time"" data-value=""1""><span class=""marketOpen genToolTip oneliner reverseToolTip"" data-tooltip=""Before market open""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""17608""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Match Group Inc"" _p_pid=""961621"" _r_pid=""961621"">
                        <span class=""earnCalCompanyName middle"">Match Group</span>&nbsp;(<a href=""/equities/match-group-inc-earnings"" class=""bold middle"" target=""_blank"">MTCH</a>)
                    </td>
                    <td class="" pid-961621-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.5476</td>
                    <td class="" pid-961621-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;880.73M</td>
                    <td class=""right"">11.47B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""961621""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Lattice Semiconductor Corporation"" _p_pid=""16544"" _r_pid=""16544"">
                        <span class=""earnCalCompanyName middle"">Lattice</span>&nbsp;(<a href=""/equities/lattice-semiconductor-earnings"" class=""bold middle"" target=""_blank"">LSCC</a>)
                    </td>
                    <td class="" pid-16544-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.5222</td>
                    <td class="" pid-16544-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;192.1M</td>
                    <td class=""right"">11.4B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16544""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Fidelity National Financial Inc"" _p_pid=""39241"" _r_pid=""39241"">
                        <span class=""earnCalCompanyName middle"">Fidelity Financial</span>&nbsp;(<a href=""/equities/fidelity-national-financial-earnings"" class=""bold middle"" target=""_blank"">FNF</a>)
                    </td>
                    <td class="" pid-39241-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.13</td>
                    <td class="" pid-39241-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.8B</td>
                    <td class=""right"">11.3B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""39241""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Bio-Techne Corp"" _p_pid=""17327"" _r_pid=""17327"">
                        <span class=""earnCalCompanyName middle"">Bio-Techne</span>&nbsp;(<a href=""/equities/techne-corp-earnings"" class=""bold middle"" target=""_blank"">TECH</a>)
                    </td>
                    <td class="" pid-17327-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.4358</td>
                    <td class="" pid-17327-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;288.05M</td>
                    <td class=""right"">10.97B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""17327""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Renaissancere Holdings Ltd"" _p_pid=""20420"" _r_pid=""20420"">
                        <span class=""earnCalCompanyName middle"">Renaissancere</span>&nbsp;(<a href=""/equities/renaissancere-holdings-ltd-earnings"" class=""bold middle"" target=""_blank"">RNR</a>)
                    </td>
                    <td class="" pid-20420-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;3.1</td>
                    <td class="" pid-20420-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.88B</td>
                    <td class=""right"">10.38B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20420""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""ANA Holdings Inc ADR"" _p_pid=""941731"" _r_pid=""941731"">
                        <span class=""earnCalCompanyName middle"">ANA Holdings ADR</span>&nbsp;(<a href=""/equities/ana-holdings-inc-earnings"" class=""bold middle"" target=""_blank"">ALNPY</a>)
                    </td>
                    <td class="" pid-941731-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.117</td>
                    <td class="" pid-941731-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;3.7B</td>
                    <td class=""right"">10.3B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""941731""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Caesars Entertainment Corporation"" _p_pid=""31022"" _r_pid=""31022"">
                        <span class=""earnCalCompanyName middle"">Caesars</span>&nbsp;(<a href=""/equities/caesars-entertainment-corp-earnings"" class=""bold middle"" target=""_blank"">CZR</a>)
                    </td>
                    <td class="" pid-31022-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.2834</td>
                    <td class="" pid-31022-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.92B</td>
                    <td class=""right"">9.9B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""31022""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Unum Group"" _p_pid=""8174"" _r_pid=""8174"">
                        <span class=""earnCalCompanyName middle"">Unum</span>&nbsp;(<a href=""/equities/unum-group-earnings"" class=""bold middle"" target=""_blank"">UNM</a>)
                    </td>
                    <td class="" pid-8174-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.92</td>
                    <td class="" pid-8174-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;3.12B</td>
                    <td class=""right"">9.77B</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""8174""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Regal Beloit Corporation"" _p_pid=""20896"" _r_pid=""20896"">
                        <span class=""earnCalCompanyName middle"">Regal Beloit</span>&nbsp;(<a href=""/equities/regal-beloit-corp-earnings"" class=""bold middle"" target=""_blank"">RRX</a>)
                    </td>
                    <td class="" pid-20896-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.6</td>
                    <td class="" pid-20896-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.73B</td>
                    <td class=""right"">9.71B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20896""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Smith AO Corporation"" _p_pid=""39291"" _r_pid=""39291"">
                        <span class=""earnCalCompanyName middle"">AO Smith</span>&nbsp;(<a href=""/equities/a.o-smith-corp-earnings"" class=""bold middle"" target=""_blank"">AOS</a>)
                    </td>
                    <td class="" pid-39291-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.7678</td>
                    <td class="" pid-39291-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;901.5M</td>
                    <td class=""right"">9.71B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""39291""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""American Financial Group Inc"" _p_pid=""39217"" _r_pid=""39217"">
                        <span class=""earnCalCompanyName middle"">American Financial</span>&nbsp;(<a href=""/equities/american-financial-group-earnings"" class=""bold middle"" target=""_blank"">AFG</a>)
                    </td>
                    <td class="" pid-39217-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.51</td>
                    <td class="" pid-39217-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.65B</td>
                    <td class=""right"">9.62B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""39217""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Henry Schein Inc"" _p_pid=""6405"" _r_pid=""6405"">
                        <span class=""earnCalCompanyName middle"">Henry Schein</span>&nbsp;(<a href=""/equities/henry-schein-earnings"" class=""bold middle"" target=""_blank"">HSIC</a>)
                    </td>
                    <td class="" pid-6405-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.33</td>
                    <td class="" pid-6405-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;3.21B</td>
                    <td class=""right"">9.55B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""6405""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""OMRON Corp ADR"" _p_pid=""942089"" _r_pid=""942089"">
                        <span class=""earnCalCompanyName middle"">OMRON ADR</span>&nbsp;(<a href=""/equities/omron-corp-earnings"" class=""bold middle"" target=""_blank"">OMRNY</a>)
                    </td>
                    <td class="" pid-942089-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.5676</td>
                    <td class="" pid-942089-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.41B</td>
                    <td class=""right"">8.97B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""942089""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Daiwa Securities Group Inc ADR"" _p_pid=""941845"" _r_pid=""941845"">
                        <span class=""earnCalCompanyName middle"">Daiwa ADR</span>&nbsp;(<a href=""/equities/daiwa-securities-group-inc-earnings"" class=""bold middle"" target=""_blank"">DSEEY</a>)
                    </td>
                    <td class="" pid-941845-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-941845-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">8.72B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""941845""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""AGCO Corporation"" _p_pid=""39138"" _r_pid=""39138"">
                        <span class=""earnCalCompanyName middle"">AGCO</span>&nbsp;(<a href=""/equities/agco-earnings"" class=""bold middle"" target=""_blank"">AGCO</a>)
                    </td>
                    <td class="" pid-39138-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;3.35</td>
                    <td class="" pid-39138-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;3.48B</td>
                    <td class=""right"">8.69B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""39138""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Liberty Global PLC"" _p_pid=""6371"" _r_pid=""6371"">
                        <span class=""earnCalCompanyName middle"">Liberty Global</span>&nbsp;(<a href=""/equities/liberty-global-inc-earnings"" class=""bold middle"" target=""_blank"">LBTYA</a>)
                    </td>
                    <td class="" pid-6371-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.0133</td>
                    <td class="" pid-6371-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.89B</td>
                    <td class=""right"">8.01B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""6371""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Liberty Global PLC Class C"" _p_pid=""16491"" _r_pid=""16491"">
                        <span class=""earnCalCompanyName middle"">Liberty Global C</span>&nbsp;(<a href=""/equities/liberty-global-(c)-earnings"" class=""bold middle"" target=""_blank"">LBTYK</a>)
                    </td>
                    <td class="" pid-16491-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.0133</td>
                    <td class="" pid-16491-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.89B</td>
                    <td class=""right"">8.01B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16491""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Liberty Global PLC Class B"" _p_pid=""16490"" _r_pid=""16490"">
                        <span class=""earnCalCompanyName middle"">Liberty Global B</span>&nbsp;(<a href=""/equities/liberty-global-(b)-earnings"" class=""bold middle"" target=""_blank"">LBTYB</a>)
                    </td>
                    <td class="" pid-16490-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.0133</td>
                    <td class="" pid-16490-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.89B</td>
                    <td class=""right"">8.01B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16490""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""XPO Inc"" _p_pid=""29751"" _r_pid=""29751"">
                        <span class=""earnCalCompanyName middle"">XPO</span>&nbsp;(<a href=""/equities/xpo-logistics-earnings"" class=""bold middle"" target=""_blank"">XPO</a>)
                    </td>
                    <td class="" pid-29751-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.6233</td>
                    <td class="" pid-29751-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.92B</td>
                    <td class=""right"">7.96B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""29751""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Topbuild Corp"" _p_pid=""955539"" _r_pid=""955539"">
                        <span class=""earnCalCompanyName middle"">Topbuild Corp</span>&nbsp;(<a href=""/equities/topbuild-corp-earnings"" class=""bold middle"" target=""_blank"">BLD</a>)
                    </td>
                    <td class="" pid-955539-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;4.57</td>
                    <td class="" pid-955539-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.29B</td>
                    <td class=""right"">7.88B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""955539""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Lear Corporation"" _p_pid=""39140"" _r_pid=""39140"">
                        <span class=""earnCalCompanyName middle"">Lear</span>&nbsp;(<a href=""/equities/lear-earnings"" class=""bold middle"" target=""_blank"">LEA</a>)
                    </td>
                    <td class="" pid-39140-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.75</td>
                    <td class="" pid-39140-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;5.56B</td>
                    <td class=""right"">7.8B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""39140""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Assurant Inc"" _p_pid=""8061"" _r_pid=""8061"">
                        <span class=""earnCalCompanyName middle"">Assurant</span>&nbsp;(<a href=""/equities/assurant-earnings"" class=""bold middle"" target=""_blank"">AIZ</a>)
                    </td>
                    <td class="" pid-8061-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.23</td>
                    <td class="" pid-8061-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.67B</td>
                    <td class=""right"">7.69B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""8061""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Ternium SA ADR"" _p_pid=""32357"" _r_pid=""32357"">
                        <span class=""earnCalCompanyName middle"">Ternium ADR</span>&nbsp;(<a href=""/equities/ternium-s.a.-earnings"" class=""bold middle"" target=""_blank"">TX</a>)
                    </td>
                    <td class="" pid-32357-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.54</td>
                    <td class="" pid-32357-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;3.86B</td>
                    <td class=""right"">7.55B</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""32357""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Voya Financial Inc"" _p_pid=""41215"" _r_pid=""41215"">
                        <span class=""earnCalCompanyName middle"">Voya Financial Inc</span>&nbsp;(<a href=""/equities/ing-us-inc-earnings"" class=""bold middle"" target=""_blank"">VOYA</a>)
                    </td>
                    <td class="" pid-41215-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.09</td>
                    <td class="" pid-41215-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.73B</td>
                    <td class=""right"">7.23B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""41215""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Noble Corp"" _p_pid=""1174272"" _r_pid=""1174272"">
                        <span class=""earnCalCompanyName middle"">Noble</span>&nbsp;(<a href=""/equities/noble-earnings"" class=""bold middle"" target=""_blank"">NE</a>)
                    </td>
                    <td class="" pid-1174272-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.8475</td>
                    <td class="" pid-1174272-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;648.25M</td>
                    <td class=""right"">7.16B</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1174272""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Graphic Packaging Holding Company"" _p_pid=""20263"" _r_pid=""20263"">
                        <span class=""earnCalCompanyName middle"">Graphic Packaging</span>&nbsp;(<a href=""/equities/graphic-packaging-holding-comp-earnings"" class=""bold middle"" target=""_blank"">GPK</a>)
                    </td>
                    <td class="" pid-20263-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.7149</td>
                    <td class="" pid-20263-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.48B</td>
                    <td class=""right"">6.94B</td>
                    <td class=""right time"" data-value=""1""><span class=""marketOpen genToolTip oneliner reverseToolTip"" data-tooltip=""Before market open""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20263""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Exelixis Inc"" _p_pid=""32545"" _r_pid=""32545"">
                        <span class=""earnCalCompanyName middle"">Exelixis</span>&nbsp;(<a href=""/equities/exelixis-inc-earnings"" class=""bold middle"" target=""_blank"">EXEL</a>)
                    </td>
                    <td class="" pid-32545-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.1948</td>
                    <td class="" pid-32545-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;471.89M</td>
                    <td class=""right"">6.84B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""32545""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Stifel Financial Corporation"" _p_pid=""20664"" _r_pid=""20664"">
                        <span class=""earnCalCompanyName middle"">Stifel</span>&nbsp;(<a href=""/equities/stifel-financial-corp-earnings"" class=""bold middle"" target=""_blank"">SF</a>)
                    </td>
                    <td class="" pid-20664-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.45</td>
                    <td class="" pid-20664-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.14B</td>
                    <td class=""right"">6.57B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20664""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""R1 RCM Inc"" _p_pid=""998046"" _r_pid=""998046"">
                        <span class=""earnCalCompanyName middle"">R1 RCM</span>&nbsp;(<a href=""/equities/r1-rcm-inc-earnings"" class=""bold middle"" target=""_blank"">RCM</a>)
                    </td>
                    <td class="" pid-998046-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.0814</td>
                    <td class="" pid-998046-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;569.94M</td>
                    <td class=""right"">6.53B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""998046""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""ZoomInfo Technologies Inc"" _p_pid=""1161497"" _r_pid=""1161497"">
                        <span class=""earnCalCompanyName middle"">ZoomInfo</span>&nbsp;(<a href=""/equities/zoominfo-technologies-inc-earnings"" class=""bold middle"" target=""_blank"">ZI</a>)
                    </td>
                    <td class="" pid-1161497-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.245</td>
                    <td class="" pid-1161497-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;310.57M</td>
                    <td class=""right"">6.37B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1161497""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""H&amp;R Block Inc"" _p_pid=""8170"" _r_pid=""8170"">
                        <span class=""earnCalCompanyName middle"">H&amp;R Block</span>&nbsp;(<a href=""/equities/h---r-block-inc-earnings"" class=""bold middle"" target=""_blank"">HRB</a>)
                    </td>
                    <td class="" pid-8170-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-1.11</td>
                    <td class="" pid-8170-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;183.4M</td>
                    <td class=""right"">6.15B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""8170""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Freshworks Inc"" _p_pid=""1177430"" _r_pid=""1177430"">
                        <span class=""earnCalCompanyName middle"">Freshworks</span>&nbsp;(<a href=""/equities/freshworks-inc-earnings"" class=""bold middle"" target=""_blank"">FRSH</a>)
                    </td>
                    <td class="" pid-1177430-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.0524</td>
                    <td class="" pid-1177430-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;150.68M</td>
                    <td class=""right"">5.69B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1177430""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Sensata Technologies Holding NV"" _p_pid=""39302"" _r_pid=""39302"">
                        <span class=""earnCalCompanyName middle"">Sensata Tech</span>&nbsp;(<a href=""/equities/sensata-technologies-holding-earnings"" class=""bold middle"" target=""_blank"">ST</a>)
                    </td>
                    <td class="" pid-39302-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.9036</td>
                    <td class="" pid-39302-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.01B</td>
                    <td class=""right"">5.62B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""39302""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Valaris Ltd"" _p_pid=""1173854"" _r_pid=""1173854"">
                        <span class=""earnCalCompanyName middle"">Valaris</span>&nbsp;(<a href=""/equities/valaris-earnings"" class=""bold middle"" target=""_blank"">VAL</a>)
                    </td>
                    <td class="" pid-1173854-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.3938</td>
                    <td class="" pid-1173854-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;479.78M</td>
                    <td class=""right"">5.49B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1173854""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Inspire Medical Systems Inc"" _p_pid=""1073227"" _r_pid=""1073227"">
                        <span class=""earnCalCompanyName middle"">Inspire Medical Systems</span>&nbsp;(<a href=""/equities/inspire-medical-systems-inc-earnings"" class=""bold middle"" target=""_blank"">INSP</a>)
                    </td>
                    <td class="" pid-1073227-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.6016</td>
                    <td class="" pid-1073227-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;153.3M</td>
                    <td class=""right"">5.43B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1073227""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""MSC Industrial Direct Company Inc"" _p_pid=""20267"" _r_pid=""20267"">
                        <span class=""earnCalCompanyName middle"">MSC Industrial Direct</span>&nbsp;(<a href=""/equities/msc-industrial-direct-comp-inc-earnings"" class=""bold middle"" target=""_blank"">MSM</a>)
                    </td>
                    <td class="" pid-20267-2023-10-31-082023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.62</td>
                    <td class="" pid-20267-2023-10-31-082023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.01B</td>
                    <td class=""right"">5.33B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20267""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Bright Horizons Family Solutions Inc"" _p_pid=""41256"" _r_pid=""41256"">
                        <span class=""earnCalCompanyName middle"">Bright Horizons</span>&nbsp;(<a href=""/equities/brgt-hrz-fml-slt-earnings"" class=""bold middle"" target=""_blank"">BFAM</a>)
                    </td>
                    <td class="" pid-41256-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.815</td>
                    <td class="" pid-41256-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;619.04M</td>
                    <td class=""right"">4.92B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""41256""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Nexstar Media Group Inc"" _p_pid=""16781"" _r_pid=""16781"">
                        <span class=""earnCalCompanyName middle"">Nexstar</span>&nbsp;(<a href=""/equities/nexstar-broadcast-earnings"" class=""bold middle"" target=""_blank"">NXST</a>)
                    </td>
                    <td class="" pid-16781-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.14</td>
                    <td class="" pid-16781-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.17B</td>
                    <td class=""right"">4.9B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16781""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Hamilton Lane Inc"" _p_pid=""997108"" _r_pid=""997108"">
                        <span class=""earnCalCompanyName middle"">Hamilton Lane</span>&nbsp;(<a href=""/equities/hamilton-lane-inc-earnings"" class=""bold middle"" target=""_blank"">HLNE</a>)
                    </td>
                    <td class="" pid-997108-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.9454</td>
                    <td class="" pid-997108-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;121.49M</td>
                    <td class=""right"">4.88B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""997108""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Colliers International Group Inc Bats"" _p_pid=""24541"" _r_pid=""16137"">
                        <span class=""earnCalCompanyName middle"">Colliers International</span>&nbsp;(<a href=""/equities/firstservice-earnings?cid=16137"" class=""bold middle"" target=""_blank"">CIGI</a>)
                    </td>
                    <td class="" pid-16137-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.73</td>
                    <td class="" pid-16137-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.1B</td>
                    <td class=""right"">4.85B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16137""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""IPG Photonics Corporation"" _p_pid=""13981"" _r_pid=""13981"">
                        <span class=""earnCalCompanyName middle"">IPG Photonics</span>&nbsp;(<a href=""/equities/ipg-photonics-corp.-earnings"" class=""bold middle"" target=""_blank"">IPGP</a>)
                    </td>
                    <td class="" pid-13981-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.03</td>
                    <td class="" pid-13981-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;318.47M</td>
                    <td class=""right"">4.74B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""13981""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Magnolia Oil &amp; Gas Corp"" _p_pid=""1025072"" _r_pid=""1025072"">
                        <span class=""earnCalCompanyName middle"">Magnolia Oil</span>&nbsp;(<a href=""/equities/tpg-pace-energy-class-a-earnings"" class=""bold middle"" target=""_blank"">MGY</a>)
                    </td>
                    <td class="" pid-1025072-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.5258</td>
                    <td class="" pid-1025072-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;307.19M</td>
                    <td class=""right"">4.67B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1025072""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Rithm Capital Corp"" _p_pid=""41225"" _r_pid=""41225"">
                        <span class=""earnCalCompanyName middle"">Rithm Capital</span>&nbsp;(<a href=""/equities/new-rel-invest-earnings"" class=""bold middle"" target=""_blank"">RITM</a>)
                    </td>
                    <td class="" pid-41225-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.3891</td>
                    <td class="" pid-41225-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;893.28M</td>
                    <td class=""right"">4.67B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""41225""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Impala Platinum Holdings Ltd PK"" _p_pid=""941953"" _r_pid=""941953"">
                        <span class=""earnCalCompanyName middle"">Impala Platinum Holdings Ltd PK</span>&nbsp;(<a href=""/equities/impala-platinum-holdings-ltd-pk-earnings"" class=""bold middle"" target=""_blank"">IMPUY</a>)
                    </td>
                    <td class="" pid-941953-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-941953-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">4.66B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""941953""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""MicroStrategy Incorporated"" _p_pid=""16678"" _r_pid=""16678"">
                        <span class=""earnCalCompanyName middle"">MicroStrategy</span>&nbsp;(<a href=""/equities/microstrategy-inc-earnings"" class=""bold middle"" target=""_blank"">MSTR</a>)
                    </td>
                    <td class="" pid-16678-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.04</td>
                    <td class="" pid-16678-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;126.25M</td>
                    <td class=""right"">4.55B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16678""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Sealed Air Corporation"" _p_pid=""8076"" _r_pid=""8076"">
                        <span class=""earnCalCompanyName middle"">Sealed Air</span>&nbsp;(<a href=""/equities/sealed-air-earnings"" class=""bold middle"" target=""_blank"">SEE</a>)
                    </td>
                    <td class="" pid-8076-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.639</td>
                    <td class="" pid-8076-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.37B</td>
                    <td class=""right"">4.54B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""8076""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Meritage Corporation"" _p_pid=""32325"" _r_pid=""32325"">
                        <span class=""earnCalCompanyName middle"">Meritage</span>&nbsp;(<a href=""/equities/meritage-homes-corp-earnings"" class=""bold middle"" target=""_blank"">MTH</a>)
                    </td>
                    <td class="" pid-32325-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;4.97</td>
                    <td class="" pid-32325-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.54B</td>
                    <td class=""right"">4.51B</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""32325""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Enact Holdings Inc"" _p_pid=""1173260"" _r_pid=""1173260"">
                        <span class=""earnCalCompanyName middle"">Enact Holdings</span>&nbsp;(<a href=""/equities/enact-holdings-earnings"" class=""bold middle"" target=""_blank"">ACT</a>)
                    </td>
                    <td class="" pid-1173260-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.87</td>
                    <td class="" pid-1173260-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;259.95M</td>
                    <td class=""right"">4.42B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1173260""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Seaboard Corporation"" _p_pid=""24426"" _r_pid=""24426"">
                        <span class=""earnCalCompanyName middle"">Seaboard</span>&nbsp;(<a href=""/equities/seaboard-corp-earnings"" class=""bold middle"" target=""_blank"">SEB</a>)
                    </td>
                    <td class="" pid-24426-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-24426-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">4.41B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""24426""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Red Rock Resorts Inc"" _p_pid=""977677"" _r_pid=""977677"">
                        <span class=""earnCalCompanyName middle"">Red Rock Resorts</span>&nbsp;(<a href=""/equities/red-rock-resorts-inc-earnings"" class=""bold middle"" target=""_blank"">RRR</a>)
                    </td>
                    <td class="" pid-977677-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.4164</td>
                    <td class="" pid-977677-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;410.32M</td>
                    <td class=""right"">4.34B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""977677""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""MakeMyTrip Limited"" _p_pid=""16640"" _r_pid=""16640"">
                        <span class=""earnCalCompanyName middle"">MakeMyTrip</span>&nbsp;(<a href=""/equities/makemytrip-ltd-earnings"" class=""bold middle"" target=""_blank"">MMYT</a>)
                    </td>
                    <td class="" pid-16640-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.0732</td>
                    <td class="" pid-16640-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;171.52M</td>
                    <td class=""right"">4.01B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16640""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Louisiana-Pacific Corporation"" _p_pid=""39168"" _r_pid=""39168"">
                        <span class=""earnCalCompanyName middle"">Louisiana-Pacific</span>&nbsp;(<a href=""/equities/louisiana-pacific-earnings"" class=""bold middle"" target=""_blank"">LPX</a>)
                    </td>
                    <td class="" pid-39168-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.48</td>
                    <td class="" pid-39168-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;737.59M</td>
                    <td class=""right"">4.01B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""39168""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Cirrus Logic Inc"" _p_pid=""13973"" _r_pid=""13973"">
                        <span class=""earnCalCompanyName middle"">Cirrus</span>&nbsp;(<a href=""/equities/cirrus-logic-inc.-earnings"" class=""bold middle"" target=""_blank"">CRUS</a>)
                    </td>
                    <td class="" pid-13973-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.54</td>
                    <td class="" pid-13973-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;457.55M</td>
                    <td class=""right"">3.93B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""13973""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""CNX Resources Corp"" _p_pid=""7975"" _r_pid=""7975"">
                        <span class=""earnCalCompanyName middle"">CNX Resources</span>&nbsp;(<a href=""/equities/consol-energy-earnings"" class=""bold middle"" target=""_blank"">CNX</a>)
                    </td>
                    <td class="" pid-7975-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.3617</td>
                    <td class="" pid-7975-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;406.94M</td>
                    <td class=""right"">3.86B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""7975""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Yamazaki Baking Co Ltd ADR"" _p_pid=""1054645"" _r_pid=""1054645"">
                        <span class=""earnCalCompanyName middle"">Yamazaki Baking ADR</span>&nbsp;(<a href=""/equities/yamazaki-baking-adr-earnings"" class=""bold middle"" target=""_blank"">YMZBY</a>)
                    </td>
                    <td class="" pid-1054645-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.5723</td>
                    <td class="" pid-1054645-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.92B</td>
                    <td class=""right"">3.85B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""1054645""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""LYFT Inc"" _p_pid=""1123146"" _r_pid=""1123146"">
                        <span class=""earnCalCompanyName middle"">LYFT</span>&nbsp;(<a href=""/equities/lyft-earnings"" class=""bold middle"" target=""_blank"">LYFT</a>)
                    </td>
                    <td class="" pid-1123146-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.1445</td>
                    <td class="" pid-1123146-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.14B</td>
                    <td class=""right"">3.84B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1123146""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""NCR Corp"" _p_pid=""8277"" _r_pid=""8277"">
                        <span class=""earnCalCompanyName middle"">NCR</span>&nbsp;(<a href=""/equities/ncr-corp-earnings"" class=""bold middle"" target=""_blank"">NCR</a>)
                    </td>
                    <td class="" pid-8277-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.935</td>
                    <td class="" pid-8277-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.03B</td>
                    <td class=""right"">3.81B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""8277""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Surgery Partners Inc"" _p_pid=""958829"" _r_pid=""958829"">
                        <span class=""earnCalCompanyName middle"">Surgery Partners Inc</span>&nbsp;(<a href=""/equities/surgery-partners-inc-earnings"" class=""bold middle"" target=""_blank"">SGRY</a>)
                    </td>
                    <td class="" pid-958829-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.1448</td>
                    <td class="" pid-958829-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;673.2M</td>
                    <td class=""right"">3.77B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""958829""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Advanced Energy Industries Inc"" _p_pid=""13963"" _r_pid=""13963"">
                        <span class=""earnCalCompanyName middle"">Advanced Energy</span>&nbsp;(<a href=""/equities/advanced-energy-earnings"" class=""bold middle"" target=""_blank"">AEIS</a>)
                    </td>
                    <td class="" pid-13963-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.11</td>
                    <td class="" pid-13963-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;419.28M</td>
                    <td class=""right"">3.75B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""13963""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Vishay Intertechnology Inc"" _p_pid=""29721"" _r_pid=""29721"">
                        <span class=""earnCalCompanyName middle"">Vishay Intertechnology</span>&nbsp;(<a href=""/equities/vishay-intertechnology-inc-earnings"" class=""bold middle"" target=""_blank"">VSH</a>)
                    </td>
                    <td class="" pid-29721-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.56</td>
                    <td class="" pid-29721-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;860.77M</td>
                    <td class=""right"">3.4B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""29721""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""CorVel Corp"" _p_pid=""15819"" _r_pid=""15819"">
                        <span class=""earnCalCompanyName middle"">CorVel</span>&nbsp;(<a href=""/equities/corvel-corp-earnings"" class=""bold middle"" target=""_blank"">CRVL</a>)
                    </td>
                    <td class="" pid-15819-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-15819-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">3.26B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""15819""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""PJT Partners Inc"" _p_pid=""958819"" _r_pid=""958819"">
                        <span class=""earnCalCompanyName middle"">PJT Partners Inc</span>&nbsp;(<a href=""/equities/pjt-partners-inc-earnings"" class=""bold middle"" target=""_blank"">PJT</a>)
                    </td>
                    <td class="" pid-958819-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.5987</td>
                    <td class="" pid-958819-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;244.87M</td>
                    <td class=""right"">3.09B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""958819""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Amedisys Inc"" _p_pid=""15391"" _r_pid=""15391"">
                        <span class=""earnCalCompanyName middle"">Amedisys</span>&nbsp;(<a href=""/equities/amedisys-inc-earnings"" class=""bold middle"" target=""_blank"">AMED</a>)
                    </td>
                    <td class="" pid-15391-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.05</td>
                    <td class="" pid-15391-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;559.62M</td>
                    <td class=""right"">3.06B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""15391""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Consol Energy Inc"" _p_pid=""1055312"" _r_pid=""1055312"">
                        <span class=""earnCalCompanyName middle"">Consol Energy</span>&nbsp;(<a href=""/equities/consol-energy-k-earnings"" class=""bold middle"" target=""_blank"">CEIX</a>)
                    </td>
                    <td class="" pid-1055312-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;4.93</td>
                    <td class="" pid-1055312-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;592.45M</td>
                    <td class=""right"">3.01B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1055312""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""CommVault Systems Inc"" _p_pid=""39328"" _r_pid=""39328"">
                        <span class=""earnCalCompanyName middle"">CommVault</span>&nbsp;(<a href=""/equities/commvault-system-earnings"" class=""bold middle"" target=""_blank"">CVLT</a>)
                    </td>
                    <td class="" pid-39328-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.6425</td>
                    <td class="" pid-39328-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;195.17M</td>
                    <td class=""right"">2.99B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""39328""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""LCI Industries"" _p_pid=""20745"" _r_pid=""20745"">
                        <span class=""earnCalCompanyName middle"">LCI Industries</span>&nbsp;(<a href=""/equities/drew-industries-inc-earnings"" class=""bold middle"" target=""_blank"">LCII</a>)
                    </td>
                    <td class="" pid-20745-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.37</td>
                    <td class="" pid-20745-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1B</td>
                    <td class=""right"">2.98B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20745""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Artisan Partners Asset Management Inc"" _p_pid=""41193"" _r_pid=""41193"">
                        <span class=""earnCalCompanyName middle"">Artisan Partners AM</span>&nbsp;(<a href=""/equities/artsn-prtnr-asst-earnings"" class=""bold middle"" target=""_blank"">APAM</a>)
                    </td>
                    <td class="" pid-41193-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.79</td>
                    <td class="" pid-41193-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;251.8M</td>
                    <td class=""right"">2.97B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""41193""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Broadstone Net Lease&nbsp;Inc"" _p_pid=""1166008"" _r_pid=""1166008"">
                        <span class=""earnCalCompanyName middle"">Broadstone Net</span>&nbsp;(<a href=""/equities/broadstone-net-lease-inc-earnings"" class=""bold middle"" target=""_blank"">BNL</a>)
                    </td>
                    <td class="" pid-1166008-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.1716</td>
                    <td class="" pid-1166008-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;111.88M</td>
                    <td class=""right"">2.92B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1166008""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Hayward Holdings Inc"" _p_pid=""1171379"" _r_pid=""1171379"">
                        <span class=""earnCalCompanyName middle"">Hayward Holdings</span>&nbsp;(<a href=""/equities/hayward-holdings-earnings"" class=""bold middle"" target=""_blank"">HAYW</a>)
                    </td>
                    <td class="" pid-1171379-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.0834</td>
                    <td class="" pid-1171379-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;212.39M</td>
                    <td class=""right"">2.91B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1171379""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Holly Energy Partners LP"" _p_pid=""958145"" _r_pid=""958145"">
                        <span class=""earnCalCompanyName middle"">Holly Energy Partners LP</span>&nbsp;(<a href=""/equities/holly-energy-partners-lp-earnings"" class=""bold middle"" target=""_blank"">HEP</a>)
                    </td>
                    <td class="" pid-958145-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.5144</td>
                    <td class="" pid-958145-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;155.98M</td>
                    <td class=""right"">2.81B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""958145""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Genworth Financial Inc"" _p_pid=""8143"" _r_pid=""8143"">
                        <span class=""earnCalCompanyName middle"">Genworth</span>&nbsp;(<a href=""/equities/genworth-finl-earnings"" class=""bold middle"" target=""_blank"">GNW</a>)
                    </td>
                    <td class="" pid-8143-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.175</td>
                    <td class="" pid-8143-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.96B</td>
                    <td class=""right"">2.74B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""8143""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Coursera Inc"" _p_pid=""1171969"" _r_pid=""1171969"">
                        <span class=""earnCalCompanyName middle"">Coursera</span>&nbsp;(<a href=""/equities/coursera-earnings"" class=""bold middle"" target=""_blank"">COUR</a>)
                    </td>
                    <td class="" pid-1171969-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.054</td>
                    <td class="" pid-1171969-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;153.92M</td>
                    <td class=""right"">2.71B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1171969""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""CNO Financial Group Inc"" _p_pid=""20525"" _r_pid=""20525"">
                        <span class=""earnCalCompanyName middle"">CNO Financial</span>&nbsp;(<a href=""/equities/cno-financial-group-inc-earnings"" class=""bold middle"" target=""_blank"">CNO</a>)
                    </td>
                    <td class="" pid-20525-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.7433</td>
                    <td class="" pid-20525-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;929.4M</td>
                    <td class=""right"">2.7B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20525""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""O-I Glass Inc"" _p_pid=""13859"" _r_pid=""13859"">
                        <span class=""earnCalCompanyName middle"">O-I Glass</span>&nbsp;(<a href=""/equities/owens-illinois-inc-earnings"" class=""bold middle"" target=""_blank"">OI</a>)
                    </td>
                    <td class="" pid-13859-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.6989</td>
                    <td class="" pid-13859-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.77B</td>
                    <td class=""right"">2.67B</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""13859""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""iRhythm Technologies Inc"" _p_pid=""992765"" _r_pid=""992765"">
                        <span class=""earnCalCompanyName middle"">iRhythm Tech</span>&nbsp;(<a href=""/equities/irhythm-technologies-inc-earnings"" class=""bold middle"" target=""_blank"">IRTC</a>)
                    </td>
                    <td class="" pid-992765-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.6262</td>
                    <td class="" pid-992765-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;122.63M</td>
                    <td class=""right"">2.61B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""992765""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Alteryx Inc"" _p_pid=""998043"" _r_pid=""998043"">
                        <span class=""earnCalCompanyName middle"">Alteryx</span>&nbsp;(<a href=""/equities/alteryx-inc-earnings"" class=""bold middle"" target=""_blank"">AYX</a>)
                    </td>
                    <td class="" pid-998043-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.0562</td>
                    <td class="" pid-998043-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;210.2M</td>
                    <td class=""right"">2.6B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""998043""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Ultragenyx"" _p_pid=""100239"" _r_pid=""100239"">
                        <span class=""earnCalCompanyName middle"">Ultragenyx</span>&nbsp;(<a href=""/equities/ultragenyx-earnings"" class=""bold middle"" target=""_blank"">RARE</a>)
                    </td>
                    <td class="" pid-100239-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-2.05</td>
                    <td class="" pid-100239-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;114.94M</td>
                    <td class=""right"">2.6B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""100239""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Kadant Inc"" _p_pid=""21166"" _r_pid=""21166"">
                        <span class=""earnCalCompanyName middle"">Kadant</span>&nbsp;(<a href=""/equities/kadant-inc-earnings"" class=""bold middle"" target=""_blank"">KAI</a>)
                    </td>
                    <td class="" pid-21166-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.28</td>
                    <td class="" pid-21166-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;233.56M</td>
                    <td class=""right"">2.59B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""21166""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Premier Inc"" _p_pid=""41323"" _r_pid=""41323"">
                        <span class=""earnCalCompanyName middle"">Premier Inc</span>&nbsp;(<a href=""/equities/premier-inc-earnings"" class=""bold middle"" target=""_blank"">PINC</a>)
                    </td>
                    <td class="" pid-41323-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.5146</td>
                    <td class="" pid-41323-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;320.16M</td>
                    <td class=""right"">2.59B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""41323""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Innospec Inc"" _p_pid=""16381"" _r_pid=""16381"">
                        <span class=""earnCalCompanyName middle"">Innospec</span>&nbsp;(<a href=""/equities/innospec-inc-earnings"" class=""bold middle"" target=""_blank"">IOSP</a>)
                    </td>
                    <td class="" pid-16381-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.51</td>
                    <td class="" pid-16381-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;491.9M</td>
                    <td class=""right"">2.53B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16381""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Rogers Corporation"" _p_pid=""21136"" _r_pid=""21136"">
                        <span class=""earnCalCompanyName middle"">Rogers</span>&nbsp;(<a href=""/equities/rogers-corp-earnings"" class=""bold middle"" target=""_blank"">ROG</a>)
                    </td>
                    <td class="" pid-21136-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.18</td>
                    <td class="" pid-21136-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;247.55M</td>
                    <td class=""right"">2.51B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""21136""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""McGrath RentCorp"" _p_pid=""16619"" _r_pid=""16619"">
                        <span class=""earnCalCompanyName middle"">McGrath</span>&nbsp;(<a href=""/equities/mcgrath-rentcorp-earnings"" class=""bold middle"" target=""_blank"">MGRC</a>)
                    </td>
                    <td class="" pid-16619-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.29</td>
                    <td class="" pid-16619-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;185.98M</td>
                    <td class=""right"">2.49B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16619""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""LGI Homes"" _p_pid=""44409"" _r_pid=""44409"">
                        <span class=""earnCalCompanyName middle"">LGI Homes</span>&nbsp;(<a href=""/equities/lgi-homes-earnings"" class=""bold middle"" target=""_blank"">LGIH</a>)
                    </td>
                    <td class="" pid-44409-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.29</td>
                    <td class="" pid-44409-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;643.38M</td>
                    <td class=""right"">2.43B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""44409""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""NMI Holdings Inc"" _p_pid=""44411"" _r_pid=""44411"">
                        <span class=""earnCalCompanyName middle"">NMI Holdings</span>&nbsp;(<a href=""/equities/nmi-holding-earnings"" class=""bold middle"" target=""_blank"">NMIH</a>)
                    </td>
                    <td class="" pid-44411-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.9091</td>
                    <td class="" pid-44411-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;128.03M</td>
                    <td class=""right"">2.33B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""44411""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Modine Manufacturing Company"" _p_pid=""20455"" _r_pid=""20455"">
                        <span class=""earnCalCompanyName middle"">Modine Manufacturing</span>&nbsp;(<a href=""/equities/modine-manufacturing-comp-earnings"" class=""bold middle"" target=""_blank"">MOD</a>)
                    </td>
                    <td class="" pid-20455-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.6379</td>
                    <td class="" pid-20455-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;616.61M</td>
                    <td class=""right"">2.3B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20455""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Omnicell Inc"" _p_pid=""16806"" _r_pid=""16806"">
                        <span class=""earnCalCompanyName middle"">Omnicell</span>&nbsp;(<a href=""/equities/omnicell-earnings"" class=""bold middle"" target=""_blank"">OMCL</a>)
                    </td>
                    <td class="" pid-16806-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.4689</td>
                    <td class="" pid-16806-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;296.79M</td>
                    <td class=""right"">2.25B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16806""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Mercury Systems Inc"" _p_pid=""16663"" _r_pid=""16663"">
                        <span class=""earnCalCompanyName middle"">Mercury</span>&nbsp;(<a href=""/equities/mercury-computer-earnings"" class=""bold middle"" target=""_blank"">MRCY</a>)
                    </td>
                    <td class="" pid-16663-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.1833</td>
                    <td class="" pid-16663-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;218.52M</td>
                    <td class=""right"">2.24B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16663""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Ameresco Inc"" _p_pid=""21050"" _r_pid=""21050"">
                        <span class=""earnCalCompanyName middle"">Ameresco</span>&nbsp;(<a href=""/equities/ameresco-inc-earnings"" class=""bold middle"" target=""_blank"">AMRC</a>)
                    </td>
                    <td class="" pid-21050-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.4636</td>
                    <td class="" pid-21050-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;389.15M</td>
                    <td class=""right"">2.19B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""21050""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Banco Itau Chile"" _p_pid=""32335"" _r_pid=""32335"">
                        <span class=""earnCalCompanyName middle"">Banco Itau Chile</span>&nbsp;(<a href=""/equities/corpbanca-sa-adr-earnings"" class=""bold middle"" target=""_blank"">ITCL</a>)
                    </td>
                    <td class="" pid-32335-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.1625</td>
                    <td class="" pid-32335-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;487.72M</td>
                    <td class=""right"">2.15B</td>
                    <td class=""right time"" data-value=""1""><span class=""marketOpen genToolTip oneliner reverseToolTip"" data-tooltip=""Before market open""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""32335""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Bloomin Brands Inc"" _p_pid=""31171"" _r_pid=""31171"">
                        <span class=""earnCalCompanyName middle"">Bloomin Brands</span>&nbsp;(<a href=""/equities/bloomin-brands-inc-earnings"" class=""bold middle"" target=""_blank"">BLMN</a>)
                    </td>
                    <td class="" pid-31171-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.4382</td>
                    <td class="" pid-31171-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.09B</td>
                    <td class=""right"">2.12B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""31171""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Four Corners Property Trust Inc"" _p_pid=""959886"" _r_pid=""959886"">
                        <span class=""earnCalCompanyName middle"">Four Corners Property Trust Inc</span>&nbsp;(<a href=""/equities/four-corners-property-trust-inc-earnings"" class=""bold middle"" target=""_blank"">FCPT</a>)
                    </td>
                    <td class="" pid-959886-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.2701</td>
                    <td class="" pid-959886-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;63.7M</td>
                    <td class=""right"">2.11B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""959886""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Urban Edge Properties"" _p_pid=""945068"" _r_pid=""945068"">
                        <span class=""earnCalCompanyName middle"">Urban Edge Properties</span>&nbsp;(<a href=""/equities/urban-edge-properties-earnings"" class=""bold middle"" target=""_blank"">UE</a>)
                    </td>
                    <td class="" pid-945068-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.0867</td>
                    <td class="" pid-945068-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;100.5M</td>
                    <td class=""right"">1.93B</td>
                    <td class=""right time"" data-value=""1""><span class=""marketOpen genToolTip oneliner reverseToolTip"" data-tooltip=""Before market open""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""945068""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Huron Consulting Group Inc"" _p_pid=""16310"" _r_pid=""16310"">
                        <span class=""earnCalCompanyName middle"">Huron</span>&nbsp;(<a href=""/equities/huron-consulting-earnings"" class=""bold middle"" target=""_blank"">HURN</a>)
                    </td>
                    <td class="" pid-16310-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.18</td>
                    <td class="" pid-16310-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;331.03M</td>
                    <td class=""right"">1.79B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16310""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Silicon Motion Technology"" _p_pid=""17180"" _r_pid=""17180"">
                        <span class=""earnCalCompanyName middle"">Silicon Motion</span>&nbsp;(<a href=""/equities/silicon-motion-te-earnings"" class=""bold middle"" target=""_blank"">SIMO</a>)
                    </td>
                    <td class="" pid-17180-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.577</td>
                    <td class="" pid-17180-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;164M</td>
                    <td class=""right"">1.73B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""17180""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Liberty Latin America Ltd"" _p_pid=""954881"" _r_pid=""954881"">
                        <span class=""earnCalCompanyName middle"">Liberty Latin America</span>&nbsp;(<a href=""/equities/liberty-global-lilac-a-earnings"" class=""bold middle"" target=""_blank"">LILA</a>)
                    </td>
                    <td class="" pid-954881-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.091</td>
                    <td class="" pid-954881-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.14B</td>
                    <td class=""right"">1.69B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""954881""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Liberty Latin America Ltd Class C"" _p_pid=""954882"" _r_pid=""954882"">
                        <span class=""earnCalCompanyName middle"">Liberty Latin America C</span>&nbsp;(<a href=""/equities/liberty-global-lilac-c-earnings"" class=""bold middle"" target=""_blank"">LILAK</a>)
                    </td>
                    <td class="" pid-954882-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.0656</td>
                    <td class="" pid-954882-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.14B</td>
                    <td class=""right"">1.69B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""954882""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Camping World Holdings Inc"" _p_pid=""991166"" _r_pid=""991166"">
                        <span class=""earnCalCompanyName middle"">Camping World Holdings</span>&nbsp;(<a href=""/equities/camping-world-holdings-inc-earnings"" class=""bold middle"" target=""_blank"">CWH</a>)
                    </td>
                    <td class="" pid-991166-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.3142</td>
                    <td class="" pid-991166-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.74B</td>
                    <td class=""right"">1.69B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""991166""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Openlane Inc"" _p_pid=""20496"" _r_pid=""20496"">
                        <span class=""earnCalCompanyName middle"">Openlane</span>&nbsp;(<a href=""/equities/kar-auction-services-inc-earnings"" class=""bold middle"" target=""_blank"">KAR</a>)
                    </td>
                    <td class="" pid-20496-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.1446</td>
                    <td class="" pid-20496-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;409.44M</td>
                    <td class=""right"">1.64B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20496""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""JBG SMITH Properties"" _p_pid=""1025079"" _r_pid=""1025079"">
                        <span class=""earnCalCompanyName middle"">JBG SMITH Properties</span>&nbsp;(<a href=""/equities/jbg-smith-properties-earnings"" class=""bold middle"" target=""_blank"">JBGS</a>)
                    </td>
                    <td class="" pid-1025079-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.15</td>
                    <td class="" pid-1025079-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;156.89M</td>
                    <td class=""right"">1.64B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1025079""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Veracyte Inc"" _p_pid=""48421"" _r_pid=""48421"">
                        <span class=""earnCalCompanyName middle"">Veracyte Inc</span>&nbsp;(<a href=""/equities/veracyte-inc-earnings"" class=""bold middle"" target=""_blank"">VCYT</a>)
                    </td>
                    <td class="" pid-48421-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.154</td>
                    <td class="" pid-48421-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;84.36M</td>
                    <td class=""right"">1.63B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""48421""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""NeoGenomics Inc"" _p_pid=""40083"" _r_pid=""40083"">
                        <span class=""earnCalCompanyName middle"">NeoGenomics</span>&nbsp;(<a href=""/equities/neogenomics-inc-earnings"" class=""bold middle"" target=""_blank"">NEO</a>)
                    </td>
                    <td class="" pid-40083-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.0856</td>
                    <td class="" pid-40083-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;141.9M</td>
                    <td class=""right"">1.55B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""40083""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""PROS Holdings Inc"" _p_pid=""20894"" _r_pid=""20894"">
                        <span class=""earnCalCompanyName middle"">PROS</span>&nbsp;(<a href=""/equities/pros-holdings-inc-earnings"" class=""bold middle"" target=""_blank"">PRO</a>)
                    </td>
                    <td class="" pid-20894-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.0335</td>
                    <td class="" pid-20894-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;75.94M</td>
                    <td class=""right"">1.51B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20894""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Veeco Instruments Inc"" _p_pid=""17481"" _r_pid=""17481"">
                        <span class=""earnCalCompanyName middle"">Veeco</span>&nbsp;(<a href=""/equities/veeco-instruments-earnings"" class=""bold middle"" target=""_blank"">VECO</a>)
                    </td>
                    <td class="" pid-17481-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.3683</td>
                    <td class="" pid-17481-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;168.94M</td>
                    <td class=""right"">1.49B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""17481""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Impinj Inc"" _p_pid=""985958"" _r_pid=""985958"">
                        <span class=""earnCalCompanyName middle"">Impinj</span>&nbsp;(<a href=""/equities/impinj-inc-earnings"" class=""bold middle"" target=""_blank"">PI</a>)
                    </td>
                    <td class="" pid-985958-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.3757</td>
                    <td class="" pid-985958-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;88.11M</td>
                    <td class=""right"">1.47B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""985958""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Sabre Corpo"" _p_pid=""101911"" _r_pid=""101911"">
                        <span class=""earnCalCompanyName middle"">Sabre Corpo</span>&nbsp;(<a href=""/equities/sabre-corpo-earnings"" class=""bold middle"" target=""_blank"">SABR</a>)
                    </td>
                    <td class="" pid-101911-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.0689</td>
                    <td class="" pid-101911-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;735.64M</td>
                    <td class=""right"">1.47B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""101911""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Herbalife Nutrition Ltd"" _p_pid=""32366"" _r_pid=""32366"">
                        <span class=""earnCalCompanyName middle"">Herbalife</span>&nbsp;(<a href=""/equities/herbalife-earnings"" class=""bold middle"" target=""_blank"">HLF</a>)
                    </td>
                    <td class="" pid-32366-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.7302</td>
                    <td class="" pid-32366-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.24B</td>
                    <td class=""right"">1.43B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""32366""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Myriad Genetics Inc"" _p_pid=""16690"" _r_pid=""16690"">
                        <span class=""earnCalCompanyName middle"">Myriad Genetics</span>&nbsp;(<a href=""/equities/myriad-genetics-earnings"" class=""bold middle"" target=""_blank"">MYGN</a>)
                    </td>
                    <td class="" pid-16690-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.0767</td>
                    <td class="" pid-16690-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;179.32M</td>
                    <td class=""right"">1.36B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16690""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Easterly Government Properties"" _p_pid=""945634"" _r_pid=""945634"">
                        <span class=""earnCalCompanyName middle"">Easterly Government Properties</span>&nbsp;(<a href=""/equities/easterly-government-properties-earnings"" class=""bold middle"" target=""_blank"">DEA</a>)
                    </td>
                    <td class="" pid-945634-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.06</td>
                    <td class="" pid-945634-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;69.48M</td>
                    <td class=""right"">1.27B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""945634""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Xenia Hotels &amp; Resorts Inc"" _p_pid=""945633"" _r_pid=""945633"">
                        <span class=""earnCalCompanyName middle"">Xenia Hotels &amp; Resorts Inc</span>&nbsp;(<a href=""/equities/xenia-hotels---resorts-inc-earnings"" class=""bold middle"" target=""_blank"">XHR</a>)
                    </td>
                    <td class="" pid-945633-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.0581</td>
                    <td class="" pid-945633-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;232.35M</td>
                    <td class=""right"">1.27B</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""945633""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Global Industrial Co"" _p_pid=""21093"" _r_pid=""21093"">
                        <span class=""earnCalCompanyName middle"">Global Industrial Co</span>&nbsp;(<a href=""/equities/systemax-inc-earnings"" class=""bold middle"" target=""_blank"">GIC</a>)
                    </td>
                    <td class="" pid-21093-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.495</td>
                    <td class="" pid-21093-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;316.65M</td>
                    <td class=""right"">1.22B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""21093""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Pathward Financial Inc"" _p_pid=""15645"" _r_pid=""15645"">
                        <span class=""earnCalCompanyName middle"">Pathward Financial</span>&nbsp;(<a href=""/equities/meta-financial-earnings"" class=""bold middle"" target=""_blank"">CASH</a>)
                    </td>
                    <td class="" pid-15645-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.25</td>
                    <td class="" pid-15645-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;155.79M</td>
                    <td class=""right"">1.2B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""15645""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Kayne Anderson MLP Investment Closed Fund"" _p_pid=""960114"" _r_pid=""960114"">
                        <span class=""earnCalCompanyName middle"">Kayne Anderson MLP Invest Closed</span>&nbsp;(<a href=""/equities/kayne-anderson-mlp-invest-closed-earnings"" class=""bold middle"" target=""_blank"">KYN</a>)
                    </td>
                    <td class="" pid-960114-2023-10-31-082023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-960114-2023-10-31-082023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">1.19B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""960114""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Nabors Industries Ltd"" _p_pid=""8284"" _r_pid=""8284"">
                        <span class=""earnCalCompanyName middle"">Nabors Industries</span>&nbsp;(<a href=""/equities/nabors-inds-earnings"" class=""bold middle"" target=""_blank"">NBR</a>)
                    </td>
                    <td class="" pid-8284-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.028</td>
                    <td class="" pid-8284-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;757.83M</td>
                    <td class=""right"">1.17B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""8284""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""John B Sanfilippo &amp; Son Inc"" _p_pid=""16418"" _r_pid=""16418"">
                        <span class=""earnCalCompanyName middle"">John B Sanfilippo&amp;Son</span>&nbsp;(<a href=""/equities/john-b.-sanfilipp-earnings"" class=""bold middle"" target=""_blank"">JBSS</a>)
                    </td>
                    <td class="" pid-16418-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.25</td>
                    <td class="" pid-16418-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;233.10M</td>
                    <td class=""right"">1.13B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16418""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""A10 Network"" _p_pid=""101850"" _r_pid=""101850"">
                        <span class=""earnCalCompanyName middle"">A10 Network</span>&nbsp;(<a href=""/equities/a10-network-earnings"" class=""bold middle"" target=""_blank"">ATEN</a>)
                    </td>
                    <td class="" pid-101850-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.2193</td>
                    <td class="" pid-101850-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;74.35M</td>
                    <td class=""right"">1.11B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""101850""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Chegg Inc"" _p_pid=""48369"" _r_pid=""48369"">
                        <span class=""earnCalCompanyName middle"">Chegg Inc</span>&nbsp;(<a href=""/equities/chegg-inc-earnings"" class=""bold middle"" target=""_blank"">CHGG</a>)
                    </td>
                    <td class="" pid-48369-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.1772</td>
                    <td class="" pid-48369-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;152.17M</td>
                    <td class=""right"">1.03B</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""48369""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Columbus McKinnon Corporation"" _p_pid=""15746"" _r_pid=""15746"">
                        <span class=""earnCalCompanyName middle"">Columbus McKinnon</span>&nbsp;(<a href=""/equities/columbus-mckinnon-earnings"" class=""bold middle"" target=""_blank"">CMCO</a>)
                    </td>
                    <td class="" pid-15746-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.7025</td>
                    <td class="" pid-15746-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;255.06M</td>
                    <td class=""right"">980.74M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""15746""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Armada Hflr Pr"" _p_pid=""41192"" _r_pid=""41192"">
                        <span class=""earnCalCompanyName middle"">Armada Hflr Pr</span>&nbsp;(<a href=""/equities/armada-hflr-pr-earnings"" class=""bold middle"" target=""_blank"">AHH</a>)
                    </td>
                    <td class="" pid-41192-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.0925</td>
                    <td class="" pid-41192-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;60.44M</td>
                    <td class=""right"">968.03M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""41192""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""VSE Corporation"" _p_pid=""17521"" _r_pid=""17521"">
                        <span class=""earnCalCompanyName middle"">VSE Corporation</span>&nbsp;(<a href=""/equities/vse-corp-earnings"" class=""bold middle"" target=""_blank"">VSEC</a>)
                    </td>
                    <td class="" pid-17521-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.698</td>
                    <td class="" pid-17521-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;210.75M</td>
                    <td class=""right"">896.92M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""17521""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""KKR Real Estate Finance Trust Inc"" _p_pid=""1009131"" _r_pid=""1009131"">
                        <span class=""earnCalCompanyName middle"">KKR Real Estate</span>&nbsp;(<a href=""/equities/kkr-real-estate-finance-trust-inc-earnings"" class=""bold middle"" target=""_blank"">KREF</a>)
                    </td>
                    <td class="" pid-1009131-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.0242</td>
                    <td class="" pid-1009131-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;47.21M</td>
                    <td class=""right"">852.08M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1009131""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Nexpoint Residential Trust Inc"" _p_pid=""949598"" _r_pid=""949598"">
                        <span class=""earnCalCompanyName middle"">Nexpoint Residential Trust Inc</span>&nbsp;(<a href=""/equities/nexpoint-residential-trust-inc-earnings"" class=""bold middle"" target=""_blank"">NXRT</a>)
                    </td>
                    <td class="" pid-949598-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.172</td>
                    <td class="" pid-949598-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;70.55M</td>
                    <td class=""right"">851.47M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""949598""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Sun Country Airlines Holdings Inc"" _p_pid=""1171385"" _r_pid=""1171385"">
                        <span class=""earnCalCompanyName middle"">Sun Country Airlines Holdings</span>&nbsp;(<a href=""/equities/sun-country-airlines-holdings-earnings"" class=""bold middle"" target=""_blank"">SNCY</a>)
                    </td>
                    <td class="" pid-1171385-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.1699</td>
                    <td class="" pid-1171385-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;244.31M</td>
                    <td class=""right"">814.98M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1171385""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Hyster-Yale Materials Handling Inc"" _p_pid=""41263"" _r_pid=""41263"">
                        <span class=""earnCalCompanyName middle"">Hyster-Yale Materials Handling</span>&nbsp;(<a href=""/equities/hystr-yl-mrl-hnd-earnings"" class=""bold middle"" target=""_blank"">HY</a>)
                    </td>
                    <td class="" pid-41263-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.72</td>
                    <td class="" pid-41263-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;925.1M</td>
                    <td class=""right"">754.08M</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""41263""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""UNITIL Corporation"" _p_pid=""24354"" _r_pid=""24354"">
                        <span class=""earnCalCompanyName middle"">UNITIL</span>&nbsp;(<a href=""/equities/unitil-corp-earnings"" class=""bold middle"" target=""_blank"">UTL</a>)
                    </td>
                    <td class="" pid-24354-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.06</td>
                    <td class="" pid-24354-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;118M</td>
                    <td class=""right"">742.02M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""24354""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Cassava Sciences Inc"" _p_pid=""16984"" _r_pid=""16984"">
                        <span class=""earnCalCompanyName middle"">Cassava Sciences</span>&nbsp;(<a href=""/equities/pain-therapeutics-earnings"" class=""bold middle"" target=""_blank"">SAVA</a>)
                    </td>
                    <td class="" pid-16984-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.68</td>
                    <td class="" pid-16984-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.00</td>
                    <td class=""right"">736.15M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16984""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""BlueLinx Holdings Inc"" _p_pid=""20613"" _r_pid=""20613"">
                        <span class=""earnCalCompanyName middle"">BlueLinx</span>&nbsp;(<a href=""/equities/bluelinx-holdings-inc-earnings"" class=""bold middle"" target=""_blank"">BXC</a>)
                    </td>
                    <td class="" pid-20613-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;2.97</td>
                    <td class="" pid-20613-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;842.69M</td>
                    <td class=""right"">732.79M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20613""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""MarineMax Inc"" _p_pid=""21029"" _r_pid=""21029"">
                        <span class=""earnCalCompanyName middle"">MarineMax</span>&nbsp;(<a href=""/equities/marinemax-inc-earnings"" class=""bold middle"" target=""_blank"">HZO</a>)
                    </td>
                    <td class="" pid-21029-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.7309</td>
                    <td class="" pid-21029-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;531.42M</td>
                    <td class=""right"">690.4M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""21029""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""North American Construction Group Ltd"" _p_pid=""31153"" _r_pid=""20181"">
                        <span class=""earnCalCompanyName middle"">North American Construction</span>&nbsp;(<a href=""/equities/north-american-energy-partners-earnings?cid=20181"" class=""bold middle"" target=""_blank"">NOA</a>)
                    </td>
                    <td class="" pid-20181-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.5938</td>
                    <td class="" pid-20181-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;211M</td>
                    <td class=""right"">601.36M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20181""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Enhabit Inc"" _p_pid=""1192421"" _r_pid=""1192421"">
                        <span class=""earnCalCompanyName middle"">Enhabit</span>&nbsp;(<a href=""/equities/enhabit-earnings"" class=""bold middle"" target=""_blank"">EHAB</a>)
                    </td>
                    <td class="" pid-1192421-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.079</td>
                    <td class="" pid-1192421-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;265.24M</td>
                    <td class=""right"">601.22M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1192421""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Enviri Corp"" _p_pid=""21118"" _r_pid=""21118"">
                        <span class=""earnCalCompanyName middle"">Enviri</span>&nbsp;(<a href=""/equities/harsco-corp-earnings"" class=""bold middle"" target=""_blank"">NVRI</a>)
                    </td>
                    <td class="" pid-21118-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.0318</td>
                    <td class="" pid-21118-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;505.88M</td>
                    <td class=""right"">598.16M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""21118""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Amerant Bancorp Inc Class A"" _p_pid=""1095928"" _r_pid=""1095928"">
                        <span class=""earnCalCompanyName middle"">Amerant Bancorp A</span>&nbsp;(<a href=""/equities/mercantil-bank-a-earnings"" class=""bold middle"" target=""_blank"">AMTB</a>)
                    </td>
                    <td class="" pid-1095928-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.616</td>
                    <td class="" pid-1095928-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;97.38M</td>
                    <td class=""right"">592.55M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1095928""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""James River Group Holdings Ltd"" _p_pid=""943135"" _r_pid=""943135"">
                        <span class=""earnCalCompanyName middle"">James River Group</span>&nbsp;(<a href=""/equities/james-river-group-holdings-ltd-earnings"" class=""bold middle"" target=""_blank"">JRVR</a>)
                    </td>
                    <td class="" pid-943135-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.4834</td>
                    <td class="" pid-943135-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;207.57M</td>
                    <td class=""right"">586.87M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""943135""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Newpark Resources Inc"" _p_pid=""20652"" _r_pid=""20652"">
                        <span class=""earnCalCompanyName middle"">Newpark Resources</span>&nbsp;(<a href=""/equities/newpark-resources-inc-earnings"" class=""bold middle"" target=""_blank"">NR</a>)
                    </td>
                    <td class="" pid-20652-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.0672</td>
                    <td class="" pid-20652-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;193.4M</td>
                    <td class=""right"">570.36M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20652""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Kaman Corporation"" _p_pid=""16442"" _r_pid=""16442"">
                        <span class=""earnCalCompanyName middle"">Kaman</span>&nbsp;(<a href=""/equities/kaman-corp-earnings"" class=""bold middle"" target=""_blank"">KAMN</a>)
                    </td>
                    <td class="" pid-16442-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.08</td>
                    <td class="" pid-16442-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;180.43M</td>
                    <td class=""right"">556.77M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16442""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""TPG RE Finance Trust Inc"" _p_pid=""1024877"" _r_pid=""1024877"">
                        <span class=""earnCalCompanyName middle"">TPG RE Finance</span>&nbsp;(<a href=""/equities/tpg-re-finance-trust-inc-earnings"" class=""bold middle"" target=""_blank"">TRTX</a>)
                    </td>
                    <td class="" pid-1024877-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.12</td>
                    <td class="" pid-1024877-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;32.53M</td>
                    <td class=""right"">555M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1024877""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Acco Brands Corporation"" _p_pid=""20908"" _r_pid=""20908"">
                        <span class=""earnCalCompanyName middle"">Acco Brands</span>&nbsp;(<a href=""/equities/acco-brands-corp-earnings"" class=""bold middle"" target=""_blank"">ACCO</a>)
                    </td>
                    <td class="" pid-20908-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.2567</td>
                    <td class="" pid-20908-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;476.36M</td>
                    <td class=""right"">552.42M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20908""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Gladstone Commercial Corporation"" _p_pid=""16204"" _r_pid=""16204"">
                        <span class=""earnCalCompanyName middle"">Gladstone Commercial</span>&nbsp;(<a href=""/equities/gladstone-commerc-earnings"" class=""bold middle"" target=""_blank"">GOOD</a>)
                    </td>
                    <td class="" pid-16204-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.05</td>
                    <td class="" pid-16204-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;36.9M</td>
                    <td class=""right"">501.85M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16204""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Whitestone REIT"" _p_pid=""24421"" _r_pid=""24421"">
                        <span class=""earnCalCompanyName middle"">Whitestone</span>&nbsp;(<a href=""/equities/whitestone-reit-earnings"" class=""bold middle"" target=""_blank"">WSR</a>)
                    </td>
                    <td class="" pid-24421-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.14</td>
                    <td class="" pid-24421-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;36.69M</td>
                    <td class=""right"">487.08M</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""24421""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Haverty Furniture Companies Inc"" _p_pid=""20234"" _r_pid=""20234"">
                        <span class=""earnCalCompanyName middle"">Haverty Furniture</span>&nbsp;(<a href=""/equities/haverty-furniture-companies-inc-earnings"" class=""bold middle"" target=""_blank"">HVT</a>)
                    </td>
                    <td class="" pid-20234-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.14</td>
                    <td class="" pid-20234-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;248.66M</td>
                    <td class=""right"">476.61M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20234""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Haverty Furniture Companies A"" _p_pid=""20293"" _r_pid=""20293"">
                        <span class=""earnCalCompanyName middle"">Haverty Furniture A</span>&nbsp;(<a href=""/equities/haverty-furniture-companies-a-earnings"" class=""bold middle"" target=""_blank"">HVTa</a>)
                    </td>
                    <td class="" pid-20293-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.14</td>
                    <td class="" pid-20293-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;248.66M</td>
                    <td class=""right"">476.61M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20293""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Denny’s Corp"" _p_pid=""15899"" _r_pid=""15899"">
                        <span class=""earnCalCompanyName middle"">Denny’s</span>&nbsp;(<a href=""/equities/dennys-corp-earnings"" class=""bold middle"" target=""_blank"">DENN</a>)
                    </td>
                    <td class="" pid-15899-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.1529</td>
                    <td class="" pid-15899-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;117.27M</td>
                    <td class=""right"">470.34M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""15899""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Carriage Services Inc"" _p_pid=""20627"" _r_pid=""20627"">
                        <span class=""earnCalCompanyName middle"">Carriage Services</span>&nbsp;(<a href=""/equities/carriage-services-inc-earnings"" class=""bold middle"" target=""_blank"">CSV</a>)
                    </td>
                    <td class="" pid-20627-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.525</td>
                    <td class="" pid-20627-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;93.83M</td>
                    <td class=""right"">450.92M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20627""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""OneSpan Inc"" _p_pid=""17480"" _r_pid=""17480"">
                        <span class=""earnCalCompanyName middle"">Onespan</span>&nbsp;(<a href=""/equities/vasco-data-securi-earnings"" class=""bold middle"" target=""_blank"">OSPN</a>)
                    </td>
                    <td class="" pid-17480-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.0207</td>
                    <td class="" pid-17480-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;60.17M</td>
                    <td class=""right"">443.36M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""17480""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Gladstone Investment Corporation"" _p_pid=""24369"" _r_pid=""24369"">
                        <span class=""earnCalCompanyName middle"">Gladstone Investment</span>&nbsp;(<a href=""/equities/gladstone-investment-corp-earnings"" class=""bold middle"" target=""_blank"">GAIN</a>)
                    </td>
                    <td class="" pid-24369-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.274</td>
                    <td class="" pid-24369-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;22.16M</td>
                    <td class=""right"">431.83M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""24369""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Camden National Corporation"" _p_pid=""15627"" _r_pid=""15627"">
                        <span class=""earnCalCompanyName middle"">Camden</span>&nbsp;(<a href=""/equities/camden-national-earnings"" class=""bold middle"" target=""_blank"">CAC</a>)
                    </td>
                    <td class="" pid-15627-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.7225</td>
                    <td class="" pid-15627-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;41.23M</td>
                    <td class=""right"">410.45M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""15627""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Universal Insurance Holdings Inc"" _p_pid=""24384"" _r_pid=""24384"">
                        <span class=""earnCalCompanyName middle"">Universal Insurance</span>&nbsp;(<a href=""/equities/universal-insurance-holdings-inc-earnings"" class=""bold middle"" target=""_blank"">UVE</a>)
                    </td>
                    <td class="" pid-24384-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.28</td>
                    <td class="" pid-24384-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;515.7M</td>
                    <td class=""right"">402.77M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""24384""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Nautilus Biotechnology Inc"" _p_pid=""1174276"" _r_pid=""1174276"">
                        <span class=""earnCalCompanyName middle"">Nautilus Biotechnology</span>&nbsp;(<a href=""/equities/arya-sciences-acquisition-iii-earnings"" class=""bold middle"" target=""_blank"">NAUT</a>)
                    </td>
                    <td class="" pid-1174276-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.18</td>
                    <td class="" pid-1174276-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.00</td>
                    <td class=""right"">398.53M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1174276""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""HomeTrust Bancshares Inc"" _p_pid=""31044"" _r_pid=""31044"">
                        <span class=""earnCalCompanyName middle"">HomeTrust</span>&nbsp;(<a href=""/equities/hometrust-bancshares-inc-earnings"" class=""bold middle"" target=""_blank"">HTBI</a>)
                    </td>
                    <td class="" pid-31044-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.7552</td>
                    <td class="" pid-31044-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;49.82M</td>
                    <td class=""right"">373.87M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""31044""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Community Health Systems Inc"" _p_pid=""39249"" _r_pid=""39249"">
                        <span class=""earnCalCompanyName middle"">Community Health Systems</span>&nbsp;(<a href=""/equities/community-health-earnings"" class=""bold middle"" target=""_blank"">CYH</a>)
                    </td>
                    <td class="" pid-39249-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.141</td>
                    <td class="" pid-39249-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;3.07B</td>
                    <td class=""right"">363.34M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""39249""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Flushing Financial Corporation"" _p_pid=""16088"" _r_pid=""16088"">
                        <span class=""earnCalCompanyName middle"">Flushing</span>&nbsp;(<a href=""/equities/flushing-financial-earnings"" class=""bold middle"" target=""_blank"">FFIC</a>)
                    </td>
                    <td class="" pid-16088-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.2375</td>
                    <td class="" pid-16088-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;46.75M</td>
                    <td class=""right"">362.63M</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16088""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Uniqure NV"" _p_pid=""100212"" _r_pid=""100212"">
                        <span class=""earnCalCompanyName middle"">Uniqure NV</span>&nbsp;(<a href=""/equities/uniqure-nv-earnings"" class=""bold middle"" target=""_blank"">QURE</a>)
                    </td>
                    <td class="" pid-100212-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.8246</td>
                    <td class="" pid-100212-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;32.64M</td>
                    <td class=""right"">338.22M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""100212""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""FTAI Infrastructure LLC"" _p_pid=""1193018"" _r_pid=""1193018"">
                        <span class=""earnCalCompanyName middle"">FTAI Infra LLC</span>&nbsp;(<a href=""/equities/ftai-infra-llc-earnings"" class=""bold middle"" target=""_blank"">FIP</a>)
                    </td>
                    <td class="" pid-1193018-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.21</td>
                    <td class="" pid-1193018-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;101M</td>
                    <td class=""right"">318.31M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1193018""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Tusimple Holdings Inc"" _p_pid=""1172383"" _r_pid=""1172383"">
                        <span class=""earnCalCompanyName middle"">Tusimple Holdings</span>&nbsp;(<a href=""/equities/tusimple-holdings-earnings"" class=""bold middle"" target=""_blank"">TSP</a>)
                    </td>
                    <td class="" pid-1172383-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.625</td>
                    <td class="" pid-1172383-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;3.27M</td>
                    <td class=""right"">316.16M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1172383""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""AudioCodes Ltd"" _p_pid=""10973"" _r_pid=""15499"">
                        <span class=""earnCalCompanyName middle"">AudioCodes</span>&nbsp;(<a href=""/equities/audiocodes-earnings?cid=15499"" class=""bold middle"" target=""_blank"">AUDC</a>)
                    </td>
                    <td class="" pid-15499-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.186</td>
                    <td class="" pid-15499-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;61.7M</td>
                    <td class=""right"">314.57M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""15499""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Zynex Inc"" _p_pid=""1122406"" _r_pid=""1122406"">
                        <span class=""earnCalCompanyName middle"">Zynex</span>&nbsp;(<a href=""/equities/zynex-earnings"" class=""bold middle"" target=""_blank"">ZYXI</a>)
                    </td>
                    <td class="" pid-1122406-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.125</td>
                    <td class="" pid-1122406-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;49.85M</td>
                    <td class=""right"">295.89M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1122406""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Intrepid Potash Inc"" _p_pid=""20760"" _r_pid=""20760"">
                        <span class=""earnCalCompanyName middle"">Intrepid Potash</span>&nbsp;(<a href=""/equities/intrepid-potash-inc-earnings"" class=""bold middle"" target=""_blank"">IPI</a>)
                    </td>
                    <td class="" pid-20760-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.1035</td>
                    <td class="" pid-20760-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;44.97M</td>
                    <td class=""right"">295.09M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20760""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Bandwidth Inc"" _p_pid=""1054803"" _r_pid=""1054803"">
                        <span class=""earnCalCompanyName middle"">Bandwidth</span>&nbsp;(<a href=""/equities/bandwidth-earnings"" class=""bold middle"" target=""_blank"">BAND</a>)
                    </td>
                    <td class="" pid-1054803-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.2051</td>
                    <td class="" pid-1054803-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;147.7M</td>
                    <td class=""right"">292.81M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1054803""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Regional Management Corp"" _p_pid=""29668"" _r_pid=""29668"">
                        <span class=""earnCalCompanyName middle"">Regional Management</span>&nbsp;(<a href=""/equities/regional-management-corp-earnings"" class=""bold middle"" target=""_blank"">RM</a>)
                    </td>
                    <td class="" pid-29668-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.9717</td>
                    <td class="" pid-29668-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;137.53M</td>
                    <td class=""right"">263.34M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""29668""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Accuray Incorporated"" _p_pid=""15438"" _r_pid=""15438"">
                        <span class=""earnCalCompanyName middle"">Accuray</span>&nbsp;(<a href=""/equities/accuray-incorped-earnings"" class=""bold middle"" target=""_blank"">ARAY</a>)
                    </td>
                    <td class="" pid-15438-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.05</td>
                    <td class="" pid-15438-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;97.62M</td>
                    <td class=""right"">260.79M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""15438""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Skywater Technology Inc"" _p_pid=""1172517"" _r_pid=""1172517"">
                        <span class=""earnCalCompanyName middle"">Skywater Technology</span>&nbsp;(<a href=""/equities/cmi-acquisition-llc-earnings"" class=""bold middle"" target=""_blank"">SKYT</a>)
                    </td>
                    <td class="" pid-1172517-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.1225</td>
                    <td class="" pid-1172517-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;66.8M</td>
                    <td class=""right"">256.41M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1172517""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Quad Graphics Inc"" _p_pid=""20895"" _r_pid=""20895"">
                        <span class=""earnCalCompanyName middle"">Quad Graphics</span>&nbsp;(<a href=""/equities/quad-graphics-inc-earnings"" class=""bold middle"" target=""_blank"">QUAD</a>)
                    </td>
                    <td class="" pid-20895-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.04</td>
                    <td class="" pid-20895-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;769.3M</td>
                    <td class=""right"">254.99M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20895""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Mayville Engineering Co Inc"" _p_pid=""1129438"" _r_pid=""1129438"">
                        <span class=""earnCalCompanyName middle"">Mayville Engineering</span>&nbsp;(<a href=""/equities/mayville-engineering-co-inc-earnings"" class=""bold middle"" target=""_blank"">MEC</a>)
                    </td>
                    <td class="" pid-1129438-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.23</td>
                    <td class="" pid-1129438-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;159.92M</td>
                    <td class=""right"">223.91M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1129438""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Unisys Corporation"" _p_pid=""8307"" _r_pid=""8307"">
                        <span class=""earnCalCompanyName middle"">Unisys</span>&nbsp;(<a href=""/equities/unisys-corp-earnings"" class=""bold middle"" target=""_blank"">UIS</a>)
                    </td>
                    <td class="" pid-8307-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.4667</td>
                    <td class="" pid-8307-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;442.07M</td>
                    <td class=""right"">220.61M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""8307""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""LB Foster Company"" _p_pid=""16138"" _r_pid=""16138"">
                        <span class=""earnCalCompanyName middle"">LB Foster</span>&nbsp;(<a href=""/equities/l.b.-foster-compa-earnings"" class=""bold middle"" target=""_blank"">FSTR</a>)
                    </td>
                    <td class="" pid-16138-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.205</td>
                    <td class="" pid-16138-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;138.65M</td>
                    <td class=""right"">211.84M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16138""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Virtus Convertible &amp; Income Fund II"" _p_pid=""960229"" _r_pid=""960229"">
                        <span class=""earnCalCompanyName middle"">Virtus Convertible Fund II</span>&nbsp;(<a href=""/equities/allianzgi-convert-income-ii-closed-earnings"" class=""bold middle"" target=""_blank"">NCZ</a>)
                    </td>
                    <td class="" pid-960229-2023-10-31-082023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-960229-2023-10-31-082023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">207.03M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""960229""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Companhia Brasileira de Distribuicao"" _p_pid=""29606"" _r_pid=""29606"">
                        <span class=""earnCalCompanyName middle"">Companhia Brasileira de Distribuicao</span>&nbsp;(<a href=""/equities/comp-brasileira-de-distribuicao-earnings"" class=""bold middle"" target=""_blank"">CBD</a>)
                    </td>
                    <td class="" pid-29606-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.2236</td>
                    <td class="" pid-29606-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;1.01B</td>
                    <td class=""right"">193.61M</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""29606""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Franklin Street Properties Corp"" _p_pid=""24385"" _r_pid=""24385"">
                        <span class=""earnCalCompanyName middle"">Franklin Street Properties</span>&nbsp;(<a href=""/equities/franklin-street-properties-corp-earnings"" class=""bold middle"" target=""_blank"">FSP</a>)
                    </td>
                    <td class="" pid-24385-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.065</td>
                    <td class="" pid-24385-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;36.24M</td>
                    <td class=""right"">188.24M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""24385""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Semler Scientific Inc"" _p_pid=""100166"" _r_pid=""100166"">
                        <span class=""earnCalCompanyName middle"">Semler Scientifc</span>&nbsp;(<a href=""/equities/semler-scientifc-earnings"" class=""bold middle"" target=""_blank"">SMLR</a>)
                    </td>
                    <td class="" pid-100166-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.62</td>
                    <td class="" pid-100166-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;15.2M</td>
                    <td class=""right"">184.13M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""100166""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Medallion Financial Corp"" _p_pid=""17312"" _r_pid=""17312"">
                        <span class=""earnCalCompanyName middle"">Medallion</span>&nbsp;(<a href=""/equities/medallion-financial-earnings"" class=""bold middle"" target=""_blank"">MFIN</a>)
                    </td>
                    <td class="" pid-17312-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.3633</td>
                    <td class="" pid-17312-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;43.06M</td>
                    <td class=""right"">166.99M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""17312""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Sequans Communications SA"" _p_pid=""20467"" _r_pid=""20467"">
                        <span class=""earnCalCompanyName middle"">Sequans Communications</span>&nbsp;(<a href=""/equities/sequans-communications-sa-earnings"" class=""bold middle"" target=""_blank"">SQNS</a>)
                    </td>
                    <td class="" pid-20467-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.098</td>
                    <td class="" pid-20467-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;11.55M</td>
                    <td class=""right"">166.87M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20467""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Oxford Square Capital Corp"" _p_pid=""24379"" _r_pid=""24379"">
                        <span class=""earnCalCompanyName middle"">Oxford Square</span>&nbsp;(<a href=""/equities/ticc-capital-corp-earnings"" class=""bold middle"" target=""_blank"">OXSQ</a>)
                    </td>
                    <td class="" pid-24379-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.14</td>
                    <td class="" pid-24379-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;12.6M</td>
                    <td class=""right"">164.72M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""24379""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Seven Hills Realty Trust"" _p_pid=""1180407"" _r_pid=""1180407"">
                        <span class=""earnCalCompanyName middle"">Seven Hills Realty Trust</span>&nbsp;(<a href=""/equities/seven-hills-realty-trust-earnings"" class=""bold middle"" target=""_blank"">SEVN</a>)
                    </td>
                    <td class="" pid-1180407-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.35</td>
                    <td class="" pid-1180407-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;9.03M</td>
                    <td class=""right"">164.55M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1180407""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Big 5 Sporting Goods Corporation"" _p_pid=""15555"" _r_pid=""15555"">
                        <span class=""earnCalCompanyName middle"">Big 5</span>&nbsp;(<a href=""/equities/big-5-sporting-go-earnings"" class=""bold middle"" target=""_blank"">BGFV</a>)
                    </td>
                    <td class="" pid-15555-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.22</td>
                    <td class="" pid-15555-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;244.19M</td>
                    <td class=""right"">153.69M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""15555""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Unifi Inc"" _p_pid=""24358"" _r_pid=""24358"">
                        <span class=""earnCalCompanyName middle"">Unifi</span>&nbsp;(<a href=""/equities/unifi-inc-earnings"" class=""bold middle"" target=""_blank"">UFI</a>)
                    </td>
                    <td class="" pid-24358-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.27</td>
                    <td class="" pid-24358-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;155.38M</td>
                    <td class=""right"">123.88M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""24358""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""CB Financial Services Inc"" _p_pid=""942495"" _r_pid=""942495"">
                        <span class=""earnCalCompanyName middle"">CB Financial Services Inc</span>&nbsp;(<a href=""/equities/cb-financial-services-inc-earnings"" class=""bold middle"" target=""_blank"">CBFV</a>)
                    </td>
                    <td class="" pid-942495-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.575</td>
                    <td class="" pid-942495-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;13.85M</td>
                    <td class=""right"">109.73M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""942495""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Rocky Brands Inc"" _p_pid=""17028"" _r_pid=""17028"">
                        <span class=""earnCalCompanyName middle"">Rocky Brands</span>&nbsp;(<a href=""/equities/rocky-brands-earnings"" class=""bold middle"" target=""_blank"">RCKY</a>)
                    </td>
                    <td class="" pid-17028-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.62</td>
                    <td class="" pid-17028-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;117.96M</td>
                    <td class=""right"">109.24M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""17028""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Esperion Therapeutics Inc"" _p_pid=""41302"" _r_pid=""41302"">
                        <span class=""earnCalCompanyName middle"">Esperion</span>&nbsp;(<a href=""/equities/esperion-th-earnings"" class=""bold middle"" target=""_blank"">ESPR</a>)
                    </td>
                    <td class="" pid-41302-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.4313</td>
                    <td class="" pid-41302-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;30M</td>
                    <td class=""right"">105.95M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""41302""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Container Store Group Inc"" _p_pid=""48368"" _r_pid=""48368"">
                        <span class=""earnCalCompanyName middle"">Container Store</span>&nbsp;(<a href=""/equities/container-store-earnings"" class=""bold middle"" target=""_blank"">TCS</a>)
                    </td>
                    <td class="" pid-48368-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.0833</td>
                    <td class="" pid-48368-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;228.84M</td>
                    <td class=""right"">103.81M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""48368""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Glatfelter"" _p_pid=""21162"" _r_pid=""21162"">
                        <span class=""earnCalCompanyName middle"">Glatfelter</span>&nbsp;(<a href=""/equities/glatfelter-earnings"" class=""bold middle"" target=""_blank"">GLT</a>)
                    </td>
                    <td class="" pid-21162-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.08</td>
                    <td class="" pid-21162-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;390.9M</td>
                    <td class=""right"">99.1M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""21162""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Renalytix AI"" _p_pid=""1163349"" _r_pid=""1163349"">
                        <span class=""earnCalCompanyName middle"">Renalytix AI Nas</span>&nbsp;(<a href=""/equities/renalytix-ai-plc-earnings"" class=""bold middle"" target=""_blank"">RNLX</a>)
                    </td>
                    <td class="" pid-1163349-2023-10-31-062023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.1179</td>
                    <td class="" pid-1163349-2023-10-31-062023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;823.33K</td>
                    <td class=""right"">98.86M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1163349""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Landmark Bancorp Inc"" _p_pid=""16484"" _r_pid=""16484"">
                        <span class=""earnCalCompanyName middle"">Landmark</span>&nbsp;(<a href=""/equities/landmark-bancorp-earnings"" class=""bold middle"" target=""_blank"">LARK</a>)
                    </td>
                    <td class="" pid-16484-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-16484-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">97.89M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16484""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Provident Financial Holdings Inc"" _p_pid=""16963"" _r_pid=""16963"">
                        <span class=""earnCalCompanyName middle"">Provident</span>&nbsp;(<a href=""/equities/provident-financial-holdings-earnings"" class=""bold middle"" target=""_blank"">PROV</a>)
                    </td>
                    <td class="" pid-16963-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.3033</td>
                    <td class="" pid-16963-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;10.37M</td>
                    <td class=""right"">90.12M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16963""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Ashford Hospitality Trust Inc"" _p_pid=""20279"" _r_pid=""20279"">
                        <span class=""earnCalCompanyName middle"">Ashford Hospitality</span>&nbsp;(<a href=""/equities/ashford-hospitality-trust-inc-earnings"" class=""bold middle"" target=""_blank"">AHT</a>)
                    </td>
                    <td class="" pid-20279-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-1.32</td>
                    <td class="" pid-20279-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;321.52M</td>
                    <td class=""right"">82.62M</td>
                    <td class=""right time"" data-value=""3""><span class=""marketClosed genToolTip oneliner reverseToolTip"" data-tooltip=""After market close""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""20279""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Argo Blockchain PLC ADR"" _p_pid=""1177521"" _r_pid=""1177521"">
                        <span class=""earnCalCompanyName middle"">Argo Blockchain ADR</span>&nbsp;(<a href=""/equities/argo-blockchain-adr-earnings"" class=""bold middle"" target=""_blank"">ARBK</a>)
                    </td>
                    <td class="" pid-1177521-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.1468</td>
                    <td class="" pid-1177521-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;14.7M</td>
                    <td class=""right"">52.42M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1177521""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Argo Blockchain PLC"" _p_pid=""1090524"" _r_pid=""1168947"">
                        <span class=""earnCalCompanyName middle"">Argo Blockchain</span>&nbsp;(<a href=""/equities/argo-blockchain-earnings?cid=1168947"" class=""bold middle"" target=""_blank"">ARBKF</a>)
                    </td>
                    <td class="" pid-1168947-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.0147</td>
                    <td class="" pid-1168947-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;14.7M</td>
                    <td class=""right"">52.42M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""1168947""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Healthier Choices Management Corp."" _p_pid=""102926"" _r_pid=""102926"">
                        <span class=""earnCalCompanyName middle"">Healthier Choices Management</span>&nbsp;(<a href=""/equities/vapor-corp-earnings"" class=""bold middle"" target=""_blank"">HCMC</a>)
                    </td>
                    <td class="" pid-102926-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-102926-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">47.53M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""102926""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Key Tronic Corporation"" _p_pid=""16474"" _r_pid=""16474"">
                        <span class=""earnCalCompanyName middle"">Key Tronic</span>&nbsp;(<a href=""/equities/key-tronic-corp-earnings"" class=""bold middle"" target=""_blank"">KTCC</a>)
                    </td>
                    <td class="" pid-16474-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-16474-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">47.24M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""16474""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Comstock Mining Inc"" _p_pid=""30825"" _r_pid=""30825"">
                        <span class=""earnCalCompanyName middle"">Comstock Mining</span>&nbsp;(<a href=""/equities/comstock-mining-earnings"" class=""bold middle"" target=""_blank"">LODE</a>)
                    </td>
                    <td class="" pid-30825-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;0.01</td>
                    <td class="" pid-30825-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;15.68M</td>
                    <td class=""right"">43.17M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""30825""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Gold Resource Corporation"" _p_pid=""24409"" _r_pid=""24409"">
                        <span class=""earnCalCompanyName middle"">Gold Resource</span>&nbsp;(<a href=""/equities/gold-resource-corp-earnings"" class=""bold middle"" target=""_blank"">GORO</a>)
                    </td>
                    <td class="" pid-24409-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.02</td>
                    <td class="" pid-24409-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;21.1M</td>
                    <td class=""right"">39.9M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""24409""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Electra Battery Materials Corp"" _p_pid=""991158"" _r_pid=""1188001"">
                        <span class=""earnCalCompanyName middle"">Electra Battery Materials</span>&nbsp;(<a href=""/equities/first-cobalt-corp-earnings?cid=1188001"" class=""bold middle"" target=""_blank"">ELBM</a>)
                    </td>
                    <td class="" pid-1188001-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.0531</td>
                    <td class="" pid-1188001-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">35.2M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""1188001""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Regis Corporation"" _p_pid=""21179"" _r_pid=""21179"">
                        <span class=""earnCalCompanyName middle"">Regis</span>&nbsp;(<a href=""/equities/regis-corp-earnings"" class=""bold middle"" target=""_blank"">RGS</a>)
                    </td>
                    <td class="" pid-21179-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;-0.05</td>
                    <td class="" pid-21179-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;70.20M</td>
                    <td class=""right"">34.87M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""21179""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Dallasnews Corp"" _p_pid=""21049"" _r_pid=""21049"">
                        <span class=""earnCalCompanyName middle"">Dallasnews</span>&nbsp;(<a href=""/equities/ah-belo-corp-earnings"" class=""bold middle"" target=""_blank"">DALN</a>)
                    </td>
                    <td class="" pid-21049-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-21049-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">25.31M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td class=""alert js-injected-user-alert-container"" data-pair-id=""21049""><span class=""js-plus-icon alertBellGrayPlus genToolTip oneliner"" data-tooltip=""Create Alert"" data-tooltip-alt=""Alert is active"" data-reg_ep=""add alert""></span></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Digerati Technologies Inc"" _p_pid=""1123598"" _r_pid=""1123598"">
                        <span class=""earnCalCompanyName middle"">Digerati Tech</span>&nbsp;(<a href=""/equities/digerati-tech-earnings"" class=""bold middle"" target=""_blank"">DTGI</a>)
                    </td>
                    <td class="" pid-1123598-2023-10-31-072023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-1123598-2023-10-31-072023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">6.72M</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""1123598""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Imaging Canada Liquidating Corp"" _p_pid=""31141"" _r_pid=""31141"">
                        <span class=""earnCalCompanyName middle"">Imaging Canada Liquidating</span>&nbsp;(<a href=""/equities/imris-inc-earnings"" class=""bold middle"" target=""_blank"">IMRSQ</a>)
                    </td>
                    <td class="" pid-31141-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-31141-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">312</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""31141""></td>
                </tr>
                            <tr>
                    <td class=""flag"">
                        <span title=""United States"" class=""ceFlags USA middle""></span>
                    </td>
                    <td class=""left noWrap earnCalCompany"" title=""Lombard Medical Inc"" _p_pid=""101912"" _r_pid=""101912"">
                        <span class=""earnCalCompanyName middle"">Lombard Medical</span>&nbsp;(<a href=""/equities/lombard-med-earnings"" class=""bold middle"" target=""_blank"">EVARF</a>)
                    </td>
                    <td class="" pid-101912-2023-10-31-092023-eps_actual"">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class="" pid-101912-2023-10-31-092023-rev_actual "">--</td>
                    <td class=""leftStrong"">/&nbsp;&nbsp;--</td>
                    <td class=""right"">27</td>
                    <td class=""right time"" data-value=""2""><span class="" genToolTip oneliner reverseToolTip""></span></td>
                    <!-- EARNING -->
                    					<td data-pair-id=""101912""></td>
                </tr>
            
</tbody>
</table>";

        #endregion
    }
}
