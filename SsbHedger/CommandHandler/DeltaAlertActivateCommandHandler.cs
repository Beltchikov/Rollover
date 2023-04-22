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
            double bearStike = Convert.ToDouble(parameters[1]);
            double boolStike = Convert.ToDouble(parameters[2]);
            
            mainWindowViewModel.DeltaAlertActive = activate;

            //if (activate) 
            //{ 
            //   _ibHost.ReqMktData(_ibHost.BEAR_NEXT_INNER_OPTION_REQ_ID, right, strike)
            //}
        }
    }
}
