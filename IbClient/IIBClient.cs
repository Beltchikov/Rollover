/* Copyright (C) 2019 Interactive Brokers LLC. All rights reserved. This code is subject to the terms
 * and conditions of the IB API Non-Commercial License or the IB API Commercial License, as applicable. */
using IBApi;
using IbClient.messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IbClient
{
    public interface IIBClient
    {
        int ClientId { get; set; }
        int NextOrderId { get; set; }
        EClientSocket ClientSocket { get; }
        void ConnectAndStartReaderThread(string host, int port, int clientId);
        Task<Contract> ResolveContractAsync(int conId, string refExch);
        Task<Contract[]> ResolveContractAsync(string secType, string symbol, string currency, string exchange);

        event Action<AccountDownloadEndMessage> AccountDownloadEnd;
        event Action<AccountSummaryMessage> AccountSummary;
        event Action<AccountSummaryEndMessage> AccountSummaryEnd;
        event Action<AccountUpdateMultiMessage> AccountUpdateMulti;
        event Action<int> AccountUpdateMultiEnd;
        event Action<BondContractDetailsMessage> BondContractDetails;
        event Action<CommissionReport> CommissionReport;
        event Action<CompletedOrderMessage> CompletedOrder;
        event Action CompletedOrdersEnd;
        event Action ConnectionClosed;
        event Action<ContractDetailsMessage> ContractDetails;
        event Action<int> ContractDetailsEnd;
        event Action<long> CurrentTime;
        event Action<int, DeltaNeutralContract> DeltaNeutralValidation;
        event Action<int, string> DisplayGroupList;
        event Action<int, string> DisplayGroupUpdated;
        event Action<int, int, string, Exception> Error;
        event Action<ExecutionMessage> ExecDetails;
        event Action<int> ExecDetailsEnd;
        event Action<FamilyCode[]> FamilyCodes;
        event Action<FundamentalsMessage> FundamentalData;
        event Action<HeadTimestampMessage> HeadTimestamp;
        event Action<HistogramDataMessage> HistogramData;
        event Action<HistoricalDataMessage> HistoricalData;
        event Action<HistoricalDataEndMessage> HistoricalDataEnd;
        event Action<HistoricalDataMessage> HistoricalDataUpdate;
        event Action<HistoricalNewsMessage> HistoricalNews;
        event Action<HistoricalNewsEndMessage> HistoricalNewsEnd;
        event Action<HistoricalTickMessage> historicalTick;
        event Action<HistoricalTickBidAskMessage> historicalTickBidAsk;
        event Action<HistoricalTickLastMessage> historicalTickLast;
        event Action<ManagedAccountsMessage> ManagedAccounts;
        event Action<MarketDataTypeMessage> MarketDataType;
        event Action<MarketRuleMessage> MarketRule;
        event Action<DepthMktDataDescription[]> MktDepthExchanges;
        event Action<NewsArticleMessage> NewsArticle;
        event Action<NewsProvider[]> NewsProviders;
        event Action<ConnectionStatusMessage> NextValidId;
        event Action<OpenOrderMessage> OpenOrder;
        event Action OpenOrderEnd;
        event Action<OrderBoundMessage> OrderBound;
        event Action<OrderStatusMessage> OrderStatus;
        event Action<PnLMessage> pnl;
        event Action<PnLSingleMessage> pnlSingle;
        event Action<PositionMessage> Position;
        event Action PositionEnd;
        event Action<PositionMultiMessage> PositionMulti;
        event Action<int> PositionMultiEnd;
        event Action<RealTimeBarMessage> RealtimeBar;
        event Action<AdvisorDataMessage> ReceiveFA;
        event Action<int, int, string> RerouteMktDataReq;
        event Action<int, int, string> RerouteMktDepthReq;
        event Action<ScannerMessage> ScannerData;
        event Action<int> ScannerDataEnd;
        event Action<string> ScannerParameters;
        event Action<SecurityDefinitionOptionParameterMessage> SecurityDefinitionOptionParameter;
        event Action<int> SecurityDefinitionOptionParameterEnd;
        event Action<int, Dictionary<int, KeyValuePair<string, char>>> SmartComponents;
        event Action<SoftDollarTiersMessage> SoftDollarTiers;
        event Action<SymbolSamplesMessage> SymbolSamples;
        event Action<TickByTickAllLastMessage> tickByTickAllLast;
        event Action<TickByTickBidAskMessage> tickByTickBidAsk;
        event Action<TickByTickMidPointMessage> tickByTickMidPoint;
        event Action<int, int, double, string, double, int, string, double, double> TickEFP;
        event Action<int, int, double> TickGeneric;
        event Action<TickNewsMessage> TickNews;
        event Action<TickOptionMessage> TickOptionCommunication;
        event Action<TickPriceMessage> TickPrice;
        event Action<TickReqParamsMessage> TickReqParams;
        event Action<TickSizeMessage> TickSize;
        event Action<int> TickSnapshotEnd;
        event Action<int, int, string> TickString;
        event Action<UpdateAccountTimeMessage> UpdateAccountTime;
        event Action<AccountValueMessage> UpdateAccountValue;
        event Action<DeepBookMessage> UpdateMktDepth;
        event Action<DeepBookMessage> UpdateMktDepthL2;
        event Action<int, int, string, string> UpdateNewsBulletin;
        event Action<UpdatePortfolioMessage> UpdatePortfolio;
        event Action<bool, string> VerifyAndAuthCompleted;
        event Action<string, string> VerifyAndAuthMessageAPI;
        event Action<bool, string> VerifyCompleted;
        event Action<string> VerifyMessageAPI;
    }
}