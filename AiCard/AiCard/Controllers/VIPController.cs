using AiCard.Bll;
using AiCard.Commom.WeChatPay;
using AiCard.Common;
using AiCard.Common.WeChat;
using AiCard.DAL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WxPayAPI;
namespace AiCard.Controllers
{
    public class VIPController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        OrderBLL orderbll = new OrderBLL();

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