using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard
{
    public static class IntExtensions
    {
        /// <summary>
        /// 把数字转为字符串
        /// </summary>
        /// <param name="i"></param>
        /// <param name="placeholder"></param>
        /// <returns></returns>
        public static string ToStrForm(this int i, int digit, string placeholder)
        {
            var d = 10 * digit;
            var str = "";
            switch (digit)
            {
                case 3:
                    str = "K";
                    break;
                case 4:
                    str = "万";
                    break;
                default:
                    break;
            }
            if (i >= d)
            {
                return (i / d).ToString($"#{str}");
            }
            else if (i == 0)
            {
                return placeholder;
            }
            else
            {
                return i.ToString();
            }
        }

    }
}