using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard
{
    public static class DateTimeExtensions
    {

        public static string ToStrForm(this DateTime datetime)
        {
            var ts = DateTime.Now - datetime;
            if (ts.TotalDays > 3)
            {
                return datetime.ToString("yyyy/MM/dd HH:mm");
            }
            else if (ts.TotalDays > 1)
            {
                return $"{(int)ts.TotalDays}天前";
            }
            else if (ts.TotalHours > 1)
            {
                return $"{(int)ts.TotalHours}小时前";
            }
            else
            {
                var min = (int)ts.TotalMinutes;
                return $"{(min < 1 ? 1 : min)}分钟前";
            }
        }

        /// <summary>
        /// 获取生日
        /// </summary>
        /// <param name="dateOfBirth"></param>
        /// <returns></returns>
        public static int? GetAgeForBirthday(this DateTime? dateOfBirth)
        {
            if (!dateOfBirth.HasValue)
            {
                return null;
            }
            return GetAgeForBirthday(dateOfBirth.Value);
        }

        /// <summary>
        /// 获取生日
        /// </summary>
        /// <param name="dateOfBirth"></param>
        /// <returns></returns>
        public static int GetAgeForBirthday(this DateTime dateOfBirth)
        {
            int age = 0;
            age = DateTime.Now.Year - dateOfBirth.Year;
            if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)
                age = age - 1;
            return age;
        }
    }
}