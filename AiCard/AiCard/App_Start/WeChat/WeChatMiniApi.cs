using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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

        private static AccessToken _accessToken = null;

        public string AppID { get; set; }

        public string Secret { get; set; }
        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns></returns>
        public string GetAccessToken()
        {
            var date = DateTime.Now;
            var api = new Api.BaseApi($"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={AppID}&secret={Secret}", "GET");
            var result = api.CreateRequestReturnJson();
            _accessToken = new WeChat.AccessToken()
            {
                Code = result["access_token"].Value<string>(),
                End = DateTime.Now.AddSeconds(3500)
            };
            return _accessToken.Code;
        }
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
            var result = new AiCard.Api.BaseApi($"https://api.weixin.qq.com/sns/jscode2session{p.ToParam("?")}", "GET").CreateRequestReturnJson();
            return new Jscode2sessionResult
            {
                OpenID = result["openid"].Value<string>(),
                UnionID = result["unionid"].Value<string>(),
                Session = result["session_key"].Value<string>(),
            };

        }
        /// <summary>
        /// 获取小程序码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetWXACodeUnlimit()
        {
            string codeimgpath = "";
            var p = new Dictionary<string, string>();
            p.Add("access_token", GetAccessToken());
            p.Add("scene", "dtkj");
            p.Add("is_hyaline", "true");
            var result = new AiCard.Api.BaseApi($"POST https://api.weixin.qq.com/wxa/getwxacodeunlimit{p.ToParam("?")}", "GET").CreateRequestReturnJson();
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, result);

                if (result["errcode"] != null)
                {
                    throw new Exception(JsonConvert.SerializeObject(result));
                }
                else
                {
                    codeimgpath = ReturnImg(ms.GetBuffer());
                }
                return codeimgpath;
            }
        }
        public string ReturnImg(byte[] streamByte)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream(streamByte);
            System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
            string newimgpath = "E:\"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + ".png";
            img.Save(newimgpath, ImageFormat.Png);
            return newimgpath;
        }


    }

    public class Jscode2sessionResult
    {
        public string OpenID { get; set; }

        public string UnionID { get; set; }

        public string Session { get; set; }

    }
}