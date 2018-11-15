using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard.Common.Extensions
{
    public static class DictionaryExtensions
    {
        public static string ToParam<TKey, TValue>(this Dictionary<TKey, TValue> dic, string headChar = "")
        {
            if (dic == null || dic.Count == 0)
            {
                return "";
            }
            string result = string.Join("&", dic.Select(s => $"{s.Key}={s.Value}"));
            if (!string.IsNullOrWhiteSpace(headChar))
            {
                result = $"{headChar}{result}"; 
            }
            return result;
        }
    }
}