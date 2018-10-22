using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
namespace AiCard.Controllers
{
    public class UserLogController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [AllowCrossSiteJson]
        public ActionResult CardLikeList(int cardID, int page = 1, int pageSize = 20)
        {
            var likes = Bll.UserLogs.Search(relationID: cardID, type: Enums.UserLogType.CardLike, page: page, pageSize: pageSize);
            var data = likes.Select(s => new { s.UserAvatar, s.UserNickName });
            return Json(Comm.ToJsonResultForPagedList(likes, data), JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult Create(UserLog log)
        {
            if (!db.Users.Any(s => s.Id == log.UserID))
            {
                return Json(Comm.ToJsonResult("UserNoFound", "用户不存在"));
            }
            log.CreateDateTime = DateTime.Now;
            switch (log.Type)
            {
                case Enums.UserLogType.ArticleLike:
                case Enums.UserLogType.ArticleComment:
                case Enums.UserLogType.ArticleRead:
                    {
                        var a = db.Articles.FirstOrDefault(s => s.ID == log.RelationID);
                        if (a == null || a.State != Enums.ArticleState.Released)
                        {
                            return Json(Comm.ToJsonResult("ArticleNoFound", "文章不存在"));
                        }
                        log.TargetEnterpriseID = a.EnterpriseID;
                        log.TargetUserID = a.UserID;
                    }
                    break;
                case Enums.UserLogType.ProductRead:
                case Enums.UserLogType.ProductCon:
                    {
                        var p = db.Products.FirstOrDefault(s => s.ID == log.RelationID);
                        if (p == null)
                        {
                            return Json(Comm.ToJsonResult("ProductNoFound", "商品不存在"));
                        }
                        log.TargetEnterpriseID = p.EnterpriseID;
                    }
                    break;
                case Enums.UserLogType.Communication:
                case Enums.UserLogType.HomePageRead:
                case Enums.UserLogType.ShopRead:
                case Enums.UserLogType.WeChatOpen:
                case Enums.UserLogType.CardRead:
                case Enums.UserLogType.CardForward:
                case Enums.UserLogType.CardSave:
                case Enums.UserLogType.CardLike:
                case Enums.UserLogType.PhoneCall:
                case Enums.UserLogType.EmailSend:
                case Enums.UserLogType.VoicePlay:
                case Enums.UserLogType.VideoPlay:
                    {
                        var c = db.Cards.FirstOrDefault(s => s.ID == log.RelationID);
                        if (c == null || !c.Enable)
                        {
                            return Json(Comm.ToJsonResult("CardNoFound", "卡片不存在"));
                        }
                        log.TargetEnterpriseID = c.EnterpriseID;
                        log.TargetUserID = c.UserID;
                    }
                    break;
                default:
                    break;
            }
            //如果已经存在点赞记录，就取消点赞
            if (log.Type == Enums.UserLogType.CardLike || log.Type == Enums.UserLogType.ArticleLike)
            {
                var dbLog = db.UserLogs.FirstOrDefault(s => s.RelationID == log.RelationID && s.UserID == log.UserID && s.Type == log.Type);
                if (dbLog == null)
                {
                    db.UserLogs.Add(log);
                }
                else
                {
                    db.UserLogs.Remove(dbLog);
                }
            }
            else
            {
                db.UserLogs.Add(log);
            }
            db.SaveChanges();
            return Json(Comm.ToJsonResult("Success", "添加成功"));
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