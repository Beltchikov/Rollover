using SsbHedger.Model;
using System;

namespace SsbHedger.CommandHandler
{
    public class VolatilityAlertActivateCommandHandler : IVolatilityAlertActivateCommandHandler
    {
        private IIbHost _ibHost = null!;
        private MainWindowViewModel _mainWindowViewModel = null!;

        public VolatilityAlertActivateCommandHandler(IIbHost ibHost)
        {
            _ibHost = ibHost;
        }

        public void Handle(MainWindowViewModel mainWindowViewModel, object[] parameters)
        {
            bool activate = (bool)parameters[0];
           _mainWindowViewModel = mainWindowViewModel;
           _mainWindowViewModel.DeltaAlertActive = activate;

            if (activate)
            {
               // TODO
            }
            else
            {
                // TODO
            }
        }
    }
}
