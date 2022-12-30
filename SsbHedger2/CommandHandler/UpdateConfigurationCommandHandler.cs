using System;

namespace SsbHedger2.CommandHandler
{
    public class UpdateConfigurationCommandHandler
    {
        private IRegistryManager registryManager = null!;

        public UpdateConfigurationCommandHandler(IRegistryManager registryManager)
        {
            this.registryManager = registryManager;
        }

        internal void Handle()
        {
            throw new NotImplementedException();

            //viewModel.Host = _configurationWindow.txtHost.Text;
            //viewModel.Port = Convert.ToInt32(_configurationWindow.txtPort.Text);
            //viewModel.ClientId = Convert.ToInt32(_configurationWindow.txtClientId.Text);
        }
    }
}