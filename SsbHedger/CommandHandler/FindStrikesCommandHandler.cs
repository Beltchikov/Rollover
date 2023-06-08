﻿using SsbHedger.Model;
using SsbHedger.SsbConfiguration;
using SsbHedger.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SsbHedger.CommandHandler
{
    public class FindStrikesCommandHandler : IFindStrikesCommandHandler
    {
        private IIbHost _ibHost = null!;
        private IAtmStrikeUtility _atmStrikeUtility;
        private IConfiguration _configuration = null!;
        private ILastTradeDateConverter _lastTradeDateConverter;

        public FindStrikesCommandHandler(
            IIbHost ibHost,
            IAtmStrikeUtility atmStrikeUtility,
            IConfiguration configuration,
            ILastTradeDateConverter lastTradeDateConverter)
        {
            _ibHost = ibHost;
            _atmStrikeUtility = atmStrikeUtility;
            _configuration = configuration;
            _lastTradeDateConverter = lastTradeDateConverter;
        }

        public void Handle(MainWindowViewModel mainWindowViewModel, object[] parameters)
        {
            if (_ibHost == null)
            { 
                throw new ApplicationException("Unexpected! _ibHost is null"); 
            }
            var viewModel = _ibHost.ViewModel 
                ?? throw new ApplicationException("Unexpected! viewModel is null");
            var underlying = _configuration.GetValue(Configuration.UNDERLYING_SYMBOL).ToString() 
                ?? throw new ApplicationException("Unexpected! underlying is null");

            if(underlying != "SPY") 
            {
                throw new ApplicationException("There is only a implementation for SPY presently.");
            }
            
            int dte = (int)_configuration.GetValue(Configuration.DTE);
            var dteDateTime = _lastTradeDateConverter.DateTimeFromDte(dte);
            string lastTradeDate = _lastTradeDateConverter.FromDateTime(dteDateTime);
            int numberOfStrikes = (int)_configuration.GetValue(Configuration.NUMBER_OF_STRIKES);

            var underlyingPrice = (double)parameters[0];
            var strikes = _ibHost.GetStrikesSpy(underlyingPrice, lastTradeDate, numberOfStrikes);
            viewModel.Strikes = new ObservableCollection<double>(strikes);
        }
    }
}
