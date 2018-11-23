﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.DAL.Models;
using AiCard.Common;

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

        [HttpGet]
        public ActionResult GetInfo(string userID)
        {
            var vip = db.Vips
                .FirstOrDefault(s => s.UserID == userID
                    && s.State == Common.Enums.VipState.Enable);
            if (vip == null)
            {
                return Json(Comm.ToJsonResult("VipNoFound", "用户不是VIP"), JsonRequestBehavior.AllowGet);
            }
            var today = DateTime.Now.Date.AddSeconds(-1);
            var todayTotalAmount = db.VipAmountLogs
                .Where(s => s.UserID == userID && s.Amount > 0 && s.CreateDateTime > today)
                .Sum(s => (decimal?)s.Amount) ?? 0;
            return Json(Comm.ToJsonResult("Success", "成功", new
            {
                vip.Code,
                vip.Amount,
                vip.TotalAmount,
                TodayTotalAmount = todayTotalAmount,
            }), JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult GetList(string userID, DateTime? date, int page = 1, int pageSize = 20,
            Common.Enums.VipAmountGetListType type = Common.Enums.VipAmountGetListType.All)
        {
            DateTime start, end;
            if (!date.HasValue)
            {
                date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }
            start = date.Value.AddSeconds(-1);
            end = date.Value.AddMonths(1);
            var logs = from va in db.VipAmountLogs
                       join u in db.Users.Select(x => new { x.Id, x.NickName, x.Avatar })
                        on va.SourceUserID equals u.Id into vau
                       where va.CreateDateTime > start && va.CreateDateTime < end
                       select new
                       {
                           va.ID,
                           va.CreateDateTime,
                           va.Amount,
                           va.Type,
                           User = vau.FirstOrDefault()
                       };
            switch (type)
            {
                case Common.Enums.VipAmountGetListType.All:
                    break;
                case Common.Enums.VipAmountGetListType.Income:
                    logs = logs.Where(s => s.Amount > 0);
                    break;
                case Common.Enums.VipAmountGetListType.Expense:
                    logs = logs.Where(s => s.Amount < 0);
                    break;
                default:
                    break;
            }
            var paged = logs.OrderByDescending(s => s.CreateDateTime).ToPagedList(page, pageSize);
            var withdrawIcon = Url.ContentFull("~/Content/Images/ic_withdraw_list_40.png");
            Func<DateTime, string> dateToStr = d =>
            {
                if (d.Date == DateTime.Now.Date)
                {
                    return d.ToString("今天 HH:mm");
                }
                else if (d.Date == DateTime.Now.Date.AddDays(-1))
                {
                    return d.ToString("昨天 HH:mm");
                }
                return d.ToString("MM-dd HH:mm");
            };
            var data = paged.Select(s =>
            {
                string image = s.User?.Avatar, name = s.User?.NickName ?? "", content = "", remark = "";
                switch (s.Type)
                {
                    case Common.Enums.VipAmountLogType.NewCard:
                        {
                            content = "创建名片";
                        }
                        break;
                    case Common.Enums.VipAmountLogType.NewChild2nd:
                        {
                            content = "成为你的一级用户";
                        }
                        break;
                    case Common.Enums.VipAmountLogType.NewChild3rd:
                        {
                            content = "成为你的二级用户";
                        }
                        break;
                    case Common.Enums.VipAmountLogType.Forward:
                        {
                            image = withdrawIcon;
                            content = "提现";
                        }
                        break;
                    case Common.Enums.VipAmountLogType.ForwardFail:
                        {
                            image = withdrawIcon;
                            content = "提现";
                            remark = "提现失败，资金返回余额";
                        }
                        break;
                    default:
                        break;
                }
                return new
                {
                    Image = image,
                    Name = name,
                    Content = content,
                    Remark = remark,
                    DateTime = dateToStr(s.CreateDateTime),
                    Amount = s.Amount
                };
            });
            return Json(Comm.ToJsonResultForPagedList(paged, data), JsonRequestBehavior.AllowGet);
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