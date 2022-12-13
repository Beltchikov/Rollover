using NSubstitute;
using SsbHedger.Builders;
using System.Runtime.Serialization;
using System.Windows;

namespace SsbHedger.UnitTests
{
    public class AppShould
    {
        [StaFact]
        public void CallRegistryManagerReadConfiguration()
        {
            // Arrange
            IRegistryManager registryManagerMock = Substitute.For<IRegistryManager>();
            IMainWindowBuilder mainWindowBuilderMock = Substitute.For<IMainWindowBuilder>();

            var sut = new App();
            Reflection.SetFiledValue(sut, "_registryManager", registryManagerMock);
            Reflection.SetFiledValue(sut, "_mainWindowBuilder", mainWindowBuilderMock);

            // Act
            var startupEventArgs = (StartupEventArgs)FormatterServices.GetUninitializedObject(typeof(StartupEventArgs));
            var parameters = new object[] { new object(), startupEventArgs };
            Reflection.CallMethod(sut, "Application_Startup", parameters);

            // Verify
            string? defaultHost = Reflection.GetFiledValue<string>(sut, "_defaultHost");
            int? defaultPort= Reflection.GetFiledValue<int>(sut, "_defaultPort");
            int? defaultClientId = Reflection.GetFiledValue<int>(sut, "_defaultClientId");
            if (defaultHost != null && defaultPort != null && defaultClientId != null)
            {
                registryManagerMock.Received().ReadConfiguration(
                    defaultHost,
                    defaultPort.Value,
                    defaultClientId.Value);
            }
        }
    }
}
