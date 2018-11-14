using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
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
            var api = new TxIm.Api();

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
                    Sign = TxIm.SigCheck.Sign(from.UserName),
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