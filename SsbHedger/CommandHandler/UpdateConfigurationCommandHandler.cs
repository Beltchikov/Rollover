using SsbHedger.Model;
using SsbHedger.SsbConfiguration;
using System;

namespace SsbHedger.CommandHandler
{
    public class UpdateConfigurationCommandHandler : IUpdateConfigurationCommandHandler
    {
        private IRegistryManager _registryManager = null!;
        private IConfiguration _configuration = null!;
        private IIbHost _ibHost = null!;

        public UpdateConfigurationCommandHandler(
            IRegistryManager registryManager,
            IConfiguration configuration,
            IIbHost ibHost)
        {
            _registryManager = registryManager;
            _configuration = configuration;
            _ibHost = ibHost;
        }

        public void Handle(MainWindowViewModel viewModel, object[] parameters)
        {
            if (parameters == null)
            {
                throw new ApplicationException("Unexpected! data is null");
            }
            
            string? host = parameters[0]?.ToString();
            if (host == null)
            {
                throw new ApplicationException("Unexpected! host is null");
            }
            
            int port = Convert.ToInt32(parameters[1]);
            int clientId = Convert.ToInt32(parameters[2]);
            
            string? underlyingSymbol = parameters[3]?.ToString();
            if (underlyingSymbol == null)
            {
                throw new ApplicationException("Unexpected! underlyingSymbol is null");
            }

            string? sessionStart = parameters[4]?.ToString();
            if (sessionStart == null)
            {
                throw new ApplicationException("Unexpected! sessionStart is null");
            }

            string? sessionEnd= parameters[5]?.ToString();
            if (sessionEnd == null)
            {
                throw new ApplicationException("Unexpected! sessionEnd is null");
            }


            _registryManager.WriteConfiguration(new ConfigurationData(
                host,
                port,
                clientId,
                underlyingSymbol,
                sessionStart,
                sessionEnd));

            _configuration.SetValue(Configuration.HOST, host);
            _configuration.SetValue(Configuration.PORT, port);
            _configuration.SetValue(Configuration.CLIENT_ID, clientId);
            _configuration.SetValue(Configuration.UNDERLYING_SYMBOL, underlyingSymbol);
            _configuration.SetValue(Configuration.SESSION_START, sessionStart);
            _configuration.SetValue(Configuration.SESSION_END, sessionEnd);

            _ibHost.Disconnect();
            _ibHost.ConnectAndStartReaderThread();
        }
    }
}