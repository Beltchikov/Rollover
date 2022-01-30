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
            input = input.Trim();
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
            if (input.Equals("q", StringComparison.InvariantCultureIgnoreCase))
            {
                return new List<string> { input };
            }
            if (input.Equals("h", StringComparison.InvariantCultureIgnoreCase))
            {
                return PrintHelp();
            }
            if (input.Equals("p", StringComparison.InvariantCultureIgnoreCase))
            {
                return _portfolio.Summary();
            }
            if (input.Equals("t", StringComparison.InvariantCultureIgnoreCase))
            {
                return _trackedSymbols.Summary();
            }
            if (input.StartsWith("r ", StringComparison.InvariantCultureIgnoreCase))
            {
                var localSymbol = input[2..].Trim();
                if (_portfolio.PositionBySymbol(localSymbol) == null)
                {
                    return new List<string> { "Symbol is not valid." };
                }
                RemoveTrackedSymbol(localSymbol);
                return new List<string> { $"Symbol {localSymbol} has been removed." };
            }

            var position = _portfolio.PositionBySymbol(input);
            if (position == null)
            {
                return new List<string> { "Symbol is not valid." };
            }

            List<string> messages = AddTrackedSymbol(position?.Contract, _trackedSymbols);
            return messages;
        }

        private void RemoveTrackedSymbol(string localSymbol)
        {
            _trackedSymbols.Remove(localSymbol);
        }

        private List<string> PrintHelp()
        {
            var help = new List<string> { "" };
            help.Add("Enter symbol from position list to track the position.");
            help.Add("Enter 'p' to list portfolio positions.");
            help.Add("Enter 't' to list tracked symbols.");
            help.Add("Enter 'q' to quit.");

            return help;
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
