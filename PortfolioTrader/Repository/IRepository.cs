namespace SignalAdvisor.Repository
{
    internal interface IRepository
    {
        int? GetContractId(string symbol);
        IEnumerable<string> NotTradebleSymbols();
    }
}