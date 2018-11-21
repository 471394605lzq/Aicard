using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace AiCard.Common.WeChat
{

    public abstract class BaseConfig
    {
        public static string AppID { get; }

        public static string AppSecret { get; }

        public static string AccessToken { get; set; }

        public static string RefreshToken { get; set; }
    }

    public class WeChatWorkConfig
    {
        /// <summary>
        /// 网页开放平台
        /// </summary>
        public static string AppID = "wxbd54d6bab8add1e0";
        //public static string AppID = "1000004";

        /// <summary>
        /// 网页开放平台
        /// </summary>
        public static string AppSecret = "14c8f514897eeea2a02a5ffa6c3c4f32";
        //public static string AppSecret = "-1k1RzNA3Z1lRsOnXF-fYPBxHv4m6fN8zgU_BOA8Y98";


        /// <summary>
        /// AppID对应的公共AccessToken
        /// </summary>
        public static string AccessToken = null;

        /// <summary>
        /// AppID对应的公共RefreshToken
        /// </summary>
        public static string RefreshToken = null;

        public static DateTime? Expires = null;

        public static String JsSign(string url, string noncestr, string timestamp)
        {
            if (string.IsNullOrWhiteSpace(AccessToken)|| Expires.Value < DateTime.Now)
            {
                var api1 = new CommonApi.BaseApi($"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={AppID}&secret={AppSecret}", "GET");
                var json = api1.CreateRequestReturnJson();
                AccessToken = json["access_token"].Value<string>();
                Expires = DateTime.Now.AddSeconds(json["expires_in"].Value<int>());
            }

            var api2 = new CommonApi.BaseApi($"https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={AccessToken}&type=jsapi", "GET");
            try
            {
                var josn2 = api2.CreateRequestReturnJson();

                var str = $"jsapi_ticket={josn2["ticket"].Value<string>()}&noncestr={noncestr}&timestamp={timestamp}&url={url}";
                Comm.WriteLog("JsSign", str, Enums.DebugLogLevel.Normal);
                byte[] StrRes = Encoding.Default.GetBytes(str);
                HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
                StrRes = iSHA.ComputeHash(StrRes);
                StringBuilder EnText = new StringBuilder();
                foreach (byte iByte in StrRes)
                {
                    EnText.AppendFormat("{0:x2}", iByte);
                }
                Comm.WriteLog("JsSign", EnText.ToString(), Enums.DebugLogLevel.Normal);
                return EnText.ToString();
            }
            catch (Exception ex)
            {
                Comm.WriteLog("JsSign", ex.Message, Enums.DebugLogLevel.Normal);
                return "Error";
            }



        }
    }

    public static class ConfigPc
    {
        public const string AppID = "wxbd54d6bab8add1e0";

        public const string AppSecret = "14c8f514897eeea2a02a5ffa6c3c4f32";

        public static string AccessToken = null;

        public static string RefreshToken = null;
    }

    public static class ConfigMini
    {
        public const string AppID = "wx4f0894f87f291778";

        public const string AppSecret = "b186bd1fe13e8a2fd1d2c66893941462";

        public static string AccessToken = null;

        public static string RefreshToken = null;

    }
}