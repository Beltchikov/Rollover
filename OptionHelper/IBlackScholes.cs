namespace OptionHelper
{
    public interface IBlackScholes
    {
        double CallDelta(double underlying, double strike, double volatility, double interestRate, double dividendYield, double daysToExpiration);
        double CallPrice(double underlying, double strike, double volatility, double interestRate, double dividendYield, double daysToExpiration);
        double NextStrikeCall(double currentPrice, double strikeStep);
        double NextStrikePut(double currentPrice, double strikeStep);
        double PutDelta(double underlying, double strike, double volatility, double interestRate, double dividendYield, double daysToExpiration);
        double PutPrice(double underlying, double strike, double volatility, double interestRate, double dividendYield, double daysToExpiration);
    }
}