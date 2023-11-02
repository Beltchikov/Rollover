using IBApi;
using System.Threading.Tasks;

namespace IbClient.IbHost
{
    public interface IIbHost
    {
        IIbConsumer Consumer { get; set; }
        Task<bool> ConnectAndStartReaderThread(string host, int port, int clientId, int timeout);
        void Disconnect();
        Task<ContractDetails> RequestContractDetailsAsync(
            Contract contract,
            int timeout);
        Task<string> RequestFundamentalDataAsync(
                    Contract contract,
                    string reportType,
                    int timeout);
        Task<double?> RequestMarketDataLiveAsync(
                    Contract contract,
                    int tickType,
                    bool snapshot,
                    int timeout);

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