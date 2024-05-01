using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PortfolioTrader.Model
{
    internal class SymbolsAndScore
    {
        public static Dictionary<string, int> StringToDictionary(string symbolsAsString)
        {
            return symbolsAsString
                .Split(Environment.NewLine)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s =>
                {
                    var splitted = s.Trim().Split([' ', '\t']).Select(s => s.Trim()).ToList();
                    if (splitted != null)
                    {
                        if (Int32.TryParse(splitted[1], out int netBms))
                            return new KeyValuePair<string, int>(splitted[0], netBms);
                        else
                            return new KeyValuePair<string, int>(splitted[0], 0);
                    }
                    throw new Exception($"Unexpected. Can not build key value pair from the string {s}");
                })
                .ToDictionary();
        }

        public static Dictionary<string, Position> StringToPositionDictionary(string symbolsAsString)
        {
            return symbolsAsString
                .Split(Environment.NewLine)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s =>
                {
                    var splitted = s.Trim().Split([' ', '\t']).Select(s => s.Trim()).ToList();
                    if (splitted != null) return new KeyValuePair<string, Position>(splitted[0], new Position(splitted));
                    throw new Exception($"Unexpected. Can not build key value pair from the string {s}");
                })
                .ToDictionary();
        }

        public static string DictionaryToString<TKey, TValue>(Dictionary<TKey, TValue> dictionary) where TKey : notnull
        {
            return dictionary.Any()
                ? dictionary
                   .Select(r => r.Key + "\t" + (r.Value == null ? "" : r.Value.ToString()))
                   .Aggregate((r, n) => r + Environment.NewLine + n)
                 : string.Empty;
        }

        public static string PositionDictionaryToString(Dictionary<string, Position> dictionary)
        {
            return dictionary.Any()
                ? dictionary
                   .Select(r =>
                   {
                       string line = r.Key + "\t" + r.Value.NetBms.ToString();
                       line += r.Value.ConId == null ? "" : "\t" + r.Value.ConId.ToString();
                       line += r.Value.PriceInCents == null ? "" : "\t" + r.Value.PriceInCents.ToString();
                       line += r.Value.PriceType == null ? "" : "\t" + r.Value.PriceType.ToString();
                       line += r.Value.Weight == null ? "" : "\t" + r.Value.Weight.ToString();
                       line += r.Value.Quantity == null ? "" : "\t" + r.Value.Quantity.ToString();
                       line += r.Value.Margin == null ? "" : "\t" + r.Value.Margin.ToString();
                       return line;
                   })
                   .Aggregate((r, n) => r + Environment.NewLine + n)
                 : string.Empty;
        }

        public static string ListToCsvString(List<string> listOfString, string separator)
        {
            string newLineSeparatedString = "";
            if (listOfString.Any()) newLineSeparatedString = listOfString.Aggregate((r, n) => r + separator + n);
            return newLineSeparatedString;
        }

        public static string ConcatStringsWithNewLine(string string1, string string2)
        {
            return string1 == ""
                ? string2
                : string1 + Environment.NewLine + string2;
        }
    }
}
