using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace AiCard.WeChat
{
    public class Config
    {
        /// <summary>
        /// 网页开放平台
        /// </summary>
        public const string AppID = "wx35d7769861d01a33";

        /// <summary>
        /// 网页开放平台
        /// </summary>
        public const string AppSecret = "db504014147f4e24b368a9e3196c0920";

        /// <summary>
        /// IOS
        /// </summary>
        public const string AppID2 = "wx85525eaae70512da";

        /// <summary>
        /// IOS
        /// </summary>
        public const string AppSecret2 = "5d30686583f3e3ca985dee737e3f7012";
        

        public const string JsapiTimeStamp = "1414587457";

        /// <summary>
        /// AppID对应的公共AccessToken
        /// </summary>
        public static string AccessToken = null;

        /// <summary>
        /// AppID对应的公共RefreshToken
        /// </summary>
        public static string RefreshToken = null;

        public static String JsSign(string url)
        {

            var api1 = new Api.BaseApi($"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={AppID}&secret={AppSecret}", "GET");
            var json = api1.CreateRequestReturnJson();
            var access_token = json["access_token"].Value<string>();
            var api2 = new Api.BaseApi($"https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={access_token}&type=jsapi", "GET");
            var josn2 = api2.CreateRequestReturnJson();
            var str = $"jsapi_ticket={josn2["ticket"].Value<string>()}&noncestr=Octopus&timestamp={JsapiTimeStamp}&url={url}";
            byte[] StrRes = Encoding.Default.GetBytes(str);
            HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
            StrRes = iSHA.ComputeHash(StrRes);
            StringBuilder EnText = new StringBuilder();
            foreach (byte iByte in StrRes)
            {
                EnText.AppendFormat("{0:x2}", iByte);
            }
            return EnText.ToString();

        }
    }
}