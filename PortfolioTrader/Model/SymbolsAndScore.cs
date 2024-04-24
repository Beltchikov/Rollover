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

        //public static Dictionary<string, TValue?> StringToDictionary<TValue>(string symbolsAsString) where TValue : class?
        //{
        //    return symbolsAsString
        //        .Split(Environment.NewLine)
        //        .Where(s => !string.IsNullOrWhiteSpace(s))
        //        .Select(s =>
        //        {
        //            var splitted = s.Trim().Split([' ', '\t']).Select(s => s.Trim()).ToList();
        //            if (splitted != null)
        //            {
        //                return new KeyValuePair<string, TValue?>(splitted[0], splitted[1] as TValue);
        //            }
        //            throw new Exception($"Unexpected. Can not build key value pair from the string {s}");
        //        })
        //        .ToDictionary();
        //}

        public static string DictionaryToString<TKey,TValue>(Dictionary<TKey, TValue> dictionary) where TKey: notnull
        {
            return dictionary.Any()
                ? dictionary
                   .Select(r => r.Key + "\t" + (r.Value==null? "": r.Value.ToString()))
                   .Aggregate((r, n) => r + Environment.NewLine + n)
                 : string.Empty;
        }
    }
}
