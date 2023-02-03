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
            int decimalPlacesAfterRounding = decimalPlacesInPrice - numberOfDigits;
            int lastDigits = Convert.ToInt32(lastDigitsString);

            double result = 0;
            if(lastDigits == 0)
            {
                var koef = Math.Pow(10, decimalPlacesAfterRounding);
                result = Math.Ceiling(price * koef) / koef;
            }
            else
            {
                int roundingKoef = (int)Math.Pow(10, numberOfDigits) / lastDigits;
                result = (Math.Ceiling(price * roundingKoef)) / roundingKoef;
            }

            return result;
        }

        private int GetDecimalPlaces(double price)
        {
            var priceString = price.ToString(CultureInfo.InvariantCulture);
            var splittedStringArray = priceString.Split('.');
            if(splittedStringArray.Length < 2) 
            {
                return 0;
            }

            return splittedStringArray[1].Length;
        }
    }
}
