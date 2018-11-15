using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net;
using System.Text;
using System.IO;
using AiCard.Common.Enums;
using AiCard.Common.Extensions;

namespace AiCard.Common.WeChat
{
    public class WeChatApi
    {
        public WeChatApi(string appID, string secret)
        {
            AppID = appID;
            Secret = secret;
        }

        private static AccessToken _accessToken = null;



        public string AppID { get; set; }

        public string Secret { get; set; }

        /// <summary>
        /// 使用Code换取OpenID，UnionID，Token等
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public AccessTokenResult GetAccessTokenSns(string code)
        {
            var p = new Dictionary<string, string>();
            p.Add("appid", AppID);
            p.Add("secret", Secret);
            p.Add("code", code);
            var result = new CommonApi.BaseApi($"https://api.weixin.qq.com/sns/oauth2/access_token{p.ToParam("?")}", "GET").CreateRequestReturnJson();
            if (result["errcode"] != null)
            {
                throw new Exception(JsonConvert.SerializeObject(result));
            }
            return new AccessTokenResult
            {
                OpenID = result["openid"].Value<string>(),
                AccessToken = result["access_token"].Value<string>(),
                UnionID = result["unionid"].Value<string>(),
                RefreshToken = result["refresh_token"].Value<string>()
            };
        }


        /// <summary>
        /// 刷新微信登录的Token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public AccessTokenResult RefreshAccessTokenSns(string refreshToken)
        {
            var p = new Dictionary<string, string>();
            p.Add("appid", AppID);
            p.Add("grant_type", "refresh_token");
            p.Add("refresh_token", refreshToken);
            var result = new CommonApi.BaseApi($"https://api.weixin.qq.com/sns/oauth2/refresh_token{p.ToParam("?")}", "GET").CreateRequestReturnJson();
            if (result["errcode"] != null)
            {
                throw new Exception(JsonConvert.SerializeObject(result));
            }
            return new AccessTokenResult
            {
                OpenID = result["openid"].Value<string>(),
                AccessToken = result["access_token"].Value<string>(),
                UnionID = result["unionid"].Value<string>(),
                RefreshToken = result["refresh_token"].Value<string>()
            };
        }

