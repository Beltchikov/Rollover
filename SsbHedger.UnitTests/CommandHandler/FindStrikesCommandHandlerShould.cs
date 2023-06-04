using NSubstitute;
using SsbHedger.CommandHandler;
using SsbHedger.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace SsbHedger.UnitTests.CommandHandler
{
    public class FindStrikesCommandHandlerShould
    {

        [Theory]
        [AutoNSubstituteData]
        public void CallGetStrikes(
            MainWindowViewModel viewModel,
            object[] parameters,
            IIbHost ibHost,
            IIbHost configuration,
            FindStrikesCommandHandler sut)
        {
            sut.Handle(viewModel, parameters);
            ibHost.Received().GetStrikes(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>());
        }
    }
}
