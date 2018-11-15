using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
using AiCard.DAL.Models;
using AiCard.Common;
using AiCard.Common.Extensions;

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
                                     where l.UserID == u.Id && l.Type == Common.Enums.UserLogType.ArticleLike
                                     orderby l.CreateDateTime descending
                                     select new { l.RelationID, l.UserID, u.NickName, u.Avatar })
                            on a.ID equals ll.RelationID into all
                         join lu in db.UserLogs.Where(s => s.Type == Common.Enums.UserLogType.ArticleLike && s.UserID == userID)
                            on a.ID equals lu.RelationID into alu
                         where a.State == Common.Enums.ArticleState.Released
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
                if (page > 0)
                {
                    filter = filter.Where(s => s.UpdateDateTime <= time);
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
                    Avatar = s.Type == Common.Enums.ArticleType.Text ? s.User?.Avatar : s.Enterprise.Logo,
                    CommentCount = s.Comment.ToStrForm(4, "评论"),
                    Content = s.Content,
                    Cover = s.Type == Common.Enums.ArticleType.Html ? s.Images : null,
                    DateTime = s.UpdateDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    DateTimeStr = s.UpdateDateTime.ToStrForm(),
                    HadLike = s.HadLike,
                    Images = s.Type == Common.Enums.ArticleType.Text ?
                        (s.Images.SplitToArray<string>()?.ToArray() ?? new string[0])
                        : new string[0],
                    LikeCount = s.Like.ToStrForm(4, "点赞"),
                    LikeUser = s.Liker.Select(x => x.Avatar).ToArray(),
                    Position = s.Type == Common.Enums.ArticleType.Text ? s.User.Position : "",
                    ShareCount = s.Share.ToStrForm(4, "分享"),
                    Title = s.Title,
                    Type = s.Type,
                    UserName = s.Type == Common.Enums.ArticleType.Text ? s.User.Name : s.Enterprise.Name,
                };
                return a;
            });
            return Json(Comm.ToJsonResultForPagedList(paged, data), JsonRequestBehavior.AllowGet);

        }

        [AllowCrossSiteJson]
        public ActionResult Detail(int articleID, string userID, int pageSize = 20)
        {
            var a = db.Articles.FirstOrDefault(s => s.ID == articleID && s.State == Common.Enums.ArticleState.Released);
            if (a == null)
            {
                return Json(Comm.ToJsonResult("NoFound", "动态不存在"), JsonRequestBehavior.AllowGet);
            }
            var c = db.Cards.FirstOrDefault(s => s.UserID == a.UserID);
            var e = db.Enterprises.FirstOrDefault(s => s.ID == a.EnterpriseID);
            var com = db.ArticleComments.Count(s => s.ArticleID == articleID);
            //var com = (from ac in db.ArticleComments
            //           from u in db.Users
            //           where ac.UserID == u.Id && ac.ArticleID == articleID
            //           orderby ac.CreateDateTime descending
            //           select new
            //           {
            //               ac.Content,
            //               ac.CreateDateTime,
            //               ac.ID,
            //               ac.UserID,
            //               u.Avatar,
            //               u.NickName,
            //           })
            //          .ToPagedList(1, pageSize);
            //var likes = (from u in db.Users
            //             from l in db.UserLogs
            //             where u.Id == l.UserID
            //             && l.Type == UserLogType.ArticleLike
            //             && l.RelationID == articleID
            //             orderby l.CreateDateTime descending
            //             select new
            //             {
            //                 u.Avatar,
            //                 u.NickName,
            //                 l.CreateDateTime
            //             }).ToPagedList(1, pageSize);
            var hadLike = db.UserLogs
                .Any(s => s.RelationID == articleID
                 && s.Type == Common.Enums.UserLogType.ArticleLike
                 && s.UserID == userID);
            var model = new
            {
                ArticleID = a.ID,
                Avatar = a.Type == Common.Enums.ArticleType.Text ? c.Avatar : e.Logo,
                CommentCount = com.ToStrForm(4, "评论"),
                DateTimeStr = a.UpdateDateTime.ToStrForm(),
                HadLike = hadLike,
                LikeCount = a.Like.ToStrForm(4, "点赞"),
                Title = a.Title,
                a.Content,
                Images = a.Type == Common.Enums.ArticleType.Html ? new string[0] : (a.Images.SplitToArray<string>()?.ToArray() ?? new string[0]),
                //LikeUser = new
                //{
                //    Page = new
                //    {
                //        likes.PageSize,
                //        likes.PageCount,
                //        likes.PageNumber,
                //        likes.HasNextPage
                //    },
                //    Data = likes.Select(s => new
                //    {
                //        s.Avatar,
                //        CreateDateTime = s.CreateDateTime.ToStrForm(),
                //        UserName = s.NickName
                //    })
                //},
                //Comments = new
                //{
                //    Page = new
                //    {
                //        com.PageSize,
                //        com.PageCount,
                //        com.PageNumber,
                //        com.HasNextPage
                //    },
                //    Data = com.Select(s => new
                //    {
                //        s.Avatar,
                //        s.Content,
                //        DateTimeStr = s.CreateDateTime.ToStrForm(),
                //        UserName = s.NickName,
                //        CommentID = s.ID,
                //    })
                //},
                a.Type,
                ShareCount = a.Share.ToStrForm(4, "分享"),
                Position = a.Type == Common.Enums.ArticleType.Text ? c.Position : "",
                Cover = a.Type == Common.Enums.ArticleType.Html ? a.Images.SplitToArray<string>()?[0] : null,
                UserName = a.Type == Common.Enums.ArticleType.Text ? c.Name : e.Name,

            };
            return Json(Comm.ToJsonResult("Success", "成功", model), JsonRequestBehavior.AllowGet);
        }

        [AllowCrossSiteJson]
        public ActionResult LikeList(int articleID, int page = 1, int pageSize = 20)
        {
            if (!db.Articles.Any(s => s.ID == articleID && s.State == Common.Enums.ArticleState.Released))
            {
                return Json(Comm.ToJsonResult("CardNoFound", "动态不存在"), JsonRequestBehavior.AllowGet);
            }
            var paged = (from u in db.Users
                         from l in db.UserLogs
                         where u.Id == l.UserID && l.RelationID == articleID && l.Type == Common.Enums.UserLogType.ArticleLike
                         orderby l.CreateDateTime descending
                         select new { u.Avatar, UserName = u.NickName })
                         .ToPagedList(page, pageSize);
            return Json(Comm.ToJsonResultForPagedList(paged, paged), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Ai雷达 发布动态
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult PushArticle(int EnterpriseID, string UserID, string Content, string Images)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Content) && string.IsNullOrWhiteSpace(Images))
                {
                    return Json(Comm.ToJsonResult("Error", "发布内容或者图片不能为空"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Article model = new Article();

                    model.EnterpriseID = EnterpriseID;
                    model.UserID = UserID;
                    model.Content = Content;
                    model.Images = Images;
                    model.Type = Common.Enums.ArticleType.Text;
                    model.CreateDateTime = DateTime.Now;
                    model.UpdateDateTime = DateTime.Now;
                    model.State = Common.Enums.ArticleState.Wait;
                    model.Comment = 0;
                    model.Share = 0;
                    model.Like = 0;
                    db.Articles.Add(model);
                    db.SaveChanges();
                    return Json(Comm.ToJsonResult("Success", "成功"), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error500", ex.Message), JsonRequestBehavior.AllowGet);
            }
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