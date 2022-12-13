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
            SetFiledValue(sut, "_registryManager", registryManagerMock);
            SetFiledValue(sut, "_mainWindowBuilder", mainWindowBuilderMock);

            // Act
            var startupEventArgs = (StartupEventArgs)FormatterServices.GetUninitializedObject(typeof(StartupEventArgs));
            var parameters = new object[] { new object(), startupEventArgs };
            CallMethod(sut, "Application_Startup", parameters);

            // Verify
            string? defaultHost = GetFiledValue<string>(sut, "_defaultHost");
            int? defaultPort= GetFiledValue<int>(sut, "_defaultPort");
            int? defaultClientId = GetFiledValue<int>(sut, "_defaultClientId");
            if (defaultHost != null && defaultPort != null && defaultClientId != null)
            {
                registryManagerMock.Received().ReadConfiguration(
                    defaultHost,
                    defaultPort.Value,
                    defaultClientId.Value);
            }
        }

        private static void CallMethod(App sut, string methodName, object[] parameters)
        {
            var methodInfo = sut.GetType().GetMethod(methodName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            methodInfo?.Invoke(sut, parameters);
        }

        private static void SetFiledValue(App sut, string fieldName, object value)
        {
            var fieldInfo = sut.GetType().GetField(
                fieldName,
               System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            fieldInfo?.SetValue(sut, value);
        }

        private static T? GetFiledValue<T>(App sut, string fieldName) //where T: struct
        {
            var fieldInfo = sut.GetType().GetField(
                fieldName,
               System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (T?)fieldInfo?.GetValue(sut);
        }
    }
}
