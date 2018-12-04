

using AiCard.Common.Qiniu;
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

namespace AiCard.Common.WeChat
{
    public class WeChatMinApi
    {
        private IConfig _config;

        public WeChatMinApi(IConfig config)
        {
            _config = config;
        }

        public string AppID { get { return _config.AppID; } }

        public string Secret { get { return _config.AppSecret; } }

        /// <summary>
        /// 获取token
        /// </summary>
        /// <returns></returns>
        //public string GetAccessToken()
        //{
        //    var date = DateTime.Now;
        //    var api = new Api.BaseApi($"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={AppID}&secret={Secret}", "GET");
        //    var result = api.CreateRequestReturnJson();
        //    _accessToken = new WeChat.AccessToken()
        //    {
        //        Code = result["access_token"].Value<string>(),
        //        End = DateTime.Now.AddSeconds(3500)
        //    };
        //    return _accessToken.Code;
        //}

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
            var result = new CommonApi.BaseApi($"https://api.weixin.qq.com/sns/jscode2session{p.ToParam("?")}", "GET").CreateRequestReturnJson();
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
            Jscode2sessionResultList.SetSession(jscode2Session.OpenID, jscode2Session.Session);
            return jscode2Session;

        }
        /// <summary>
        /// 获取小程序码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetWXACodeUnlimit(Enum type, Dictionary<string, string> scene)
        {
            var p = new Dictionary<string, string>();
            p.Add("access_token", GetAccessToken());

            string path = $"Type={Convert.ToInt32(type)}&{scene.ToParam()}";
            string page = "";//中转页面
            if (_config.AppID == new ConfigMini().AppID)
            {
                page = $"pages/test1/test1";
            }
            else if (_config.AppID == new ConfigMiniPersonal().AppID)
            {
                page = $"pages/test1/test1";
            }
            var data = new
            {
                page = page,
                scene = path,
                is_hyaline = true,
                //line_color= "{ 'r':'255','g':'255','b':'255'}"
            };
            var result = new CommonApi.BaseApi($"https://api.weixin.qq.com/wxa/getwxacodeunlimit{p.ToParam("?")}", "POST", data).CreateRequest();

            string codeName = $"{path.Replace("&", "_").Replace("=", "_")}.png";
            var pDir = HttpContext.Current.Server.MapPath("~/Upload/");

            //判断返回的文件流是否是png格式
            if (SteamCheck.IsAllowedExtension(result, FileType.Png))
            {
                System.Drawing.Image img = System.Drawing.Image.FromStream(result);

                string newimgpath = $"{pDir}{codeName}";
                img.Save(newimgpath, ImageFormat.Png);
                string resultpath = null;
                try
                {
                    QinQiuApi qniu = new QinQiuApi();
                    resultpath = qniu.UploadFile(newimgpath, false, true);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    File.Delete(newimgpath);
                }
                return resultpath;

            }
            else
            {
                string txtData = "";
                if (result == null)
                {
                    return null;
                }
                using (var reader = new StreamReader(result))
                {
                    txtData = reader.ReadToEnd();
                }
                throw new Exception(txtData);
            }

        }
        //public string ReturnImg(byte[] streamByte)
        //{
        //    System.IO.MemoryStream ms = new System.IO.MemoryStream(streamByte);
        //    System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
        //    string newimgpath = "E:\"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + ".png";
        //    img.Save(newimgpath, ImageFormat.Png);
        //    return newimgpath;
        //}

        public Stream GetCardQrCode(Dictionary<string, string> para)
        {
            var p = new Dictionary<string, string>();
            p.Add("access_token", GetAccessToken());
            var data = new
            {
                page = $"pages/test1/test1",
                scene = para.ToParam(),
                is_hyaline = true,
                //line_color= "{ 'r':'255','g':'255','b':'255'}"
            };
            return new CommonApi.BaseApi($"https://api.weixin.qq.com/wxa/getwxacodeunlimit{p.ToParam("?")}", "POST", data).CreateRequest();
        }

