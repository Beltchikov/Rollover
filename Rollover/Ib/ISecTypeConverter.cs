using IBApi;

namespace Rollover.Ib
{
    public interface ISecTypeConverter
    {
        Contract GetUnderlyingSecType(Contract derivativeContract);
    }
}