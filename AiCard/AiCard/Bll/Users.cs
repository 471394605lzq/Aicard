using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AiCard.DAL.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace AiCard.Bll.Users
{
    /// <summary>
    /// 处理用户的OpenID
    /// </summary>
    public class UserOpenID
    {
        private ApplicationUser _user;
        private List<AppIDOpenID> _openIDs;

        public UserOpenID(ApplicationUser user)
        {
            _user = user;
            _openIDs = new List<AppIDOpenID>();
            if (!string.IsNullOrWhiteSpace(user.OpenIDs))
            {
                _openIDs = JsonConvert.DeserializeObject<List<AppIDOpenID>>(user.OpenIDs);
            }

        }
        /// <summary>
        /// 查询OpenID
        /// </summary>
        /// <param name="appID"></param>
        /// <returns>找不到的返回null</returns>
        public string SearchOpenID(string appID)
        {
            return _openIDs.FirstOrDefault(s => s.AppID == appID)?.OpenID;
        }

        /// <summary>
        /// 添加OpenID，添加完成后会修改OpenIDs字段
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="openID"></param>
        public void AddOpenID(string appID, string openID)
        {
            if (!_openIDs.Any(s => s.AppID == appID && s.OpenID == openID))
            {
                _openIDs.Add(new AppIDOpenID { AppID = appID, OpenID = openID });

                _user.OpenIDs = JsonConvert.SerializeObject(_openIDs);
            }
        }

        /// <summary>
        /// 返回当前用户存的OpenID
        /// </summary>
        public List<AppIDOpenID> List
        {
            get
            {
                return _openIDs;
            }
        }
    }

    public class AppIDOpenID
    {
        public string AppID { get; set; }

        public string OpenID { get; set; }
    }


}