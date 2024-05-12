namespace SignalAdvisor.Commands
{
    class RequestPositions
    {
        public static void Run(IPositionsVisitor visitor)
        {
            visitor.IbHost.RequestPositions(
                (p) =>
                {
                    visitor.Positions.Add(p.Contract.Symbol);
                },
                () =>
                {
                    var t = 0;
                });
        }
    }
}
