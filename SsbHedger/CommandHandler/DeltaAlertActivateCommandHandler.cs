using SsbHedger.Model;
using System;
using System.IO;
using System.Media;
using System.Security.RightsManagement;
using System.Threading;
using System.Threading.Tasks;

namespace SsbHedger.CommandHandler
{
    public class DeltaAlertActivateCommandHandler : IDeltaAlertActivateCommandHandler
    {
        private IIbHost _ibHost = null!;
        private MainWindowViewModel _mainWindowViewModel = null!;
        Thread _alertThread = null!;

        public DeltaAlertActivateCommandHandler(IIbHost ibHost)
        {
            _ibHost = ibHost;
        }

        public void Handle(MainWindowViewModel mainWindowViewModel, object[] parameters)
        {
            bool activate = (bool)parameters[0];
            double putStike = Convert.ToDouble(parameters[1]);
            double callStike = Convert.ToDouble(parameters[2]);

            _mainWindowViewModel = mainWindowViewModel;
            _mainWindowViewModel.DeltaAlertActive = activate;

            if (activate)
            {
                _ibHost.ReqMktDataNextPutOption(putStike);
                _ibHost.ReqMktDataNextCallOption(callStike);
            }
            else
            {
                _ibHost.CancelMktDataNextPutOption();
                _ibHost.CancelMktDataNextCalllOption();
            }

            if (Math.Abs(_mainWindowViewModel.NextPutDelta) <= _mainWindowViewModel.DeltaThreshold
                || Math.Abs(_mainWindowViewModel.NextCallDelta) <= _mainWindowViewModel.DeltaThreshold)
                if (_alertThread == null)
                {
                    {
                        _alertThread = new Thread(new ThreadStart(AlertFunction));
                        _alertThread.Start();
                    }
                }
        }

        private void AlertFunction()
        {
            while (_mainWindowViewModel.DeltaAlertActive)
            {
                SoundPlayer player = new SoundPlayer(Properties.Resources.DeltaAlert);
                player.Play();
                Thread.Sleep(5000);
            }
        }
    }
}
