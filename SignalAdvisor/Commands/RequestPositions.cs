namespace SignalAdvisor.Commands
{
    class RequestPositions
    {
        public static void Run(ITwsVisitor visitor)
        {
            visitor.IbHost.RequestPositions(
                (p) =>
                {
                    var t = 0;
                },
                () =>
                {
                    var t = 0;
                });
        }
    }
}