        public Stream GetCardQrCode(string para)
        {
            var p = new Dictionary<string, string>();
            p.Add("access_token", GetAccessToken());
            var data = new
            {
                page = $"pages/test1/test1",
                scene = para,
                is_hyaline = true,
                //line_color= "{ 'r':'255','g':'255','b':'255'}"
            };
            return new CommonApi.BaseApi($"https://api.weixin.qq.com/wxa/getwxacodeunlimit{p.ToParam("?")}", "POST", data).CreateRequest();
        }

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <returns></returns>
        public string GetAccessToken()
        {
            if (_config.AccessToken == null || _config.AccessTokenEnd <= DateTime.Now)
            {
                RefreshToken();
            }
            return _config.AccessToken;
        }

        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <returns></returns>
        public string RefreshToken()
        {
            //var date = DateTime.Now;
            //if (_config.AccessToken != null && _config.AccessTokenEnd < date)
            //{
            //    return _config.AccessToken;
            //}
            var api = new CommonApi.BaseApi($"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={AppID}&secret={Secret}", "GET");
            var result = api.CreateRequestReturnJson();
            _config.AccessToken = result["access_token"].Value<string>();
            _config.AccessTokenEnd = DateTime.Now.AddSeconds(3500);
            return _config.AccessToken;
        }

        public string SendMessage(string openID, string tempID, string formID, string page, object keyword)
        {
            var p = new Dictionary<string, string>();
            p.Add("access_token", GetAccessToken());
            var data = new
            {
                touser = openID,
                template_id = tempID,
                page = page,
                data = keyword,
                form_id = formID,
            };
            var api = new CommonApi.BaseApi($"https://api.weixin.qq.com/cgi-bin/message/wxopen/template/send?{p.ToParam()}", "POST", data);
            var result = api.CreateRequestReturnJson();
            return JsonConvert.SerializeObject(result);
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


        public static void SetSession(string openID, string session)
        {
            if (string.IsNullOrWhiteSpace(openID) || string.IsNullOrWhiteSpace(session))
            {
                throw new Exception($"参数有有误：openID:{openID},session:{session}");
            }
            var d = HttpContext.Current.Server.MapPath($"~/Session");
            if (!Directory.Exists(d))
            {
                Directory.CreateDirectory(d);
            }
            var path = HttpContext.Current.Server.MapPath($"~/Session/{openID}.txt");
            System.IO.File.WriteAllText(path, session);
        }

        public static string GetSession(string openID)
        {
            var path = HttpContext.Current.Server.MapPath($"~/Session/{openID}.txt");
            if (!File.Exists(path))
            {
                throw new Exception($"openID：{openID}的缓存不存在");
            }
            string session = System.IO.File.ReadAllText(path);
            //System.IO.File.Delete(path);
            return session;
        }

        public static string AESDecrypt(string text, string session, string iv)
        {
            try
            {
                text = text.Replace(" ", "+");
                session = session.Replace(" ", "+");
                iv = iv.Replace(" ", "+");
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

        public static string AESDecryptPhoneNumber(string encryptedData, string session, string iv)
        {
            try
            {
                //从EncryptedData从解密用户数据
                var str = AESDecrypt(encryptedData, session, iv);
                var jObj = JsonConvert.DeserializeObject<JToken>(str);
                return jObj["purePhoneNumber"].Value<string>();
            }
            catch (Exception)
            {
                //Comm.WriteLog("CreateByWeChatPhoneDecrypt", JsonConvert.SerializeObject(new { encryptedData, session, iv }), Common.Enums.DebugLogLevel.Error);
                throw new Exception("手机号解密失败");
            }
        }
    }



    public enum WeChatPage
    {
        CardDetail = 1

    }

    public enum WeChatPagePersonal
    {
        CardDetail = 1

    }

}



