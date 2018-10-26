using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
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
            if (result["errcode"] != null)
            {
                throw new Exception(JsonConvert.SerializeObject(result));
            }


            var jscode2Session = new Jscode2sessionResult
            {
                OpenID = result["openid"].Value<string>(),
                UnionID = result["unionid"]?.Value<string>(),
                Session = result["session_key"].Value<string>(),
            };
            if (result["unionid"] == null)
            {
                Jscode2sessionResultList.SessionCache.Add(jscode2Session);
            }
            return jscode2Session;

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

        public string GetCardQrCode(int cardID)
        {
            var p = new Dictionary<string, string>();
            p.Add("page", "");
            p.Add("scene", Secret);
            var result = new AiCard.Api.BaseApi($"https://api.weixin.qq.com/wxa/getwxacodeunlimit?access_token={_accessToken}", "POST").CreateRequestReturnJson();
            return "";

        }
    }

    public class Jscode2sessionResult
    {
        public string OpenID { get; set; }

        public string UnionID { get; set; }

        public string Session { get; set; }

    }

    public static class Jscode2sessionResultList
    {
        static Jscode2sessionResultList()
        {
            SessionCache = new List<Jscode2sessionResult>();
        }

        public static List<Jscode2sessionResult> SessionCache { get; set; }


        public static string AESDecrypt(string text, string session, string iv)
        {
            try
            {
                //判断是否是16位 如果不够补0
                //text = tests(text);
                //16进制数据转换成byte
                byte[] encryptedData = Convert.FromBase64String(text);  // strToToHexByte(text);
                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                rijndaelCipher.Key = Convert.FromBase64String(session); // Encoding.UTF8.GetBytes(AesKey);
                rijndaelCipher.IV = Convert.FromBase64String(iv);// Encoding.UTF8.GetBytes(AesIV);
                rijndaelCipher.Mode = CipherMode.CBC;
                rijndaelCipher.Padding = PaddingMode.PKCS7;
                ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
                byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                string result = Encoding.Default.GetString(plainText);
                //int index = result.LastIndexOf('>');
                //result = result.Remove(index + 1);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}



