using System.Diagnostics.CodeAnalysis;
using System;
using SsbHedger.Model;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SsbHedger.Configuration;

namespace SsbHedger.CommandHandler
{
    internal class UpdateConfigurationCommandHandlerBuilder
    {
        [ExcludeFromCodeCoverage]
        internal UpdateConfigurationCommandHandler Build(MainWindowViewModel viewModel)
        {
            IRegistryManager? registryManager = ((App)Application.Current).Services.GetService<IRegistryManager>();
            if (registryManager == null)
            {
                throw new ApplicationException("Unexpected! registryManager is null");
            }

            return new UpdateConfigurationCommandHandler(registryManager, viewModel);
        }
    }
}