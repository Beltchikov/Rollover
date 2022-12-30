using System.Diagnostics.CodeAnalysis;
using System;
using SsbHedger2.Model;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace SsbHedger2.CommandHandler
{
    internal class UpdateConfigurationCommandHandlerBuilder
    {
        [ExcludeFromCodeCoverage]
        internal UpdateConfigurationCommandHandler Build(MainWindowViewModel mainWindowViewModel)
        {
            IRegistryManager? registryManager = ((App)Application.Current).Services.GetService<IRegistryManager>();
            if (registryManager == null)
            {
                throw new ApplicationException("Unexpected! registryManager is null");
            }

            return new UpdateConfigurationCommandHandler(registryManager);
        }
    }
}