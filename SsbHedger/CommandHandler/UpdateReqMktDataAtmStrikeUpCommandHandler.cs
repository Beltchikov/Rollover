using SsbHedger.Model;
using System;

namespace SsbHedger.CommandHandler
{
    public class UpdateReqMktDataAtmStrikeUpCommandHandler : IUpdateReqMktDataAtmStrikeUpCommandHandler
    {
        private IIbHost _ibHost = null!;
        bool _requestSent;

        public UpdateReqMktDataAtmStrikeUpCommandHandler(IIbHost ibHost)
        {
            _ibHost = ibHost;
        }

        public void Handle(MainWindowViewModel viewModel, object[] parameters)
        {
            double callStike = Convert.ToDouble(parameters[0]);
            if(_requestSent)
            {
                _ibHost.CancelMktDataNextCalllOption();
            }

            _ibHost.ReqMktDataNextCallOption(callStike);
            _requestSent = true;
        }
    }
}
