using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    if (splitted != null) return new KeyValuePair<string, int>(splitted[0], Convert.ToInt32(splitted[1]));
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
                       line += r.Value.Weight == null ? "" : "\t" + r.Value.Weight.ToString();
                       line += r.Value.PriceInCents == null ? "" : "\t" + r.Value.PriceInCents.ToString();
                       line += r.Value.PriceType == null ? "" : "\t" + r.Value.PriceType.ToString();
                       line += r.Value.Quantity == null ? "" : "\t" + r.Value.Quantity.ToString();
                       line += r.Value.Margin == null ? "" : "\t" + r.Value.Margin.ToString();
                       return line;
                   })
                   .Aggregate((r, n) => r + Environment.NewLine + n)
                 : string.Empty;
        }
    }
}
