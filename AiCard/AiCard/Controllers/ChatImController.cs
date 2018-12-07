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
            //新增客户
            var addcustresult = AddUserCustomer(fromUserID, cardID);
            return Json(Comm.ToJsonResult("Success", "成功", new
            {
                From = new
                {
                    Avatar = from.Avatar.SplitToArray<string>(',')[0],
                    UserID = from.Id,
                    UserName = from.UserName,
                    NickName = from.NickName,
                    Sign = Common.TxIm.SigCheck.Sign(from.UserName),
                },
                To = new
                {
                    Avatar = to.Avatar.SplitToArray<string>(',')[0],
                    UserID = to.Id,
                    UserName = to.UserName,
                    NickName = to.NickName,
                }
            }), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 新增名片归属用户下的客户信息
        /// </summary>
        /// <param name="custuserid"></param>
        /// <param name="ownerusercardid"></param>
        /// <returns></returns>
        private RunterResult AddUserCustomer(string custuserid, int ownerusercardid)
        {
            var from = db.Users.FirstOrDefault(s => s.Id == custuserid);
            //根据客户对应的userid获取企业客户信息
            var cust = db.EnterpriseCustomers.FirstOrDefault(s=>s.UserID==custuserid);
            var to = (from u in db.Users
                      from c in db.Cards
                      where u.Id == c.UserID && c.ID == ownerusercardid
                      select new { u.Id, u.UserName, c.Avatar, NickName = c.Name }).FirstOrDefault();
            if (to == null)
            {
                return new RunterResult { IsSuccess = false, Message = "名片不存在" };
            }
            if (from == null)
            {
                return new RunterResult { IsSuccess = false, Message = "用户不存在" };
            }
            else
            {
                //如果不存在企业客户则新增
                if (cust == null)
                {
                    //保存企业客户信息
                    EnterpriseCustomer ecust = new EnterpriseCustomer();
                    ecust.UserID = custuserid;
                    ecust.EnterpriseID = from.EnterpriseID;
                    ecust.RealName = from.NickName;
                    ecust.Mobile = from.PhoneNumber;
                    db.EnterpriseCustomers.Add(ecust);
                    int resultrow = db.SaveChanges();
                    if (resultrow > 0)
                    {
                        //保存企业名片用户下的客户信息
                        EnterpriseUserCustomer euscust = new EnterpriseUserCustomer();
                        euscust.CustomerID = ecust.ID;
                        euscust.OwnerID = to.Id;
                        euscust.State = Common.Enums.EnterpriseUserCustomerState.NoFllow;
                        euscust.Source = Common.Enums.EnterpriseUserCustomerSource.CardList;
                        db.EnterpriseUserCustomer.Add(euscust);
                        db.SaveChanges();
                    }
                }
                else
                {
                    //如果存在就检测归宿人客户是否存在
                    var us = db.EnterpriseUserCustomer.FirstOrDefault(s => s.OwnerID == to.Id && s.CustomerID == cust.ID);
                    if (us == null)
                    {
                        //保存企业名片用户下的客户信息
                        EnterpriseUserCustomer euscust = new EnterpriseUserCustomer();
                        euscust.CustomerID = cust.ID;
                        euscust.OwnerID = to.Id;
                        euscust.State = Common.Enums.EnterpriseUserCustomerState.NoFllow;
                        euscust.Source = Common.Enums.EnterpriseUserCustomerSource.CardList;
                        db.EnterpriseUserCustomer.Add(euscust);
                        db.SaveChanges();
                    }
                }
            }
            return new RunterResult { IsSuccess = true, Message = "新增成功" };
        }
        public class RunterResult
        {
            public bool IsSuccess { get; set; }
            public string Message { get; set; }
        }
        /// <summary>
        /// 名片用户(fromUserID)跟普通用户(ToUserID)聊天用到的用户信息
        /// </summary>
        /// <param name="fromUserID">请求消息的用户ID</param>
        /// <param name="cardID">名片的用户ID</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult GetFromAndToUserID(string fromUserName, string toUserName)
        {
            var from = (from u in db.Users
                        from c in db.Cards
                        where u.Id == c.UserID && u.UserName == fromUserName
                        select new { u.Id, u.UserName, c.Avatar, NickName = c.Name }).FirstOrDefault();
            if (from == null)
            {
                return Json(Comm.ToJsonResult("FromUserNoFound", "发送用户不存在"));
            }
            var api = new Common.TxIm.ImApi();

            var to = db.Users.FirstOrDefault(s => s.UserName == toUserName);
            //if (to == null)
            //{
            //    return Json(Comm.ToJsonResult("CardNoFound", "接收消息用户不存在"));
            //}
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
                    Avatar = from.Avatar.SplitToArray<string>(',')[0],
                    UserID = from.Id,
                    UserName = from.UserName,
                    NickName = from.NickName,
                    Sign = Common.TxIm.SigCheck.Sign(from.UserName),
                },
                To = new
                {
                    Avatar = to == null ? "" : to.Avatar,
                    UserID = to == null ? "" : to.Id,
                    UserName = to == null ? "" : to.UserName,
                    NickName = to == null ? "" : to.NickName,
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
            var e = db.Enterprises.FirstOrDefault(s => s.ID == toUser.EnterpriseID);
            string openID = option.SearchOpenID(appID);
            var fromUser = db.Users.FirstOrDefault(s => s.Id == fromUserID);
            var temp = new Common.WeChat.WeChatMessageTemp.EDefaultNotifyWeChatMessage(e.Name, fromUser.NickName, content, DateTime.Now);
            var result = new Common.WeChat.WeChatMinApi(config)
                    .SendMessage(openID, formID, null, temp);

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