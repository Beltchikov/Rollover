using SsbHedger.Model;
using System;

namespace SsbHedger.CommandHandler
{
    public class UpdateReqMktDataAtmStrikePutCommandHandler : IUpdateReqMktDataAtmStrikeUpCommandHandler
    {
        private IIbHost _ibHost = null!;
        bool _requestSent;

        public UpdateReqMktDataAtmStrikePutCommandHandler(IIbHost ibHost)
        {
            _ibHost = ibHost;
        }

        public void Handle(MainWindowViewModel viewModel, object[] parameters)
        {
            double callStike = Convert.ToDouble(parameters[0]);
            if (_requestSent)
            {
                _ibHost.CancelMktPutOptionIV();
            }

            _ibHost.ReqMktDataPutOptionIV(callStike);
            _requestSent = true;
        }
    }
}
