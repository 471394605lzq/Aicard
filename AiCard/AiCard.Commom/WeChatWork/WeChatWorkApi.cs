using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AiCard.Common.Enums;
using AiCard.Common.WeChatWork.Models;
using AiCard.Common.Extensions;

namespace AiCard.Common.WeChatWork
{
    public class WeChatWorkApi
    {
        public string AccessToken { get; set; }

        public WeChatWorkApi()
        {

        }

        public WeChatWorkApi(string corpid, string secret)
        {
            GetAccessToken(corpid, secret);
        }


        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <param name="corpid">公司ID 我的企业→企业信息→企业ID</param>
        /// <param name="secret">通讯录Secret 管理工具→通讯录同步→Secret</param>
        public void GetAccessToken(string corpid, string secret)
        {
            var api = new CommonApi.BaseApi($"https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={corpid}&corpsecret={secret}", "get");
            var result = api.CreateRequestReturnJson();
            AccessToken = result["access_token"].Value<string>();
        }


        public List<Department> GetDepartment(int? pid = null)
        {
            var p = new Dictionary<string, object>();
            p.Add("access_token", AccessToken);
            p.Add("id", pid);
            var api = new CommonApi.BaseApi($"https://qyapi.weixin.qq.com/cgi-bin/department/list{p.ToParam("?")}", "get");

            var result = api.CreateRequestReturnJson();
            var deps = result["department"].Value<JArray>()
                .Select(s => new Models.Department
                {
                    ID = s["id"].Value<int>(),
                    Name = s["name"].Value<string>(),
                    Order = s["order"].Value<int>(),
                    ParentID = s["parentid"].Value<int>()
                }).ToList();
            var tree = new List<Models.Department>();
            Action<Models.Department> setChild = null;
            setChild = pDep =>
            {
                pDep.Child.AddRange(deps.Where(s => s.ParentID == pDep.ID));
                foreach (var child in pDep.Child)
                {
                    setChild(child);
                }
            };
            var topDeps = deps.Where(s => s.ParentID == 0).ToList();
            foreach (var item in topDeps)
            {
                setChild(item);
            }
            return topDeps;
        }

        public List<Models.User> GetUsesByDepID(int id)
        {
            var p = new Dictionary<string, string>();
            p.Add("access_token", AccessToken);
            p.Add("department_id", id.ToString());
            p.Add("fetch_child", "0");
            var api = new CommonApi.BaseApi($"https://qyapi.weixin.qq.com/cgi-bin/user/list{p.ToParam("?")}", "get");
            var result = api.CreateRequestReturnJson();
            return result["userlist"].Value<JArray>()
                .Select(s =>
                {
                    ;
                    return new Models.User
                    {
                        Avatar = s["avatar"].Value<string>(),
                        Email = s["email"].Value<string>(),
                        Gender = (Gender)Enum.Parse(typeof(Gender), s["gender"].Value<string>()),
                        ID = s["userid"].Value<string>(),
                        Mobile = s["mobile"].Value<string>(),
                        Name = s["name"].Value<string>(),
                        Position = s["position"].Value<string>(),
                        Telephone = s["telephone"].Value<string>(),
                        //UnionID = GetUserUnionID(s["userid"].Value<string>())
                    };
                }).ToList();
        }

        /// <summary>
        /// 获取微信用户基本信息，通过openID,Token内嵌
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public string GetUserUnionID(string userid)
        {
            var p0 = new Dictionary<string, object>();
            p0.Add("access_token", AccessToken);
            try
            {
                var api = new CommonApi.BaseApi($"https://qyapi.weixin.qq.com/cgi-bin/user/convert_to_openid{p0.ToParam("?")}", "POST", new { userid = userid });
                var openid = api.CreateRequestReturnJson()["openid"].Value<string>();

                var p = new Dictionary<string, string>();
                p.Add("access_token", AccessToken);
                p.Add("openid", openid);
                p.Add("lang", "zh_CN");
                var result = new CommonApi.BaseApi($"https://api.weixin.qq.com/cgi-bin/user/info{p.ToParam("?")}", "GET")
                 .CreateRequestReturnJson();
                if (result["errcode"] != null)
                {
                    throw new Exception(JsonConvert.SerializeObject(result));
                }
                return result["unionid"].Value<string>();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}