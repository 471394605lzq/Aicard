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

    public class ConfigWeChatWork : IConfig
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

        public DateTime AccessTokenEnd { get { return accessTokenEnd; } set { accessTokenEnd = value; } }

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
    public class ConfigOpen : IConfig
    {
        private const string appID = "wxfa198eede0c55bc1";

        private const string appSecret = "e6c9b85d554c0697213ddba58d388148";

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