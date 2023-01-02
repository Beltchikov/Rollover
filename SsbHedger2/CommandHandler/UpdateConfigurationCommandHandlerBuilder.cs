using System.Diagnostics.CodeAnalysis;
using System;
using SsbHedger.Model;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SsbHedger.RegistryManager;

namespace SsbHedger.CommandHandler
{
    internal class UpdateConfigurationCommandHandlerBuilder
    {
        [ExcludeFromCodeCoverage]
        internal UpdateConfigurationCommandHandler Build()
        {
            IRegistryManager? registryManager = ((App)Application.Current).Services.GetService<IRegistryManager>();
            if (registryManager == null)
            {
                throw new ApplicationException("Unexpected! registryManager is null");
            }

            IConfiguration? configuration = ((App)Application.Current).Services.GetService<IConfiguration>();
            if (configuration == null)
            {
                throw new ApplicationException("Unexpected! configuration is null");
            }

            return new UpdateConfigurationCommandHandler(registryManager, configuration);
        }
    }
}