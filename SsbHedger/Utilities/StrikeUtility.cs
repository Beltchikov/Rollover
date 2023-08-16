using SsbHedger.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SsbHedger.Utilities
{
    public class StrikeUtility : IStrikeUtility
    {
        public AtmStrikes[] AtmStrikeCandidates(double underlyingPrice, double strikesStep)
        {
            int decimalPlaces = DecimalPlaces(strikesStep);
            var firstAtmCandidate = Math.Round(underlyingPrice / strikesStep, 0) / (decimalPlaces + 1);

            if (firstAtmCandidate > underlyingPrice)
            {
                return new AtmStrikes[]
                {
                    new AtmStrikes(firstAtmCandidate, firstAtmCandidate - strikesStep),
                    new AtmStrikes(firstAtmCandidate, firstAtmCandidate - 2*strikesStep),

                    new AtmStrikes(firstAtmCandidate - strikesStep, firstAtmCandidate + strikesStep),
                    new AtmStrikes(firstAtmCandidate - strikesStep, firstAtmCandidate + 2*strikesStep),

                    new AtmStrikes(firstAtmCandidate - 2*strikesStep, firstAtmCandidate + strikesStep),
                    new AtmStrikes(firstAtmCandidate - 2*strikesStep, firstAtmCandidate + 2*strikesStep)
                };
            }
            else if (firstAtmCandidate < underlyingPrice)
            {
                return new AtmStrikes[]
               {
                    new AtmStrikes(firstAtmCandidate, firstAtmCandidate + strikesStep),
                    new AtmStrikes(firstAtmCandidate, firstAtmCandidate + 2*strikesStep),

                    new AtmStrikes(firstAtmCandidate + strikesStep, firstAtmCandidate - strikesStep),
                    new AtmStrikes(firstAtmCandidate + strikesStep, firstAtmCandidate - 2*strikesStep),

                    new AtmStrikes(firstAtmCandidate + 2*strikesStep, firstAtmCandidate - strikesStep),
                    new AtmStrikes(firstAtmCandidate + 2*strikesStep, firstAtmCandidate - 2*strikesStep)
               };
            }

            return new AtmStrikes[] { new AtmStrikes(firstAtmCandidate, firstAtmCandidate) };
        }

        private static int DecimalPlaces(double value)
        {
            return value.ToString().Length - ((int)value).ToString().Length == 0
                     ? value.ToString().Length - ((int)value).ToString().Length
                     : value.ToString().Length - ((int)value).ToString().Length - 1;
        }

        public void SetAtmStrikesInViewModel(IIbHost ibHost, double underlyingPrice)
        {

            if (underlyingPrice > 0)
            {
                var viewModel = ibHost.ViewModel;
                if (viewModel == null)
                {
                    throw new ApplicationException("Unexpected! viewModel is null");
                }

                viewModel.NextAtmStrike = -1;
                viewModel.SecondAtmStrike = -1;
                var atmStrikesCandidates = AtmStrikeCandidates(underlyingPrice, MainWindowViewModel.STRIKES_STEP);

                if (atmStrikesCandidates.Count() == 1)
                {
                    viewModel.NextAtmStrike = atmStrikesCandidates.First().NextAtmStrike;
                    viewModel.SecondAtmStrike = atmStrikesCandidates.First().SecondAtmStrike;
                }
                else
                {
                    foreach (var atmStrikesCandidate in atmStrikesCandidates)
                    {
                        ibHost.AtmStrikesCandidate = atmStrikesCandidate;
                        ibHost.ReqCheckNextOptionsStrike(ibHost.AtmStrikesCandidate.NextAtmStrike);
                        ibHost.ReqCheckSecondOptionsStrike(ibHost.AtmStrikesCandidate.SecondAtmStrike);

                        //ibHost.ReqMktDataNextCallOption(ibHost.AtmStrikesCandidate.NextAtmStrike);
                        //ibHost.ReqMktDataNextPutOption(ibHost.AtmStrikesCandidate.SecondAtmStrike);



                        var startTime = DateTime.Now;
                        while ((DateTime.Now - startTime).TotalMilliseconds < ibHost.Timeout) { }
                        if (viewModel.NextAtmStrike > 0 && viewModel.SecondAtmStrike > 0)
                        {
                            break;
                        }
                        else
                        {
                            viewModel.NextAtmStrike = -1;
                            viewModel.SecondAtmStrike = -1;
                        }
                    }

                    if (viewModel.NextAtmStrike <= 0 || viewModel.SecondAtmStrike <= 0)
                    {
                        throw new ApplicationException($"Unexpected! ATM strike could not be found. " +
                            $"NextAtmStrike:{viewModel.NextAtmStrike} " +
                            $"SecondAtmStrike:{viewModel.SecondAtmStrike}");

                        //viewModel.PositionsInfoMessage = $"Unexpected! ATM strike could not be found. " +
                        //    $"NextAtmStrike:{viewModel.NextAtmStrike} " +
                        //    $"SecondAtmStrike:{viewModel.SecondAtmStrike}";
                    }
                }
            }
        }
    }

    public record AtmStrikes(double NextAtmStrike, double SecondAtmStrike);
}
