using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard.WeChat
{
    public class WeChatMinApi
    {
        public WeChatMinApi(string appID, string secret)
        {
            AppID = appID;
            Secret = secret;
        }


        public string AppID { get; set; }

        public string Secret { get; set; }

        /// <summary>
        /// 使用Code换取OpenID和UnionID
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Jscode2sessionResult Jscode2session(string code)
        {
            var p = new Dictionary<string, string>();
            p.Add("appid", AppID);
            p.Add("secret", Secret);
            p.Add("js_code", code);
            p.Add("grant_type", "authorization_code");
            //throw new Exception(JsonConvert.SerializeObject($"https://api.weixin.qq.com/sns/jscode2session{p.ToParam("?")}"));
            var result = new AiCard.Api.BaseApi($"https://api.weixin.qq.com/sns/jscode2session{p.ToParam("?")}", "GET").CreateRequestReturnJson(); ;
            if (result["errcode"] != null)
            {
                throw new Exception(JsonConvert.SerializeObject(result));
            }
            return new Jscode2sessionResult
            {
                OpenID = result["openid"].Value<string>(),
                UnionID = result["unionid"].Value<string>(),
            };

        }


    }

    public class Jscode2sessionResult
    {
        public string OpenID { get; set; }

        public string UnionID { get; set; }

        public string Session { get; set; }
    }
}