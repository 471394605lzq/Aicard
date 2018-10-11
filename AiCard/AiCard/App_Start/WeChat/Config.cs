using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace AiCard.WeChat
{

    public abstract class BaseConfig
    {
        public static string AppID { get; }

        public static string AppSecret { get; }

        public static string AccessToken { get; set; }

        public static string RefreshToken { get; set; }
    }

    //public abstract class Config
    //{
    //    public Config(Enums.WeChatAccount account)
    //    {
    //        switch (account)
    //        {
    //            case Enums.WeChatAccount.PC:
    //                AppID = "wxbd54d6bab8add1e0";
    //                AppSecret = "";
    //                break;
    //            case Enums.WeChatAccount.AiCardMini:
    //            default:
    //                AppID = "wx4f0894f87f291778";
    //                AppSecret = "b186bd1fe13e8a2fd1d2c66893941462";
    //                break;
    //        }
    //    }

    //    /// <summary>
    //    /// 网页开放平台
    //    /// </summary>
    //    public abstract string AppID { get; set; }

    //    /// <summary>
    //    /// 网页开放平台
    //    /// </summary>
    //    public abstract string AppSecret { get; set; }

    //    public const string JsapiTimeStamp = "1414587457";

    //    /// <summary>
    //    /// AppID对应的公共AccessToken
    //    /// </summary>
    //    public static string AccessToken = null;

    //    /// <summary>
    //    /// AppID对应的公共RefreshToken
    //    /// </summary>
    //    public static string RefreshToken = null;

    //    public static String JsSign(string url)
    //    {

    //        var api1 = new Api.BaseApi($"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={AppID}&secret={AppSecret}", "GET");
    //        var json = api1.CreateRequestReturnJson();
    //        var access_token = json["access_token"].Value<string>();
    //        var api2 = new Api.BaseApi($"https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={access_token}&type=jsapi", "GET");
    //        var josn2 = api2.CreateRequestReturnJson();
    //        var str = $"jsapi_ticket={josn2["ticket"].Value<string>()}&noncestr=Octopus&timestamp={JsapiTimeStamp}&url={url}";
    //        byte[] StrRes = Encoding.Default.GetBytes(str);
    //        HashAlgorithm iSHA = new SHA1CryptoServiceProvider();
    //        StrRes = iSHA.ComputeHash(StrRes);
    //        StringBuilder EnText = new StringBuilder();
    //        foreach (byte iByte in StrRes)
    //        {
    //            EnText.AppendFormat("{0:x2}", iByte);
    //        }
    //        return EnText.ToString();

    //    }
    //}

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