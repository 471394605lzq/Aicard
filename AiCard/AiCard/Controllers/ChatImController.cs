using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
using AiCard.DAL.Models;
using AiCard.Common;

namespace AiCard.Controllers
{
    public class ChatImController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// 获取聊天说话用到用户信息
        /// </summary>
        /// <param name="fromUserID">请求消息的用户ID</param>
        /// <param name="cardID">名片的用户ID</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult GetFromAndToByCardID(string fromUserID, int cardID)
        {
            var from = db.Users.FirstOrDefault(s => s.Id == fromUserID);
            if (from == null)
            {
                return Json(Comm.ToJsonResult("FromUserNoFound", "用户不存在"));
            }
            var api = new Common.TxIm.ImApi();

            var to = (from u in db.Users
                      from c in db.Cards
                      where u.Id == c.UserID && c.ID == cardID
                      select new { u.Id, u.UserName, c.Avatar, NickName = c.Name }).FirstOrDefault();
            if (to == null)
            {
                return Json(Comm.ToJsonResult("CardNoFound", "卡片不存在"));
            }
            try
            {
                api.ImportUser(from.UserName, from.NickName, from.Avatar);
                api.ImportUser(to.UserName, to.NickName, to.Avatar);
            }
            catch (Exception ex)
            {
                Json(Comm.ToJsonResult("Error", ex.Message));
            }

            return Json(Comm.ToJsonResult("Success", "成功", new
            {
                From = new
                {
                    Avatar = from.Avatar,
                    UserID = from.Id,
                    UserName = from.UserName,
                    NickName = from.NickName,
                    Sign = Common.TxIm.SigCheck.Sign(from.UserName),
                },
                To = new
                {
                    Avatar = to.Avatar,
                    UserID = to.Id,
                    UserName = to.UserName,
                    NickName = to.NickName,
                }
            }), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="content"></param>
        /// <param name="fromUserID"></param>
        /// <param name="formID"></param>
        /// <param name="toUserID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SendMessage(string content, string fromUserID, string formID, string toUserID)
        {
            Common.WeChat.IConfig config = new Common.WeChat.ConfigMini();
            var appID = config.AppID;
            var toUser = db.Users.FirstOrDefault(s => s.Id == toUserID);
            var option = new Bll.Users.UserOpenID(toUser);
            string openID = option.SearchOpenID(appID);
            string tempID = "szJbdS4HgheYYCoDRy4sWwGZltbdSWYARzK5VYrzh1c";
            var fromUser = db.Users.FirstOrDefault(s => s.Id == fromUserID);
            object key = new
            {
                keyword1 = "AI名片",
                keyword2 = fromUser.NickName,
                keyword3 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                keyword4 = content,
            };
            var result = new Common.WeChat.WeChatMinApi(config)
                    .SendMessage(openID, tempID, formID, null, key);

            return Json(Comm.ToJsonResult("Success", "", result));
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}