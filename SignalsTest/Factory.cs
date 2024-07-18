using Ta.Indicators;

namespace SignalsTest
{
    public class Factory
    {
        public static Macd Create(string name)
        {
            return name.ToUpper() switch
            {
                "MACD" => new Macd(12, 26, 9, 235.318, 235.233, 0.125),
                _ => throw new NotImplementedException($"Not implemented for{name}"),
            };
        }
    }
}
