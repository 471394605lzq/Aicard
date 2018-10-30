using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
namespace AiCard.Controllers
{
    public class ArticleController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        [AllowCrossSiteJson]
        public ActionResult Index(int cardID, string userID, int type = 0, int page = 1, int pageSize = 20, DateTime? time = null)
        {
            var card = db.Cards.Where(s => s.Enable).FirstOrDefault(s => s.ID == cardID);
            if (card == null)
            {
                return Json(Comm.ToJsonResult("CardNoFound", "名片不存在"));
            }

            if (page != 1 && !time.HasValue)
            {
                return Json(Comm.ToJsonResult("Error", "Page!=1的时候time参数必须传"));
            }
            var filter = from a in db.Articles
                         join e in db.Enterprises
                            on a.EnterpriseID equals e.ID into ae
                         join u in (from u in db.Users
                                    from c in db.Cards
                                    where u.Id == c.UserID
                                    select new { c.Position, c.Avatar, c.UserID, c.Name })
                            on a.UserID equals u.UserID into au
                         join ll in (from l in db.UserLogs
                                     from u in db.Users.Select(s => new { s.Id, s.NickName, s.Avatar })
                                     where l.UserID == u.Id && l.Type == Enums.UserLogType.ArticleLike
                                     orderby l.CreateDateTime descending
                                     select new { l.RelationID, l.UserID, u.NickName, u.Avatar })
                            on a.ID equals ll.RelationID into all
                         join lu in db.UserLogs.Where(s => s.Type == Enums.UserLogType.ArticleLike && s.UserID == userID)
                            on a.ID equals lu.RelationID into alu
                         where a.State == Enums.ArticleState.Released
                         select new
                         {
                             a.ID,
                             a.Images,
                             a.Like,
                             a.Title,
                             a.Video,
                             a.Comment,
                             a.Content,
                             a.UpdateDateTime,
                             a.EnterpriseID,
                             a.UserID,
                             a.Type,
                             a.Share,
                             User = au.FirstOrDefault(),
                             Enterprise = ae.FirstOrDefault(),
                             Liker = all.Take(6),
                             HadLike = alu.Any()
                         };

            switch (type)
            {
                case 0:
                    {
                        filter = filter.Where(s => s.UserID == card.UserID);
                    }
                    break;
                default:
                    {
                        filter = filter.Where(s => s.UserID != card.UserID
                            && s.EnterpriseID == card.EnterpriseID);
                    }
                    break;
            }


            if (time.HasValue)
            {
                if (page > 1)
                {
                    filter = filter.Where(s => s.UpdateDateTime < time);
                }
                else
                {
                    filter = filter.Where(s => s.UpdateDateTime > time);
                }
            }
            //如果Page小于1时候（往上拉更新的时候）
            if (page < 1)
            {
                page = 1;
                pageSize = 100;
            }
            var paged = filter
                .OrderByDescending(s => s.UpdateDateTime)
                .ToPagedList(page, pageSize);
            var data = paged.Select(s =>
            {
                var a = new ArticleIndexViewModels
                {
                    ArticleID = s.ID,
                    Avatar = s.User.Avatar,
                    CommentCount = s.Comment.ToStrForm(4, "评论"),
                    Content = s.Content,
                    Cover = s.Type == Enums.ArticleType.Html ? s.Images : null,
                    DateTime = s.UpdateDateTime.ToString(),
                    DateTimeStr = s.UpdateDateTime.ToStrForm(),
                    HadLike = s.HadLike,
                    Images = s.Type == Enums.ArticleType.Text ?
                        (s.Images.SplitToArray<string>()?.ToArray() ?? new string[0])
                        : new string[0],
                    LikeCount = s.Like.ToStrForm(4, "点赞"),
                    LikeUser = s.Liker.Select(x => x.Avatar).ToArray(),
                    Position = s.User.Position,
                    ShareCount = s.Share.ToStrForm(4, "分享"),
                    Title = s.Title,
                    Type = s.Type,
                    UserName = s.User.Name,
                };
                return a;
            });
            return Json(Comm.ToJsonResultForPagedList(paged, data), JsonRequestBehavior.AllowGet);

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