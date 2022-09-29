namespace OptionHelper
{
    public interface IOptionHelper
    {
        double NextHigherStrike(double underlying, double step, double stepStrike, int precision);
        double NextLowerStrike(double underlying, double step, double stepStrike, int precision);
        double FirstCallStrikeByDelta(
           double targetDelta,
           double underlying,
           double stepUnderlying,
           double stepStike,
           double volatility,
           double interestRate,
           double dividendYield,
           double daysToExpiration,
           int precision
           );
        double FirstPutStrikeByDelta(
           double targetDelta,
           double underlying,
           double stepUnderlying,
           double stepStike,
           double volatility,
           double interestRate,
           double dividendYield,
           double daysToExpiration,
           int precision
           );
    }
}