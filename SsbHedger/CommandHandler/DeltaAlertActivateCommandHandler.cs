using SsbHedger.Model;
using System;
using System.Security.RightsManagement;
using System.Threading.Tasks;

namespace SsbHedger.CommandHandler
{
    public class DeltaAlertActivateCommandHandler : IDeltaAlertActivateCommandHandler
    {
        private IIbHost _ibHost = null!;
        public DeltaAlertActivateCommandHandler(IIbHost ibHost)
        {
            _ibHost = ibHost;
        }

        public void Handle(MainWindowViewModel mainWindowViewModel, object[] parameters)
        {
            bool activate = (bool)parameters[0];
            double putStike = Convert.ToDouble(parameters[1]);
            double callStike = Convert.ToDouble(parameters[2]);
            
            mainWindowViewModel.DeltaAlertActive = activate;

            if (activate)
            {
                _ibHost.ReqMktDataNextPutOption(putStike);
                _ibHost.ReqMktDataNextCallOption(callStike);
            }
            else
            {
                // TODO
                
                // https://interactivebrokers.github.io/tws-api/md_cancel.html
                //_ibHost.CancelMktDataNextPutOption(putStike);
                //_ibHost.CancelMktDataNextCalllOption(putStike);
            }
        }
    }
}
