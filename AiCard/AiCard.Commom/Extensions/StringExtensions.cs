using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard
{
    public static class StringExtensions
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        /// <param name="strIDs"></param>
        /// <param name="separator">默认值,</param>
        /// <returns></returns>
        public static List<int> SplitToIntArray(this String strIDs, char separator = ',')
        {
            return strIDs.SplitToArray<int>(separator);
        }

        /// <summary>
        /// 返回数据,int,decimal,double,float,bool经过特殊处理，不会返回不能转换字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="separator">默认值,</param>
        /// <returns></returns>
        public static List<T> SplitToArray<T>(this String str, char separator = ',')
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }
            var tt = typeof(T);
            var temp = str.Split(separator).Where(s => !string.IsNullOrEmpty(s));
            if (tt == typeof(int))
            {
                int t;
                temp = temp.Where(s => int.TryParse(s, out t));
            }
            else if (tt == typeof(decimal))
            {
                decimal t;
                temp = temp.Where(s => decimal.TryParse(s, out t));
            }
            else if (tt == typeof(double))
            {
                double t;
                temp = temp.Where(s => double.TryParse(s, out t));
            }
            else if (tt == typeof(float))
            {
                float t;
                temp = temp.Where(s => float.TryParse(s, out t));
            }
            else if (tt == typeof(bool))
            {
                bool t;
                temp = temp.Where(s => bool.TryParse(s, out t));
            }
            if (tt.IsEnum)
            {
                return temp.Where(s =>
                {
                    try
                    {
                        Enum.Parse(typeof(T), s, true);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }

                })
                .Select(s => (T)Enum.Parse(typeof(T), s, true)).ToList();
            }
            return temp.Select(s => (T)Convert.ChangeType(s, typeof(T))).ToList();
        }

        public static string HiddenPhoneNum(string number)
        {
            return $"{number.Remove(2)}***{number.Remove(0, number.Length - 2)}";
        }

        public static List<int> AllIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }
    }
}