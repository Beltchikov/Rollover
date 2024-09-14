using StockAnalyzer.DataProviders;
using System.Collections.Generic;

namespace StockAnalyzer.Repositories
{
    public interface IRepository
    {
        List<string> Get(List<SymbolAndAccountingAttribute> symbolAndAccountingAttributesList);
    }
}