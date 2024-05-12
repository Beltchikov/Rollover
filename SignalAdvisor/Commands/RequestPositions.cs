namespace SignalAdvisor.Commands
{
    class RequestPositions
    {
        public static void Run(ITwsVisitor RequestPositions)
        {
            RequestPositions.IbHost.RequestAccountSummaryAsync(
                "All", 
                App.ACCOUNT_SUMMARY_TAGS, 
                (a) =>
                {
                    var todo = 0;
                },
                (e) =>
                {
                    var todo = 0;
                });
        }
    }
}
