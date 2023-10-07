﻿using IBApi;
using System.Threading.Tasks;

namespace StockAnalyzer.Ib
{
    public interface IIbHost
    {
        public IIbConsumer? Consumer { get; set; }
        public Task<bool> ConnectAndStartReaderThread(string host, int port, int clientId, int timeout);
        public void Disconnect();
        public Task<ContractDetails?> RequestContractDetailsAsync(string ticker, int timeout);

        public Task<string> RequestFundamentalDataAsync(string ticker, string reportType, int timeout);

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