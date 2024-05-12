﻿using IBApi;
using IbClient.messages;
using IbClient.Types;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TickType = IbClient.Types.TickType;

namespace IbClient.IbHost
{
    public interface IIbHost
    {
        IIbConsumer Consumer { get; set; }
        Task<bool> ConnectAndStartReaderThread(string host, int port, int clientId, int timeout);
        Task ConnectAndStartReaderThread(
           string host,
           int port,
           int clientId,
           Action<ConnectionStatusMessage> connectionStatusCallback,
           Action<ManagedAccountsMessage> managedAccountsCallback,
           Action<ErrorMessage> errorMessageCallback);
        bool IsConnected { get; }
        void Disconnect();
        Task RequestAccountSummaryAsync(
            string group,
            string tags,
             Action<AccountSummaryMessage> accountSummaryCallback,
           Action<AccountSummaryEndMessage> accountSummaryEndCallback);
        Task<int> ReqIdsAsync(int idParam);
        Task<double?> RateOfExchange(string currency, int timeout);
        Task<ContractDetails> RequestContractDetailsAsync(
            Contract contract,
            int timeout);
        Task<string> RequestFundamentalDataAsync(
                    Contract contract,
                    string reportType,
                    int timeout);
        Task<(double?, TickType?, string)> RequestMktData(
            Contract contract,
            string genericTickList,
            bool snapshot,
            bool regulatorySnapshot,
            List<TagValue> mktDataOptions,
            int timeout);

        Task RequestHistoricalDataAsync(
            Contract contractBuy,
            string endDateTime,
            string durationString,
            string barSizeSetting,
            string whatToShow,
            int useRTH,
            int formatDate,
            bool keepUpToDate,
            List<TagValue> tagValues,
            int timeout,
            Action<HistoricalDataMessage> historicalDataCallback,
            Action<HistoricalDataMessage> historicalDataUpdateCallback,
            Action<HistoricalDataEndMessage> historicalDataEndCallback);
        Task<SymbolSamplesMessage> RequestMatchingSymbolsAsync(string symbol, int timeout);
        Task<OrderStateOrError> WhatIfOrderStateFromContract(Contract contract, int qty, int timeout);
        Task<OrderStateOrError> PlaceOrderAsync(Contract contract, Order order, int timeout);
        void ClearQueueOrderOpenMessage();

        //public void ReqHistoricalData();
        //void ApplyDefaultHistoricalData();
        //void ReqPositions();
        //void ReqMktDataNextPutOption(double putStike);
        //void ReqMktDataNextCallOption(double callStike);
        //void CancelMktDataNextPutOption();
        //void CancelMktDataNextCalllOption();
        //void ReqMktUnderlying();
        //void CancelMktUnderlying();
        //void ReqCheckNextOptionsStrike(double nextAtmStrike);
        //void ReqCheckSecondOptionsStrike(double secondAtmStrike);
        //void ReqMktDataCallOptionIV(double callStike);
        //void ReqMktDataPutOptionIV(double putStike);
        //void CancelMktCallOptionIV();
        //void CancelMktPutOptionIV();
        //IEnumerable<double> GetStrikes(
        //    string underlying,
        //    string lastTradeDateOrContractMonth,
        //    int numberOfStrikes);
        //IEnumerable<double> GetStrikesSpy(double underlyingPrice, string lastTradeDate, int numberOfStrikes, double strikeStep);
    }
}