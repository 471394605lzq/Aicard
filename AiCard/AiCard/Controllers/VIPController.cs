using AiCard.Bll;
using AiCard.Commom.WeChatPay;
using AiCard.Common;
using AiCard.Common.WeChat;
using AiCard.DAL.Models;
using AiCard.Models.Vip;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WxPayAPI;
using Newtonsoft.Json.Linq;
using System.Data.Entity.Infrastructure;
using PagedList;

namespace AiCard.Controllers
{
    /// <summary>
    /// author:lym
    /// date:2018-11-16
    /// VIP会员控制器
    /// </summary>
    public class VIPController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        OrderBLL orderbll = new OrderBLL();

        /// <summary>
        /// 注册VIP
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="iv">手机号</param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult CreateByWeChatPhone(string userID, string openID, string iv, string encryptedData, string code)
        {

            string mobile = null;
            var session = Common.WeChat.Jscode2sessionResultList.GetSession(openID);
            try
            {
                //从EncryptedData从解密用户数据
                var str = Common.WeChat.Jscode2sessionResultList.AESDecrypt(encryptedData, session, iv);
                var jObj = JsonConvert.DeserializeObject<JToken>(str);
                mobile = jObj["purePhoneNumber"].Value<string>();
            }
            catch (Exception)
            {
                Comm.WriteLog("CreateByWeChatPhoneDecrypt", JsonConvert.SerializeObject(new { encryptedData, session, iv }), Common.Enums.DebugLogLevel.Error);
                return Json(Comm.ToJsonResult("Decrypt Fail", "解密失败"));
            }

            if (db.Users.Any(s => s.PhoneNumber == mobile))
            {
                return Json(Comm.ToJsonResult("MobileHadUsed", "手机号已被使用"));
            }

            //if (!Reg.IsMobile(mobile))
            //{
            //    return Json(Comm.ToJsonResult("Moblie Error", "手机号不正确"));
            //}
            var user = db.Users.FirstOrDefault(s => s.Id == userID);
           
            if (user == null)
            {
                return Json(Comm.ToJsonResult("UserNoFound", "用户不存在"));
            }
            if (db.CardPersonals.Any(s => s.UserID == userID))
            {
                return Json(Comm.ToJsonResult("CardPersonalHadCreate", "该用户已经个人名片已存在"));
            }
            Vip parentVip = null;
            if (!string.IsNullOrWhiteSpace(code))
            {
                //判断是否邀请码是否存在
                parentVip = db.Vips.FirstOrDefault(s => s.State == Common.Enums.VipState.Enable && s.Code == code);
                if (parentVip == null)
                {
                    return Json(Comm.ToJsonResult("CodeNoFound", "验证码不存在"));
                }
            }
            //保存用户手机号到用户表
            user.PhoneNumber = mobile;
            //把名片已知信息填到个人名片
            var card = new CardPersonal
            {
                UserID = userID,
                Avatar = user.Avatar,
                Enable = true,
                Gender = Common.Enums.Gender.NoSet,
                Name = user.NickName,
                Mobile = mobile
            };
            db.CardPersonals.Add(card);
            //db.SaveChanges();
            if (parentVip != null)
            {
                //建立关系并给上级+3块
            }
            return Json(Comm.ToJsonResult("Success", "成功", new { PCardID = card.ID }));


        }

        /// <summary>
        /// 获取VIP会员名片分页列表
        /// </summary>
        /// <param name="sReqParameter">请求的参数</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult Index(string sReqParameter = "")
        {
            #region
            ReqVipCardList reqParam = JsonConvert.DeserializeObject<ReqVipCardList>(sReqParameter);
            if (reqParam == null)
            {
                reqParam = new Models.Vip.ReqVipCardList()
                {
                    filter = string.Empty,
                    Page = 1,
                    PageSize = 20
                };
            }
            string selectStr = string.Empty;
            try
            {
                #region
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    string sw = string.Empty;
                    if (!string.IsNullOrWhiteSpace(reqParam.filter))
                    {
                        sw = $" and (t2.Mobile like '%{reqParam.filter}%' or t3.UserName like '%{reqParam.filter}%') ";
                    }
                    selectStr = $@"select t1.ID as VipID,t1.Amount,t1.TotalAmount,t1.VipChild2ndCount,t1.VipChild3rdCount,t1.FreeChildCount,
                                    t1.[State] ,t2.Name,t2.Avatar,t2.Mobile,t2.Gender,t3.UserName as UserName
                                    from Vips t1 
                                    left join CardPersonals t2 on t1.CardID=t2.ID
                                    left join AspNetUsers t3 on t1.UserID=t3.ID
                                    where t1.[Type]={(int)Common.Enums.VipRank.Vip99}  {sw}
                                    order by t1.CreateDateTime desc";

                    var query = db.Database.SqlQuery<VipCardList>(selectStr);
                    var paged = query.ToPagedList(reqParam.Page, reqParam.PageSize);
                    return Json(Comm.ToJsonResultForPagedList(paged, paged), JsonRequestBehavior.AllowGet);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Comm.WriteLog("VIPCotroller.Index", ex.Message, Common.Enums.DebugLogLevel.Error, ex: ex);
                return Json(Comm.ToJsonResult("Error", "调用获取vip用户列表接口发生异常"), JsonRequestBehavior.AllowGet);
            }
            //return Json(Comm.ToJsonResult("Success", "成功", ""), JsonRequestBehavior.AllowGet);
            #endregion
        }

