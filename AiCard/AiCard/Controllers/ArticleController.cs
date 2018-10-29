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

        // GET: Article
        public ActionResult Index(string userID, int? enterpriseID, int page = 1, int pageSize = 20, DateTime? time = null)
        {
            if (string.IsNullOrWhiteSpace(userID) && !enterpriseID.HasValue)
            {
                return Json(Comm.ToJsonResult("Error", "UserID和EnterpriseID不能同时为空"));
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
                                     from u in db.Users.Select(s => new { s.Id, s.NickName })
                                     where l.UserID == u.Id && l.Type == Enums.UserLogType.ArticleLike
                                     orderby l.CreateDateTime descending
                                     select new { l.RelationID, l.UserID, u.NickName })
                            on a.ID equals ll.RelationID into all
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
                             Liker = all.Take(2)
                         };
            Func<DateTime, string> dateToString = datetime =>
            {
                var ts = datetime - DateTime.Now;
                if (ts.TotalDays > 3)
                {
                    return datetime.ToString("yyyy/MM/dd HH:mm");
                }
                else if (ts.TotalDays > 1)
                {
                    return $"{(int)ts.TotalDays}天前";
                }
                else if (ts.TotalHours > 1)
                {
                    return $"{(int)ts.TotalHours}小时前";
                }
                else
                {
                    var min = (int)ts.TotalMinutes;
                    return $"{(min < 1 ? 1 : min)}分钟前";
                }
            };

            Func<int, string> coutToString = i =>
            {
                if (i >= 10000)
                {
                    return (i / 10000).ToString("#万");
                }
                else
                {
                    return i.ToString();
                }
            };
            //单分页大于（往下拉取数据）
            if (page >= 1)
            {
                if (time.HasValue)
                {
                    filter = filter.Where(s => s.UpdateDateTime < time);
                }
                if (!string.IsNullOrWhiteSpace(userID))
                {
                    filter = filter.Where(s => s.UserID == userID);
                }
                else if (enterpriseID.HasValue)
                {
                    filter = filter.Where(s => s.EnterpriseID == enterpriseID);
                }
                var paged = filter.OrderByDescending(s => s.UpdateDateTime).ToPagedList(page, pageSize);
                var data = paged.Select(s =>
                {
                    string lUser = string.Join(",", s.Liker.Select(u => u.NickName));
                    var a = new ArticleIndexViewModels
                    {
                        ID = s.ID,
                        Avatar = s.User.Avatar,
                        UserName = s.User.Name,
                        DateTimeStr = dateToString(s.UpdateDateTime),
                        DateTime = s.UpdateDateTime,
                        CommentCount = coutToString(s.Comment),
                        LikeCount = coutToString(s.Like),
                        Content = s.Content,
                        Cover = s.Type == Enums.ArticleType.Html ? s.Images : null,
                        Images = s.Type == Enums.ArticleType.Text ? s.Images.SplitToArray<string>().ToArray() : new string[0],
                        LikeUser = s.Like > 2 ? $"{lUser}等{s.Like - 2}人觉得很赞" : $"{lUser}觉得很赞",
                        Position = s.User.Position,
                        ShareCount = coutToString(s.Share)
                    };
                    return a;
                });
                return Json(Comm.ToJsonResultForPagedList(paged, data));
            }
            else
            {   //单分页小于（往上拉取数据）
                var data = filter.Where(s => s.UpdateDateTime > time)
                    .OrderByDescending(s => s.UpdateDateTime)
                    .ToList()
                    .Select(s =>
                   {
                       string lUser = string.Join(",", s.Liker.Select(u => u.NickName));
                       var a = new ArticleIndexViewModels
                       {
                           ID = s.ID,
                           Avatar = s.User.Avatar,
                           UserName = s.User.Name,
                           DateTimeStr = dateToString(s.UpdateDateTime),
                           DateTime = s.UpdateDateTime,
                           CommentCount = coutToString(s.Comment),
                           LikeCount = coutToString(s.Like),
                           Content = s.Content,
                           Cover = s.Type == Enums.ArticleType.Html ? s.Images : null,
                           Images = s.Type == Enums.ArticleType.Text ? s.Images.SplitToArray<string>().ToArray() : new string[0],
                           LikeUser = s.Like > 2 ? $"{lUser}等{s.Like - 2}人觉得很赞" : $"{lUser}觉得很赞",
                           Position = s.User.Position,
                           ShareCount = coutToString(s.Share)
                       };
                       return a;
                   }).ToList();
                return Json(Comm.ToJsonResult("Success", "成功", data));
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