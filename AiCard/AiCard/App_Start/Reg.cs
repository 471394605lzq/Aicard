using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace AiCard
{
    public static class Reg
    {
        /// <summary>
        /// 手机号正则表达式
        /// </summary>
        public const string MOBILE = @"^(13[0-9]|14[579]|15[0-3,5-9]|17[0135678]|18[0-9])\d{8}$";
        /// <summary>
        /// 电话号正则表达式
        /// </summary>
        public const string PHONE = @"(^(\d{3,4}-)?\d{7,8}$)|^(13[0-9]|14[579]|15[0-3,5-9]|17[0135678]|18[0-9])\d{8}$";
        /// <summary>
        /// 邮件正则表达式
        /// </summary>
        public const string EMAIL = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        /// <summary>
        /// 身份证正则表达式
        /// </summary>
        public const string IDCARD = @"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$";

        private static Regex regMobile = new Regex(MOBILE);
        private static Regex regPhone = new Regex(PHONE);
        private static Regex regEmail = new Regex(EMAIL);
        private static Regex regIDCard = new Regex(IDCARD);

        /// <summary>
        /// 是否满足手机号格式
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsMobile(string text)
        {
            return regMobile.IsMatch(text);
        }


        /// <summary>
        /// 是否满足电话号格式
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsPhone(string text)
        {
            return regPhone.IsMatch(text);
        }

        /// <summary>
        /// 是否满足Email格式
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsEmail(string text)
        {
            return regEmail.IsMatch(text);
        }

        /// <summary>
        /// 是否满足身份证格式
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsIDCard(string text)
        {
            return regIDCard.IsMatch(text);
        }

    }
}