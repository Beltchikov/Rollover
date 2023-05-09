using SsbHedger.Model;
using SsbHedger.Utilities;
using System;

namespace SsbHedger.CommandHandler
{
    public class FindStrikesCommandHandler : IFindStrikesCommandHandler
    {
        private IIbHost _ibHost = null!;
        private IAtmStrikeUtility _atmStrikeUtility;

        public FindStrikesCommandHandler(IIbHost ibHost, IAtmStrikeUtility atmStrikeUtility)
        {
            _ibHost = ibHost;
            _atmStrikeUtility = atmStrikeUtility;
        }

        public void Handle(MainWindowViewModel mainWindowViewModel, object[] parameters)
        {
            var underlyingPrice = (double)parameters[0];
            var viewModel = _ibHost.ViewModel;

            if(_ibHost == null) 
            { throw new ApplicationException("Unexpected! _ibHost is null"); }
            if (_ibHost.ViewModel == null)
            { throw new ApplicationException("Unexpected! _ibHost.ViewModel is null"); }

            _ibHost.ViewModel.NextAtmStrike = -1;
            _ibHost.ViewModel.SecondAtmStrike = -1;
            var atmStrikesCandidates = _atmStrikeUtility.AtmStrikeCandidates(underlyingPrice, MainWindowViewModel.STRIKES_STEP);

            //_ibHost.AtmStrikesCandidate = new AtmStrikes(Math.Round(underlyingPrice, 0), Math.Round(underlyingPrice, 0));
            //_ibHost.ReqCheckNextOptionsStrike(Math.Round(underlyingPrice,0));

            // Duplicate ticket exception
            _ibHost.AtmStrikesCandidate = atmStrikesCandidates[0];
            _ibHost.ReqCheckNextOptionsStrike(Math.Round(atmStrikesCandidates[0].NextAtmStrike, 0));

            //_atmStrikeUtility.SetAtmStrikesInViewModel(_ibHost, underlyingPrice);
        }
    }
}
