using System;
using System.Globalization;

namespace SsbHedger.SsbChartControl.Utilities
{
    public class RoundingUtility : IRoundingUtility
    {
        public double RoundUsingTwoLastDigits(double price, string lastDigitsString)
        {
            int numberOfDigits = lastDigitsString.Length;
            int decimalPlacesInPrice = GetDecimalPlaces(price);
            int lastDigits = Convert.ToInt32(lastDigitsString);

            double result = 0;
            if(lastDigits == 0)
            {
                result = Math.Round(price, decimalPlacesInPrice - numberOfDigits);
            }
            else
            {
                int roundingKoef = (int)Math.Pow(10, numberOfDigits) / lastDigits;
                result = (Math.Round(price * roundingKoef, 0)) / roundingKoef;
            }

            return result;
        }

        private int GetDecimalPlaces(double price)
        {
            var priceString = price.ToString(CultureInfo.InvariantCulture);
            var splittedString = priceString.Split('.');
            return splittedString[1].Length;
        }
    }
}
