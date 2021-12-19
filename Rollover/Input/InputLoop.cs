using Rollover.Ib;
using Rollover.Tracking;
using System;
using System.Diagnostics;

namespace Rollover.Input
{
    public class InputLoop : IInputLoop
    {
        private IOutputHelper _outputHelper;
        private IConnectedCondition _connectedCondition;
        private IPortfolio _portfolio;
        private ITrackedSymbols _trackedSymbols;
        private string _state;
        private IReducer _reducer;
        IInputQueue _inputQueue;

        public InputLoop(
            IOutputHelper outputHelper,
            IConnectedCondition connectedCondition,
            IPortfolio portfolio,
            ITrackedSymbols trackedSymbols, 
            IReducer reducer)
        {
            _outputHelper = outputHelper;
            _connectedCondition = connectedCondition;
            _portfolio = portfolio;
            _trackedSymbols = trackedSymbols;
            _reducer = reducer;

            _reducer.StateChanged += OnStateChanged;
        }

        public bool CheckConnectionMessages(IConsoleWrapper consoleWrapper, IInputQueue inputQueue, int timeout)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            while (stopWatch.Elapsed.TotalMilliseconds < timeout)
            {
                var input = inputQueue.Dequeue();
                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                var outputList = _outputHelper.Convert(input);
                outputList.ForEach(o => consoleWrapper.WriteLine(o));

                _connectedCondition.AddInput(input);
                if (_connectedCondition.IsConnected())
                {
                    return true;
                }
            }

            return false;
        }

        public void Run(IConsoleWrapper consoleWrapper, IInputQueue inputQueue)
        {
            _inputQueue = inputQueue;
            _state = "Connected";

            while (true)
            {
                var input = inputQueue.Dequeue();
                _state = _reducer.GetState(_state, input);
                if (input == null)
                {
                    continue;
                }

                var outputList = _outputHelper.Convert(input);
                outputList.ForEach(o => consoleWrapper.WriteLine(o));

                if (input.Equals("q", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }

                if (_portfolio.SymbolExists(input))
                {
                    if (! _trackedSymbols.SymbolExists(input))
                    {
                        _trackedSymbols.Add(input);
                        //inputQueue.Enqueue("Symbol added");
                    }
                }
            }

            consoleWrapper.WriteLine("Goodbye!");
        }

        private void OnStateChanged(object sender, StateChangedEventArgs e)
        {
            _inputQueue.Enqueue($"STATE:{e.State}");
        }

    }
}
