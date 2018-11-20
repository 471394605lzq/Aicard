using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.DAL.Models;
namespace AiCard.Controllers
{
    public class VipAmountController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult Top(int page = 1, int pageSize = 100, Common.Enums.VipTotalAmountRankType type = Common.Enums.VipTotalAmountRankType.All)
        {
            var query = (from v in db.Vips
                         from c in db.CardPersonals
                         where c.ID == v.CardID && v.TotalAmountRank > 0
                         select new
                         {
                             v.TotalAmount,
                             c.Name,
                             v.TotalAmountRank,
                             c.Avatar,
                         });
            switch (type)
            {
                case Common.Enums.VipTotalAmountRankType.All:
                    query = query.OrderBy(s => s.TotalAmountRank);
                    break;
                case Common.Enums.VipTotalAmountRankType.Week:
                    query = query.OrderBy(s => s.TotalAmountRank);
                    break;
                case Common.Enums.VipTotalAmountRankType.Month:
                    query = query.OrderBy(s => s.TotalAmountRank);
                    break;
                default:
                    break;
            }
            var paged = query.ToPagedList(page, pageSize);
            return Json(Common.Comm.ToJsonResultForPagedList(paged, paged));
        }

        public ActionResult MyRank(string userID)
        {
            var user = (from v in db.Vips
                        from c in db.CardPersonals
                        where c.ID == v.CardID && v.UserID == userID
                        select new
                        {
                            v.TotalAmount,
                            c.Name,
                            v.TotalAmountRank,
                            c.Avatar,
                        }).FirstOrDefault();
            return Json(Common.Comm.ToJsonResult("Success", "成功", user));
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