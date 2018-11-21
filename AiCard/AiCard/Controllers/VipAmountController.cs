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
                             v.TotalMonthAmountRank,
                             v.TotalWeekAmountRank,
                             c.Avatar,
                             v.Type,
                             v.UserID
                         });
            switch (type)
            {
                case Common.Enums.VipTotalAmountRankType.All:
                    query = query.OrderBy(s => s.TotalAmountRank);
                    break;
                case Common.Enums.VipTotalAmountRankType.Week:
                    query = query.OrderBy(s => s.TotalWeekAmountRank);
                    break;
                case Common.Enums.VipTotalAmountRankType.Month:
                    query = query.OrderBy(s => s.TotalMonthAmountRank);
                    break;
                default:
                    break;
            }
            var paged = query.ToPagedList(page, pageSize);

            var data = paged.Select(s => new
            {
                Amount = GetRank(type, s.TotalAmount, 0, 0),
                Rank = GetRank(type, s.TotalAmountRank, s.TotalWeekAmountRank, s.TotalMonthAmountRank),
                s.Avatar,
                s.Name,
                s.Type,
                s.UserID,
            });
            return Json(Common.Comm.ToJsonResultForPagedList(paged, data), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MyRank(string userID, Common.Enums.VipTotalAmountRankType type = Common.Enums.VipTotalAmountRankType.All)
        {
            var user = (from v in db.Vips
                        from c in db.CardPersonals
                        where c.ID == v.CardID && v.UserID == userID
                        select new
                        {
                            v.TotalAmount,
                            c.Name,
                            v.TotalAmountRank,
                            v.TotalWeekAmountRank,
                            v.TotalMonthAmountRank,
                            c.Avatar,
                            v.Type,
                            v.UserID
                        }).FirstOrDefault();
            return Json(Common.Comm.ToJsonResult("Success", "成功", new
            {
                Amount = GetRank(type, user.TotalAmount, 0, 0),
                Rank = GetRank(type, user.TotalAmountRank, user.TotalWeekAmountRank, user.TotalMonthAmountRank),
                user.Avatar,
                user.Name,
                user.Type,
                user.UserID,
            }), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取对应榜的数据
        /// </summary>
        /// <param name="type">类别</param>
        /// <param name="all">总榜</param>
        /// <param name="week">周榜</param>
        /// <param name="month">月榜</param>
        /// <returns></returns>
        public T GetRank<T>(Common.Enums.VipTotalAmountRankType type, T all, T week, T month)
        {
            switch (type)
            {
                default:
                case Common.Enums.VipTotalAmountRankType.All:
                    return all;
                case Common.Enums.VipTotalAmountRankType.Week:
                    return week;
                case Common.Enums.VipTotalAmountRankType.Month:
                    return month;
            }
        }

        /// <summary>
        /// 对VIP进行排行处理
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadTotalAmountRank()
        {
            //统计总榜
            var sql = $@"UPDATE Vips 
            SET TotalAmountRank= v2.[Rank]
            FROM
            dbo.Vips Join
            (SELECT DENSE_RANK() OVER (ORDER BY TotalAmount  Desc) AS [Rank],ID 
            FROM dbo.Vips v1 WHERE v1.TotalAmount>0) AS v2 ON v2.ID = Vips.ID";
            var countTotalAmount = db.Database.ExecuteSqlCommand(sql);
            return Json(Common.Comm.ToJsonResult("Success", "统计排行完成"));
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