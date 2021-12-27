using IBApi;

namespace Rollover.Ib
{
    public interface IUnderlyingConverter
    {
        Contract GetUnderlying(Contract derivativeContract);
    }
}