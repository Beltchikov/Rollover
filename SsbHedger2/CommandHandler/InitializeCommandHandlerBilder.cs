using Microsoft.Extensions.DependencyInjection;
using SsbHedger2.Model;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace SsbHedger2.CommandHandler
{
    internal class InitializeCommandHandlerBilder : IInitializeCommandHandlerBilder
    {
        [ExcludeFromCodeCoverage]
        public InitializeCommandHandler Build(MainWindowViewModel mainWindowViewModel)
        {
            IRegistryManager? registryManager = ((App)Application.Current).Services.GetService<IRegistryManager>();
            if(registryManager == null)
            {
                throw new ApplicationException("Unexpected! registryManager is null");
            }

            IIbHost? ibHost = ((App)Application.Current).Services.GetService<IIbHost>();
            if (ibHost == null)
            {
                throw new ApplicationException("Unexpected! ibHost is null");
            }

            ibHost.ViewModel = mainWindowViewModel;

            return new InitializeCommandHandler(registryManager, ibHost);
        }
    }
}
