using Microsoft.Extensions.DependencyInjection;
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
                //.AddSingleton<IIbHost, IbHost>()
                .BuildServiceProvider();
        }
        public IServiceProvider Services { get; }
    }
}
