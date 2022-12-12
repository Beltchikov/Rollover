using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SsbHedger;
using System.Windows;
using System.Runtime.Serialization;
using SsbHedger.Builders;

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
            registryManagerMock.Received().ReadConfiguration(
                Arg.Any<string>(),
                Arg.Any<int>(),
                Arg.Any<int>());

            //throw new NotImplementedException();
        }

        private void CallMethod(App sut, string methodName, object[] parameters)
        {
            var methodInfo = sut.GetType().GetMethod(methodName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            methodInfo?.Invoke(sut, parameters);
        }

        private void SetFiledValue(App sut, string fieldName, object value)
        {
            var fieldInfo = sut.GetType().GetField(
                fieldName,
               System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            fieldInfo?.SetValue(sut, value);
        }
    }
}
