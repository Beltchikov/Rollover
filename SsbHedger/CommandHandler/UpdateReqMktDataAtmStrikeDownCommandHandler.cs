using SsbHedger.Model;
using System;

namespace SsbHedger.CommandHandler
{
    public class UpdateReqMktDataAtmStrikeDownCommandHandler : IUpdateReqMktDataAtmStrikeDownCommandHandler
    {
        private IIbHost _ibHost = null!;
        bool _requestSent;

        public UpdateReqMktDataAtmStrikeDownCommandHandler(IIbHost ibHost)
        {
            _ibHost = ibHost;
        }

        public void Handle(MainWindowViewModel viewModel, object[] parameters)
        {
            double putStike = Convert.ToDouble(parameters[0]);
            if (_requestSent)
            {
                _ibHost.CancelMktDataNextPutOption();
            }

            _ibHost.ReqMktDataNextPutOption(putStike);
            _requestSent = true;
        }
    }
}
