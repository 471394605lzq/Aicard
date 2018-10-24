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
            var filter = from a in db.Articles
                         join e in db.Enterprises on a.EnterpriseID equals e.ID into ae
                         join u in db.Users on a.UserID equals u.Id into au
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
                             a.CreateDateTime,
                             a.EnterpriseID,
                             a.UserID,
                             User = au.FirstOrDefault(),
                             Enterprise = ae.FirstOrDefault()
                         };
            if (page > 1)
            {
                if (!string.IsNullOrWhiteSpace(userID))
                {
                    filter = filter.Where(s => s.UserID == userID);
                }
                else if (enterpriseID.HasValue)
                {
                    filter = filter.Where(s => s.EnterpriseID == enterpriseID);
                }
            }
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