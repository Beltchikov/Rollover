using SsbHedger.Model;
using System;

namespace SsbHedger.CommandHandler
{
    public class UpdateReqMktDataAtmStrikeCallCommandHandler : IUpdateReqMktDataAtmStrikeCallCommandHandler
    {
        private IIbHost _ibHost = null!;
        bool _requestSent;

        public UpdateReqMktDataAtmStrikeCallCommandHandler(IIbHost ibHost)
        {
            _ibHost = ibHost;
        }

        public void Handle(MainWindowViewModel viewModel, object[] parameters)
        {
            double putStike = Convert.ToDouble(parameters[0]);
            if (_requestSent)
            {
                _ibHost.CancelMktCallOptionIV();
            }

            _ibHost.ReqMktDataCallOptionIV(putStike);
            _requestSent = true;
        }
    }
}
