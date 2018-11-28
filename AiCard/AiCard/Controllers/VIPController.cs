using AiCard.Bll;
using AiCard.Common;
using AiCard.Common.WeChat;
using AiCard.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

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

        #region 接口
        /// <summary>
        /// 注册VIP
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="iv">手机号</param>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult CreateByWeChatPhone(string userID, string iv, string encryptedData, string code)
        {
            var user = db.Users.FirstOrDefault(s => s.Id == userID);

            if (user == null)
            {
                return Json(Comm.ToJsonResult("UserNoFound", "用户不存在"));
            }
            if (db.CardPersonals.Any(s => s.UserID == userID))
            {
                return Json(Comm.ToJsonResult("CardPersonalHadCreate", "该用户已经个人名片已存在"));
            }
            //把数据中的OpenID取出
            var userOpenIDs = new Bll.Users.UserOpenID(user);
            IConfig config = new ConfigMiniPersonal();
            var openID = userOpenIDs.SearchOpenID(config.AppID);
            if (openID == null)
            {
                return Json(Comm.ToJsonResult("OpenIDIsNull", "OpenID不存在"));
            }
            string session = null;
            try
            {
                session = Jscode2sessionResultList.GetSession(openID);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("GetSessionFail", ex.Message));
            }

            string mobile = null;
            try
            {
                mobile = Jscode2sessionResultList.AESDecryptPhoneNumber(encryptedData, session, iv);
            }
            catch (Exception)
            {
                return Json(Comm.ToJsonResult("DecryptFail", "解密失败，SessionKey过期，需要重新调用登录接口"));
            }

            if (db.Users.Any(s => s.PhoneNumber == mobile))
            {
                return Json(Comm.ToJsonResult("MobileHadUsed", "手机号已被使用"));
            }

            //if (!Reg.IsMobile(mobile))
            //{
            //    return Json(Comm.ToJsonResult("Moblie Error", "手机号不正确"));
            //}


            Vip parentVip = null;
            if (!string.IsNullOrWhiteSpace(code))
            {
                //判断是否邀请码是否存在
                parentVip = db.Vips.FirstOrDefault(s => s.State == Common.Enums.VipState.Enable && s.Code == code);
                if (parentVip == null)
                {
                    return Json(Comm.ToJsonResult("CodeNoFound", "邀请码不存在"));
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
            db.SaveChanges();
            try
            {
                card.WeChatMiniQrCode = GetWeChatQrCode(card.ID);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Comm.WriteLog(this.GetType().ToString(), ex.Message, Common.Enums.DebugLogLevel.Error);
            }
            if (parentVip != null)
            {
                var result = new Bll.VipBLL().CreateVipRelation(userID, code);
                if (result.retCode == Comm.ReqResultCode.failed)
                {
                    //回滚
                    db.CardPersonals.Remove(card);
                    db.SaveChanges();
                    return Json(Comm.ToJsonResult("Error", result.retMsg));
                }
            }
            return Json(Comm.ToJsonResult("Success", "成功", new { PCardID = card.ID }));


        }



        /// <summary>
        /// 升级VIP，创建订单及预调起支付
        /// </summary>
        /// <param name="code">微信小程序登录返回的code</param>
        /// /// <param name="userID">用户ID</param>
        /// <returns></returns>
        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult UpGradeVIP(string code, string userID)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return Json(Comm.ToJsonResult("Error", "code参数不能为空"));
            }
            if (string.IsNullOrWhiteSpace(userID))
            {
                return Json(Comm.ToJsonResult("Error", "用户ID不能为空"));
            }
            var vip = db.Vips.FirstOrDefault(s => s.UserID == userID);
            if (vip == null)
            {
                return Json(Comm.ToJsonResult("NoVip", "没有注册会员"));
            }
            else if (vip.Type != Common.Enums.VipRank.Default)
            {
                return Json(Comm.ToJsonResult("VipHadUp", "已升级"));
            }
            else if (vip.State == Common.Enums.VipState.Uploading)
            {
                return Json(Comm.ToJsonResult("VipUploading", "升级中"));
            }
            dynamic result = null;
            try
            {
                result = orderbll.CreateUpGradeOrder(code, userID);
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


        public ActionResult GetVipInfo(string userID, int? vipID)
        {

            var query = from u in db.Users
                        from v in db.Vips
                        where u.Id == v.UserID
                        select new
                        {
                            UserID = u.Id,
                            u.Avatar,
                            u.NickName,
                            VipID = v.ID,
                            v.State,
                            v.Type,
                            PCardID = v.CardID,
                            v.FreeChildCount,
                            v.VipChild2ndCount,
                            v.VipChild3rdCount,
                            v.TotalAmount,
                            v.TotalAmountRank,
                            v.TotalMonthAmountRank,
                            v.TotalWeekAmountRank,
                            v.CreateDateTime,
                            v.CardID,
                            v.Code
                        };
            if (vipID.HasValue)
            {
                query = query.Where(s => s.VipID == vipID);
            }
            else
            {
                query = query.Where(s => s.UserID == userID);
            }
            var vip = query.FirstOrDefault();
            if (vip == null)
            {
                return Json(Comm.ToJsonResult("VipNoFound", "未注册"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Comm.ToJsonResult("Success", "成功", new
                {
                    vip.VipID,
                    vip.State,
                    vip.Type,
                    PCardID = vip.CardID,
                    vip.FreeChildCount,
                    vip.VipChild2ndCount,
                    vip.VipChild3rdCount,
                    vip.TotalAmount,
                    vip.TotalAmountRank,
                    vip.TotalMonthAmountRank,
                    vip.TotalWeekAmountRank,
                    CreateDateTime = vip.CreateDateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    vip.UserID,
                    vip.Code
                }), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult ChangeVip(string userID, Common.Enums.VipState state)
        {
            var vip = db.Vips.FirstOrDefault(s => s.UserID == userID);
            if (vip == null)
            {
                return Json(Comm.ToJsonResult("VipNoFound", "VIP不存在"));
            }
            vip.State = state;
            db.SaveChanges();
            return Json(Comm.ToJsonResult("Success", "成功"));
        }
        #endregion

        public string GetWeChatQrCode(int pCardID)
        {
            Common.WeChat.IConfig config = new Common.WeChat.ConfigMiniPersonal();
            var api = new Common.WeChat.WeChatMinApi(config);
            var p = new Dictionary<string, string>();
            p.Add("PCardID", pCardID.ToString());
            return api.GetWXACodeUnlimit(Common.WeChat.WeChatPagePersonal.CardDetail, p);
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