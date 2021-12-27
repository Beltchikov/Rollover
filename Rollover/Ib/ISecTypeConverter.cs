namespace Rollover.Ib
{
    public interface ISecTypeConverter
    {
        string GetUnderlyingSecType(string derivativeSecType);
    }
}