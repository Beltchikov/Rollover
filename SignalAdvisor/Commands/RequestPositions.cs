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
