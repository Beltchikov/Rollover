namespace SignalAdvisor.Commands
{
    class RequestPositions
    {
        public static async Task RunAsync(IPositionsVisitor visitor)
        {
            bool positionsRequested = false;
            await visitor.IbHost.RequestPositions(
               (p) =>
               {
                   visitor.Positions.Add(p);
                   visitor.Bars.Add(new KeyValuePair<string, List<Ta.Bar>>(p.Contract.Symbol, new List<Ta.Bar>()));
                   visitor.Signals.Add(new KeyValuePair<string, List<int>>(p.Contract.Symbol, new List<int>()));
               },
               () =>
               {
                   positionsRequested = true;
                   visitor.OnPropertyChanged(nameof(visitor.Positions));
                   visitor.RequestPositionsExecuted = true;
               });

            await Task.Run(() => { while (!positionsRequested) { }; });
        }
    }
}
