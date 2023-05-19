using SsbHedger.Model;
using System;
using System.Runtime.CompilerServices;

namespace SsbHedger.CommandHandler
{
    public class UpdateReqMktDataAtmStrikeCallCommandHandler : IUpdateReqMktDataAtmStrikeCallCommandHandler
    {
        private IIbHost _ibHost = null!;
        double _lastCallStrike = 0;

        public UpdateReqMktDataAtmStrikeCallCommandHandler(IIbHost ibHost)
        {
            _ibHost = ibHost;
        }

        public void Handle(MainWindowViewModel viewModel, object[] parameters)
        {
            double callStike = Convert.ToDouble(parameters[0]);
            if (_lastCallStrike == callStike)
            {
                return;
            }

            _ibHost.CancelMktPutOptionIV();
            _ibHost.ReqMktDataPutOptionIV(callStike);
            _lastCallStrike = callStike;
        }
    }
}
