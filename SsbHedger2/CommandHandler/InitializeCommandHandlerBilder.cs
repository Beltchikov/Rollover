﻿using Microsoft.Extensions.DependencyInjection;
using SsbHedger.RegistryManager;
using SsbHedger.Model;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace SsbHedger.CommandHandler
{
    internal class InitializeCommandHandlerBilder 
    {
        [ExcludeFromCodeCoverage]
        public InitializeCommandHandler Build(MainWindowViewModel mainWindowViewModel)
        {
            IRegistryManager? registryManager = ((App)Application.Current).Services.GetService<IRegistryManager>();
            if(registryManager == null)
            {
                throw new ApplicationException("Unexpected! registryManager is null");
            }

            IConfiguration? configuration = ((App)Application.Current).Services.GetService<IConfiguration>();
            if (configuration == null)
            {
                throw new ApplicationException("Unexpected! configuration is null");
            }

            IIbHost? ibHost = ((App)Application.Current).Services.GetService<IIbHost>();
            if (ibHost == null)
            {
                throw new ApplicationException("Unexpected! ibHost is null");
            }

            ibHost.ViewModel = mainWindowViewModel;

            return new InitializeCommandHandler(registryManager, configuration, ibHost);
        }
    }
}
