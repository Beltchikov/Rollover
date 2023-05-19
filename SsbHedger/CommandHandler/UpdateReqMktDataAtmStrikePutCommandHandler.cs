using SsbHedger.Model;
using System;

namespace SsbHedger.CommandHandler
{
    public class UpdateReqMktDataAtmStrikePutCommandHandler : IUpdateReqMktDataAtmStrikePutCommandHandler
    {
        private IIbHost _ibHost = null!;
        double _lastPutStrike = 0;

        public UpdateReqMktDataAtmStrikePutCommandHandler(IIbHost ibHost)
        {
            _ibHost = ibHost;
        }

        public void Handle(MainWindowViewModel viewModel, object[] parameters)
        {
            double putStike = Convert.ToDouble(parameters[0]);
            if (_lastPutStrike == putStike)
            {
                return;
            }

            _ibHost.CancelMktCallOptionIV();
            _ibHost.ReqMktDataCallOptionIV(putStike);
            _lastPutStrike = putStike;
        }
    }
}
