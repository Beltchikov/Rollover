using IBApi;
using IbClient.messages;
using IbClient.Types;
using System;
using System.Threading.Tasks;
using TickType = IbClient.Types.TickType;

namespace IbClient.IbHost
{
    public interface IIbHost
    {
        IIbConsumer Consumer { get; set; }
        Task<bool> ConnectAndStartReaderThread(string host, int port, int clientId, int timeout);
        void Disconnect();
        Task<double?> RateOfExchange(string currency, int timeout);
        Task<ContractDetails> RequestContractDetailsAsync(
            Contract contract,
            int timeout);
        Task<string> RequestFundamentalDataAsync(
                    Contract contract,
                    string reportType,
                    int timeout);
        Task<(double?, TickType?, MarketDataType?)> RequestMarketDataSnapshotAsync(Contract contract, MarketDataType[] marketDataTypes, int timeout);
        Task<double?> RequestMarketDataSnapshotAsync(Contract contract, TickType tickType, int timeout);
        Task RequestMarketDataSnapshotAsync(
            Contract contract,
            MarketDataType marketDataType,
            Action<TickPriceMessage> onTickPriceMessage,
            Action<TickSizeMessage> onTickSizeMessage,
            Action onTickSnapshotEnd);
        Task<OrderStateOrError> WhatIfOrderStateFromContract(Contract contract, int qty, int timeout);

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