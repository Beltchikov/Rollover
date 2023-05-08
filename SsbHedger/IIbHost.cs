using SsbHedger.Model;
using SsbHedger.Utilities;
using System.Threading.Tasks;

namespace SsbHedger
{
    public interface IIbHost
    {
        public MainWindowViewModel? ViewModel { get; set; }
        AtmStrikes AtmStrikesCandidate { get; set; }
        public int Timeout { get; }
        public Task<bool> ConnectAndStartReaderThread();
        public void Disconnect();
        public void ReqHistoricalData();
        void ApplyDefaultHistoricalData();
        void ReqPositions();
        void ReqMktDataNextPutOption(double putStike);
        void ReqMktDataNextCallOption(double callStike);
        void CancelMktDataNextPutOption();
        void CancelMktDataNextCalllOption();
        void ReqMktUnderlying();
        void CancelMktUnderlying();
        void ReqCheckNextOptionsStrike(double nextAtmStrike);
        void ReqCheckSecondOptionsStrike(double secondAtmStrike);
    }
}