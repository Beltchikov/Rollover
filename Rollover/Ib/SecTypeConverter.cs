using IBApi;
using System.Collections.Generic;

namespace Rollover.Ib
{
    public class SecTypeConverter : ISecTypeConverter
    {
        public Contract GetUnderlyingSecType(Contract derivativeContract)
        {
            // SecType, Symbol, Currency
            // TODO
            return derivativeContract;

            //return _secTypeMap[derivativeSecType];
        }
    }
}