        /// <summary>
        /// 获取微信用户基本信息，通过openID,Token内嵌
        /// </summary>
        /// <param name="openID"></param>
        /// <returns></returns>
        public UserInfoResult GetUserInfoCgi(string openID)
        {
            var p = new Dictionary<string, string>();
            p.Add("access_token", GetAccessToken());
            p.Add("openid", openID);
            p.Add("lang", "zh_CN");
            try
            {
                var result = new CommonApi.BaseApi($"https://api.weixin.qq.com/cgi-bin/user/info{p.ToParam("?")}", "GET")
                 .CreateRequestReturnJson();
                if (result["errcode"] != null)
                {
                    throw new Exception(JsonConvert.SerializeObject(result));
                }
                return new UserInfoResult
                {
                    NickName = result["nickname"].Value<string>(),
                    HeadImgUrl = result["headimgurl"].Value<string>(),
                    UnionID = result["unionid"].Value<string>(),
                    IsSubscribe = result["subscribe"]?.Value<int>() == 1
                };
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// 获取微信用户基本信息
        /// </summary>
        /// <param name="openID"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public UserInfoResult GetUserInfoSns(string openID, string accessToken)
        {
            var p = new Dictionary<string, string>();
            p.Add("access_token", accessToken);
            p.Add("openid", openID);
            p.Add("lang", "zh_CN");
            var result = new CommonApi.BaseApi($"https://api.weixin.qq.com/sns/userinfo{p.ToParam("?")}", "GET").CreateRequestReturnJson();
            if (result["errcode"] != null)
            {
                throw new Exception(JsonConvert.SerializeObject(result));
            }
            return new UserInfoResult
            {
                NickName = result["nickname"].Value<string>(),
                HeadImgUrl = result["headimgurl"].Value<string>(),
                UnionID = result["unionid"].Value<string>(),
            };
        }

        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string SendTextMessage(string to, string from, string content)
        {
            return new XDocument(new XElement("xml",
                   new XElement("ToUserName", to),
                   new XElement("FromUserName", from),
                   new XElement("CreateTime", DateTime.Now.Ticks),
                   new XElement("MsgType", "text"),
                   new XElement("Content", content),
                   new XElement("MsgId", Comm.Random.Next(100000, 999999))
               )).ToString();
        }

        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string SendNewsMessage(string to, string from, List<NewsMessage> news)
        {
            return new XDocument(new XElement("xml",
                  new XElement("ToUserName", to),
                  new XElement("FromUserName", from),
                  new XElement("CreateTime", DateTime.Now.Ticks),
                  new XElement("MsgType", "news"),
                  new XElement("ArticleCount", news.Count),
                  new XElement("Articles",
                  news.Select(s =>
                  {
                      return new XElement("item",
                          new XElement("Title", s.Title),
                          new XElement("Description", s.Description),
                          new XElement("PicUrl", Comm.ResizeImage(s.PicUrl)),
                          new XElement("Url", s.Url));
                  })
              ))).ToString();
        }

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <returns></returns>
        public string GetAccessToken()
        {
            if (_accessToken == null || _accessToken.End <= DateTime.Now)
            {
                RefreshToken();
            }
            return _accessToken.Code;
        }

        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <returns></returns>
        public string RefreshToken()
        {
            var date = DateTime.Now;
            var api = new CommonApi.BaseApi($"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={AppID}&secret={Secret}", "GET");
            var result = api.CreateRequestReturnJson();
            _accessToken = new WeChat.AccessToken()
            {
                Code = result["access_token"].Value<string>(),
                End = DateTime.Now.AddSeconds(3500)
            };
            return _accessToken.Code;
        }

        public string GetMenu()
        {
            string accessToken = GetAccessToken();
            var url = $"https://api.weixin.qq.com/cgi-bin/menu/get?access_token={accessToken}";
            var api = new CommonApi.BaseApi(url, "GET");
            var result = api.CreateRequestReturnString();
            return result;
        }


        /// <summary>
        /// 修改微信菜单
        /// </summary>
        /// <param name="menu"></param>
        public void CreateMenu(string menu)
        {
            var jMenu = JsonConvert.DeserializeObject<JObject>(menu);
            string accessToken = GetAccessToken();
            var url = $"https://api.weixin.qq.com/cgi-bin/menu/create?access_token={accessToken}";
            var api = new CommonApi.BaseApi(url, "POST", jMenu);
            var result = api.CreateRequestReturnJson();
            if (result["errcode"].Value<string>() != "0")
            {
                throw new Exception(result["errmsg"].Value<string>());
            }
        }

        /// <summary>
        /// 获取微信的推送消息模版
        /// </summary>
        /// <returns></returns>
        public List<TempMessage> GetAllTempMessage()
        {
            var url = $"  https://api.weixin.qq.com/cgi-bin/template/get_all_private_template?access_token={GetAccessToken()}";
            var api = new CommonApi.BaseApi(url, "GET");
            var result = api.CreateRequestReturnJson();

            return result["template_list"].Values<JObject>().Select(s => new TempMessage
            {
                Content = s["content"].Value<string>(),
                DeputyID = s["deputy_industry"].Value<string>(),
                ID = s["template_id"].Value<string>(),
                PrimaryID = s["primary_industry"].Value<string>(),
                Title = s["title"].Value<string>(),
            }).ToList();
        }


        /// <summary>
        /// 发送消息，有异常会跳过
        /// </summary>
        /// <param name="userIDs"></param>
        /// <param name="tempID"></param>
        /// <param name="data"></param>
        /// <param name="url"></param>
        public void SendTempMessage(IEnumerable<string> userIDs, string tempID, object data, string url)
        {
            var accessToken = GetAccessToken();

            foreach (var openID in userIDs)
            {
                var obj = new
                {
                    touser = openID,
                    template_id = tempID,
                    url = url,
                    data = data
                };
                var api = new CommonApi.BaseApi($"https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={accessToken}", "POST", obj);
                try
                {
                    var r2 = api.CreateRequestReturnJson();
                    Comm.WriteLog("wechat", $"OpenID:{openID},Result:{JsonConvert.SerializeObject(r2)}", DebugLogLevel.Normal);
                }
                catch (Exception ex)
                {
                    Comm.WriteLog("wechat", $"OpenID:{openID},Error:{ex.Message}", DebugLogLevel.Error);
                }

            }
        }



        /// <summary>
        /// 获取关注用户的OpenID
        /// </summary>
        /// <param name="next">第一个拉取的OPENID，不填默认从头开始拉取</param>
        /// <returns></returns>
        public OpenIDList GetAllFollowerOpenIDs(string next = null)
        {
            var accessToken = GetAccessToken();
            var apiGetOpenIDs = new CommonApi.BaseApi($"https://api.weixin.qq.com/cgi-bin/user/get?access_token={accessToken}&next_openid={next}", "GET");
            var result = apiGetOpenIDs.CreateRequestReturnJson();
            var count = result["count"].Value<int>();
            return new OpenIDList
            {
                Count = count,
                Next = result["next_openid"].Value<string>(),
                OpenIDs = count > 0 ? result["data"]["openid"].Values<string>().ToArray() : new string[0],
                Total = result["total"].Value<int>(),
            };
        }


        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string UploadFile(HttpPostedFileBase file)
        {
            var accessToken = GetAccessToken();
            string url = $"https://api.weixin.qq.com/cgi-bin/material/add_material?access_token={accessToken}&type=image";
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            CookieContainer cookieContainer = new CookieContainer();
            request.CookieContainer = cookieContainer;
            request.AllowAutoRedirect = true;
            request.Method = "POST";
            string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线
            request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;
            byte[] itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");

            string fileName = file.FileName;
            //请求头部信息
            MemoryStream ms = new MemoryStream();
            file.InputStream.CopyTo(ms);
            byte[] bf = ms.ToArray();
            StringBuilder sbHeader = new StringBuilder(string.Format("Content-Disposition:form-data;name=\"media\";filename=\"{0}\"\r\nContent-Type:application/octet-stream\r\n\r\n", fileName));
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sbHeader.ToString());
            Stream postStream = request.GetRequestStream();
            postStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
            postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
            postStream.Write(bf, 0, bf.Length);
            postStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            postStream.Close();
            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream instream = response.GetResponseStream();
            using (StreamReader sr = new StreamReader(instream, Encoding.UTF8))
            {
                string content = sr.ReadToEnd();
                var json = JsonConvert.DeserializeObject<JObject>(content);
                return json["media_id"].Value<string>();
            }

        }

    }



