using IbClient.IbHost;

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
                   visitor.Positions.Add(p.Contract.Symbol);
               },
               () =>
               {
                   positionsRequested = true;
                   visitor.OnPropertyChanged(nameof(visitor.Positions));
               });

            await Task.Run(() => { while (!positionsRequested) { }; });
        }
    }
}
