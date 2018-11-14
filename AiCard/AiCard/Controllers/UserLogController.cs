using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
using AiCard.Common.Enums;
using AiCard.DAL.Models;

namespace AiCard.Controllers
{
    public class UserLogController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [AllowCrossSiteJson]
        public ActionResult CardLikeList(int cardID, int page = 1, int pageSize = 20)
        {
            var likes = Bll.UserLogs.Search(relationID: cardID, type: UserLogType.CardLike, page: page, pageSize: pageSize);
            var data = likes.Select(s => new { s.UserAvatar, s.UserNickName });
            return Json(Comm.ToJsonResultForPagedList(likes, data), JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult Create(UserLog log)
        {
            try
            {
                Bll.UserLogs.Add(log);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message));
            }
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