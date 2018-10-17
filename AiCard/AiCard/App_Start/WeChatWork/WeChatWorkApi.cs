using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace AiCard.WeChatWork
{
    public class WeChatWorkApi
    {
        public string AccessToken { get; set; }

        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <param name="corpid">公司ID 我的企业→企业信息→企业ID</param>
        /// <param name="secret">通讯录Secret 管理工具→通讯录同步→Secret</param>
        public void GetAccessToken(string corpid, string secret)
        {
            var api = new Api.BaseApi($"https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={corpid}&corpsecret={secret}", "get");
            var result = api.CreateRequestReturnJson();
            AccessToken = result["access_token"].Value<string>();
        }


        public List<Models.Department> GetDepartment(int? pid = null)
        {
            var api = new Api.BaseApi($"https://qyapi.weixin.qq.com/cgi-bin/department/list?access_token={AccessToken}&id={pid}", "get");

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

            var api = new Api.BaseApi($"https://qyapi.weixin.qq.com/cgi-bin/user/list?access_token={AccessToken}&department_id={id}&fetch_child=0", "get");
            var result = api.CreateRequestReturnJson();
            return result["userlist"].Value<JArray>()
                .Select(s =>
                {
                    ;
                    return new Models.User
                    {
                        Avatar = s["avatar"].Value<string>(),
                        Email = s["email"].Value<string>(),
                        Gender = (Enums.Gender)Enum.Parse(typeof(Enums.Gender), s["gender"].Value<string>()),
                        ID = s["userid"].Value<string>(),
                        Mobile = s["mobile"].Value<string>(),
                        Name = s["name"].Value<string>(),
                        Position = s["position"].Value<string>(),
                        Telephone = s["telephone"].Value<string>()
                    };
                }).ToList();
        }

    }
}