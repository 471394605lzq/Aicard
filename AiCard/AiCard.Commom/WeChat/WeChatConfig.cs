using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace AiCard.Common.WeChat
{

    public interface IConfig
    {
        string AppID { get; }

        string AppSecret { get; }

        string AccessToken { get; set; }

        string RefreshToken { get; set; }

        DateTime AccessTokenEnd { get; set; }
    }

    public class WeChatWorkConfig : IConfig
    {
        /// <summary>
        /// 网页开放平台
        /// </summary>
        private static string appID = "wxbd54d6bab8add1e0";
        //public static string AppID = "1000004";

        /// <summary>
        /// 网页开放平台
        /// </summary>
        private static string appSecret = "14c8f514897eeea2a02a5ffa6c3c4f32";
        //public static string AppSecret = "-1k1RzNA3Z1lRsOnXF-fYPBxHv4m6fN8zgU_BOA8Y98";


        /// <summary>
        /// AppID对应的公共AccessToken
        /// </summary>
        private static string accessToken = null;

        /// <summary>
        /// AppID对应的公共RefreshToken
        /// </summary>
        private static string refreshToken = null;

        private static DateTime accessTokenEnd;

        public string AccessToken { get { return accessToken; } set { accessToken = value; } }

        public string AppID { get { return appID; } }

        public string AppSecret { get { return appSecret; } }

        public string RefreshToken { get { return refreshToken; } set { refreshToken = value; } }

        public DateTime AccessTokenEnd { get { return accessTokenEnd; } set { accessTokenEnd = value; } }

        

        public static String JsSign(string url, string noncestr, string timestamp)
        {
            if (string.IsNullOrWhiteSpace(accessToken) || accessTokenEnd < DateTime.Now)
            {
                var api1 = new CommonApi.BaseApi($"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={appID}&secret={appSecret}", "GET");
                var json = api1.CreateRequestReturnJson();
                accessToken = json["access_token"].Value<string>();
                accessTokenEnd = DateTime.Now.AddSeconds(json["expires_in"].Value<int>());
            }

            var api2 = new CommonApi.BaseApi($"https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={accessToken}&type=jsapi", "GET");
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

    public class ConfigPc : IConfig
    {
        private const string appID = "wxbd54d6bab8add1e0";

        private const string appSecret = "14c8f514897eeea2a02a5ffa6c3c4f32";

        private static string accessToken = null;

        private static string refreshToken = null;

        private static DateTime accessTokenEnd;

        public string AccessToken { get { return accessToken; } set { accessToken = value; } }

        public DateTime AccessTokenEnd { get { return accessTokenEnd; } set { accessTokenEnd = value; } }

        public string AppID { get { return appID; } }

        public string AppSecret { get { return appSecret; } }

        public string RefreshToken { get { return refreshToken; } set { refreshToken = value; } }
    }

    public class ConfigMini : IConfig
    {
        private const string appID = "wx4f0894f87f291778";

        private const string appSecret = "b186bd1fe13e8a2fd1d2c66893941462";

        private static string accessToken = null;

        private static string refreshToken = null;

        private static DateTime accessTokenEnd;

        public string AccessToken { get { return accessToken; } set { accessToken = value; } }

        public DateTime AccessTokenEnd { get { return accessTokenEnd; }  set { accessTokenEnd = value; } }

        public string AppID { get { return appID; } }

        public string AppSecret { get { return appSecret; } }

        public string RefreshToken { get { return refreshToken; } set { refreshToken = value; } }
    }

    public class ConfigMiniPersonal : IConfig
    {
        private const string appID = "wxad81cad7f42d8097";

        private const string appSecret = "18d242324c0e74283c06c3858e374728";

        private static string accessToken = null;

        private static string refreshToken = null;

        private static DateTime accessTokenEnd;

        public string AccessToken { get { return accessToken; } set { accessToken = value; } }

        public string AppID { get { return appID; } }

        public string AppSecret { get { return appSecret; } }

        public string RefreshToken { get { return refreshToken; } set { refreshToken = value; } }

        public DateTime AccessTokenEnd { get { return accessTokenEnd; } set { accessTokenEnd = value; } }
    }
}