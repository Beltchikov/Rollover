using IBApi;
using IBSampleApp.messages;
using Rollover.Ib;
using Rollover.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Rollover.Input
{
    public class InputProcessor : IInputProcessor
    {
        private readonly IPortfolio _portfolio;
        private readonly ITrackedSymbols _trackedSymbols;
        private readonly IRepository _repository;

        public InputProcessor(
            IPortfolio portfolio,
            ITrackedSymbols trackedSymbols,
            IRepository repository)
        {
            _portfolio = portfolio;
            _trackedSymbols = trackedSymbols;
            _repository = repository;
        }

        public List<string> Convert(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new List<string>();
            }
            if (input.Contains("errorCode"))
            {
                return input.Contains("id=-1")
                    ? new List<string>()
                    : new List<string> { input };
            }
            if (input.Contains("Accounts found"))
            {
                return new List<string> { input };
            }
            if (input.Contains("Connected"))
            {
                return new List<string> { input };
            }

            var position = _portfolio.PositionBySymbol(input);
            var symbol = position?.Contract?.Symbol;
            if (position == null || symbol == null)
            {
                return new List<string> { "Symbol is not valid." };
            }

            List<string> messages = AddTrackedSymbol(position?.Contract, _trackedSymbols);
            return messages;
        }

        private List<string> AddTrackedSymbol(Contract contract, ITrackedSymbols trackedSymbols)
        {
            var contractDetails = _repository.ContractDetails(contract).FirstOrDefault();
            var trackedSymbol = new TrackedSymbol(
                contractDetails?.ContractDetails?.Contract?.LocalSymbol,
                contract.ConId,
                contractDetails?.ContractDetails?.Contract?.Exchange);

            if (!_trackedSymbols.Add(trackedSymbol))
            {
                return new List<string> { "Symbol is tracked already." };
            }

            return _trackedSymbols.Summary();
        }
    }
}