        /// <summary>
        /// 升级VIP，创建订单及预调起支付
        /// </summary>
        /// <param name="code">微信小程序登录返回的code</param>
        /// /// <param name="UserID">用户ID</param>
        /// <returns></returns>
        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult UpGradeVIP(string code, string UserID)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return Json(Comm.ToJsonResult("Error", "code参数不能为空"), JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(UserID))
            {
                return Json(Comm.ToJsonResult("Error", "用户ID不能为空"), JsonRequestBehavior.AllowGet);
            }
            dynamic result = null;
            try
            {
                result = orderbll.CreateUpGradeOrder(code, UserID);
            }
            catch (Exception ex)
            {
                Comm.WriteLog("WeChatPay", ex.Message, Common.Enums.DebugLogLevel.Error, ex: ex);
                return Json(Comm.ToJsonResult("Error", "调用升级接口发生异常"), JsonRequestBehavior.AllowGet);

            }

            return Json(Comm.ToJsonResult(result.retCode, result.retMsg, result.objectData), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 退款测试
        /// </summary>
        /// <param name="code"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult Refund(string code, string UserID)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return Json(Comm.ToJsonResult("Error", "code参数不能为空"), JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(UserID))
            {
                return Json(Comm.ToJsonResult("Error", "用户ID不能为空"), JsonRequestBehavior.AllowGet);
            }
            dynamic result = null;
            try
            {
                result = orderbll.RefundApply(code, UserID);
            }
            catch (Exception ex)
            {
                Comm.WriteLog("Refund", ex.Message, Common.Enums.DebugLogLevel.Error, ex: ex);
                return Json(Comm.ToJsonResult("Error", "调用退款接口发生异常"), JsonRequestBehavior.AllowGet);

            }

            return Json(Comm.ToJsonResult(result.retCode, result.retMsg, result.objectData), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 生成海报
        /// </summary>
        /// <param name="pCardID"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult GetPoster(int pCardID)
        {

            //try
            //{
            //    var card = (from c in db.CardPersonals
            //                where c.ID == pCardID
            //                select c).FirstOrDefault();
            //    if (card == null)
            //    {
            //        return Json(Comm.ToJsonResult("Error", "该名片不存在"), JsonRequestBehavior.AllowGet);
            //    }
            //    var qe = (from e in db.Enterprises
            //              where e.ID == card.EnterpriseID
            //              select e).FirstOrDefault();

            //    var cardm = (from ct in db.CardTabs
            //                 where ct.CardID == pCardID
            //                 orderby ct.Count descending
            //                 select new
            //                 {
            //                     Name = ct.Name,
            //                     Count = ct.Count,
            //                     Style = ct.Style
            //                 }).Take(2).ToList();
            //    List<TagModel> listst = new List<TagModel>();
            //    if (cardm.Count > 0)
            //    {
            //        for (int i = 0; i < cardm.Count; i++)
            //        {
            //            if (i == 0)
            //            {
            //                TagModel tm = new TagModel();
            //                tm.TagName = cardm[i].Name + " " + cardm[i].Count.ToStrForm(4, "");
            //                tm.TagStyle = cardm[i].Style;
            //                listst.Add(tm);
            //            }
            //        }
            //    }
            //    var dm = new DrawingPictureModel
            //    {
            //        AvatarPath = string.IsNullOrWhiteSpace(card.Avatar) ? "" : DrawingPictures.DownloadImg(card.Avatar.SplitToArray<string>(',')[0], $"avatar_{pCardID}.png", 834, 834),
            //        CompanyName = qe.Name,
            //        LogoPath = string.IsNullOrWhiteSpace(qe.Logo) ? "" : DrawingPictures.DownloadImg(qe.Logo, $"logo_{pCardID}.png", 96, 96),
            //        Position = card.Position,
            //        QrPath = string.IsNullOrWhiteSpace(card.WeChatMiniQrCode) ? "" : DrawingPictures.DownloadImg(card.WeChatMiniQrCode, $"qrcode_{pCardID}.png", 240, 240),
            //        Remark = card.Remark,
            //        UserName = card.Name,
            //        TagList = listst,
            //        PosterImageName = "cardid_" + pCardID.ToString()
            //    };
            //    //调用生成海报方法
            //    string returnpath = Comm.MergePosterImage(dm);
            //    var data = new
            //    {
            //        Posterpath = Comm.ResizeImage(returnpath)
            //    };
            //    var card = db.Cards.FirstOrDefault(s => s.ID == pCardID);
            //    card.Poster = returnpath;
            //    db.SaveChanges();
            //    return Json(Comm.ToJsonResult("Success", "成功", data), JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception ex)
            //{
            //    return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
            //}
            return View();
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