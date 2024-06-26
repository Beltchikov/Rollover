﻿namespace PortfolioTrader.Repository
{
    internal interface IRepository
    {
        int? GetContractId(string symbol);
        IEnumerable<string> NotTradebleSymbols();
    }
}