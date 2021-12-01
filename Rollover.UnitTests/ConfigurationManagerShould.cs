using AutoFixture.Xunit2;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rollover.UnitTests
{
    public class ConfigurationManagerShould
    {
        [Theory, AutoNSubstituteData]
        public void ReadConfigurationFile(
            [Frozen] IFileHelper fileHelper,
            ConfigurationManager sut)
        {
            sut.GetConfiguration();
            fileHelper.Received().ReadAllText(ConfigurationManager.CONFIGURATION_FILE);
        }

        [Theory, AutoNSubstituteData]
        public void CallSerializer(
            [Frozen] ISerializer serializer,
            ConfigurationManager sut)
        {
            sut.GetConfiguration();
            serializer.Received().Deserialize<Configuration>(Arg.Any<string>());
        }
    }
}
