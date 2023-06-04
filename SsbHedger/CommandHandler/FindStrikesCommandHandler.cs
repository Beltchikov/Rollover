using SsbHedger.Model;
using SsbHedger.SsbConfiguration;
using SsbHedger.Utilities;
using System;
using System.Collections.Generic;

namespace SsbHedger.CommandHandler
{
    public class FindStrikesCommandHandler : IFindStrikesCommandHandler
    {
        private IIbHost _ibHost = null!;
        private IAtmStrikeUtility _atmStrikeUtility;
        private IConfiguration _configuration = null!;

        public FindStrikesCommandHandler(IIbHost ibHost, IAtmStrikeUtility atmStrikeUtility, IConfiguration configuration)
        {
            _ibHost = ibHost;
            _atmStrikeUtility = atmStrikeUtility;
            _configuration = configuration;
        }

        public void Handle(MainWindowViewModel mainWindowViewModel, object[] parameters)
        {
            //if (_ibHost == null)
            //{ throw new ApplicationException("Unexpected! _ibHost is null"); }
            //var viewModel = _ibHost.ViewModel;
            //if (viewModel == null)
            //{ throw new ApplicationException("Unexpected! viewModel is null"); }


            //viewModel.Strikes = _ibHost.GetStrikes(underlying, lastTradeDateOrContractMonth, numberOfStrikes);



            // OLD IMPLEMENTATION
            var underlyingPrice = (double)parameters[0];
            var viewModel = _ibHost.ViewModel;

            if (_ibHost == null)
            { throw new ApplicationException("Unexpected! _ibHost is null"); }
            if (viewModel == null)
            { throw new ApplicationException("Unexpected! viewModel is null"); }

            if (viewModel.AtmStrikeCall <= underlyingPrice && underlyingPrice <= viewModel.AtmStrikePut)
            {
                return;
            }

            var strikesList = new List<double>();
            viewModel.AtmStrikePut = (int)Math.Ceiling(underlyingPrice);    // In option table up is down and down is up
            viewModel.AtmStrikeCall = (int)Math.Floor(underlyingPrice);// In option table up is down and down is up
            for (int i = 0; i < (int)Math.Ceiling((double)MainWindowViewModel.STRIKES_COUNT / 2); i++)
            {
                var nextStrikeUp = viewModel.AtmStrikePut + i;
                strikesList.Add(nextStrikeUp);
                var nextStrikeDown = viewModel.AtmStrikeCall - i;
                strikesList.Add(nextStrikeDown);
            }

            strikesList.Sort();
            foreach (var strike in strikesList)
            {
                viewModel.Strikes.Add(strike);
            }



            // TODO logic for 0.5 strikes

            //_ibHost.ViewModel.NextAtmStrike = -1;
            //_ibHost.ViewModel.SecondAtmStrike = -1;
            //var atmStrikesCandidates = _atmStrikeUtility.AtmStrikeCandidates(underlyingPrice, MainWindowViewModel.STRIKES_STEP);

            ////_ibHost.AtmStrikesCandidate = new AtmStrikes(Math.Round(underlyingPrice, 0), Math.Round(underlyingPrice, 0));
            ////_ibHost.ReqCheckNextOptionsStrike(Math.Round(underlyingPrice,0));

            //// Duplicate ticket exception
            //_ibHost.AtmStrikesCandidate = atmStrikesCandidates[0];
            //_ibHost.ReqCheckNextOptionsStrike(Math.Round(atmStrikesCandidates[0].NextAtmStrike, 0));

            ////_atmStrikeUtility.SetAtmStrikesInViewModel(_ibHost, underlyingPrice);
        }
    }
}
