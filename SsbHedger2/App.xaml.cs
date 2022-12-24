using Microsoft.Extensions.DependencyInjection;
using SsbHedger2.Abstractions;
using System;
using System.Windows;

namespace SsbHedger2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Services = new ServiceCollection()
                .AddSingleton<IRegistryCurrentUserAbstraction, RegistryCurrentUserAbstraction>()
                .AddSingleton<IIbHost, IbHost>()
                .AddSingleton<IRegistryManager, RegistryManager>()
                .BuildServiceProvider();
        }
        public IServiceProvider Services { get; }
    }
}
