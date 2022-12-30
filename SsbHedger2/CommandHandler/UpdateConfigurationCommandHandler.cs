using System;
using SsbHedger2.Model;

namespace SsbHedger2.CommandHandler
{
    public class UpdateConfigurationCommandHandler
    {
        private IRegistryManager _registryManager = null!;
        MainWindowViewModel _viewModel = null!;

        public UpdateConfigurationCommandHandler(
            IRegistryManager registryManager,
            MainWindowViewModel viewModel)
        {
            _registryManager = registryManager;
            _viewModel = viewModel;
        }

        public void Handle(object[] parameters)
        {
            throw new NotImplementedException();

            //viewModel.Host = _configurationWindow.txtHost.Text;
            //viewModel.Port = Convert.ToInt32(_configurationWindow.txtPort.Text);
            //viewModel.ClientId = Convert.ToInt32(_configurationWindow.txtClientId.Text);
        }
    }
}