    public class UserInfoResult
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string HeadImgUrl { get; set; }

        /// <summary>
        /// 统一用户ID
        /// </summary>
        public string UnionID { get; set; }

        /// <summary>
        /// 加密的用户信息
        /// </summary>
        public string EncryptedData { get; set; }


        public string IV { get; set; }

        public string OpenID { get; set; }

        /// <summary>
        /// 是否已经关注
        /// </summary>
        public bool IsSubscribe { get; set; }
    }

    public class AccessTokenResult
    {
        public string OpenID { get; set; }

        public string AccessToken { get; set; }

        public string UnionID { get; set; }

        public string RefreshToken { get; set; }
    }

    public interface IMessage
    {
        int ID { get; set; }

        string Key { get; set; }


        string Event { get; set; }

        DateTime CreateDateTime { get; set; }


        int Sort { get; set; }

        MessageType Type { get; }
    }

    public class NewsMessage : IMessage
    {
        public virtual int ID { get; set; }

        public virtual string Key { get; set; }

        [Display(Name = "事件")]
        public virtual string Event { get; set; }



        [Required]
        [Display(Name = "标题")]
        public virtual string Title { get; set; }


        [Required]
        [Display(Name = "图片")]
        public virtual string PicUrl { get; set; }


        [Required]
        [Display(Name = "连接")]
        public virtual string Url { get; set; }

        [Display(Name = "描述")]
        [DataType(DataType.MultilineText)]
        public virtual string Description { get; set; }


        [Required]
        [Display(Name = "时间")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        public virtual DateTime CreateDateTime { get; set; }

        [Required]
        [Display(Name = "排序")]
        public virtual int Sort { get; set; }

        [Display(Name = "类型")]
        public MessageType Type { get { return MessageType.News; } }
    }


    public class AccessToken
    {
        public string Code { get; set; }

        public DateTime End { get; set; }
    }


    public class OpenIDList
    {
        public int Count { get; set; }

        public int Total { get; set; }

        public string[] OpenIDs { get; set; }

        public string Next { get; set; }
    }

    public class TempMessage
    {
        public virtual string ID { get; set; }

        public virtual string Title { get; set; }

        public virtual string PrimaryID { get; set; }


        public virtual string DeputyID { get; set; }


        public virtual string Content { get; set; }

    }

    public class ImageMessage : IMessage
    {
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        [Display(Name = "创建时间")]
        public DateTime CreateDateTime { get; set; }

        [Display(Name = "事件")]
        public string Event { get; set; }

        public int ID { get; set; }

        public string Key { get; set; }

        [Display(Name = "排序")]
        public int Sort { get; set; }

        [Display(Name = "媒体ID")]
        public string MediaID { get; set; }

        [Display(Name = "类型")]
        public MessageType Type { get { return MessageType.Image; } }
    }

    public enum MessageType
    {
        Image,
        News
    }
}