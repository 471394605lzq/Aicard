using AiCard.Common;
using AiCard.DAL.Models;
using AiCard.Models.Order;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AiCard.Controllers
{
    public class OrderManageController : Controller
    {
        /// <summary>
        /// 获取升级及退款订单记录
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult Index(string Name,string Mobile, int page = 1, int pageSize = 15)
        {
            #region
            string selectStr = string.Empty;
            try
            {
                string sw = string.Empty;
                if (!string.IsNullOrWhiteSpace(Mobile))
                {
                    sw = $" and t2.Mobile like '%{Mobile}%' ";
                }
                if (!string.IsNullOrWhiteSpace(Name))
                {
                    sw = $" and t2.Name like '%{Name}%' ";
                }
                selectStr = $@"select t1.ID as OrderID,t1.Amount,t1.[Code],t1.Channel,t1.[Type],t1.[State],
                                    t1.PayCode,t1.ReceivableAmount,t1.CreateDateTime,t2.Name as UserName,
                                    t1.PayDateTime ,t1.PayInput,t1.PayResult,t2.Mobile
                                    from Orders t1 
                                    left join CardPersonals t2 on t1.UserID=t2.UserID
                                    where 1=1  {sw}
                                    order by t1.CreateDateTime desc";

                #region
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var query = db.Database.SqlQuery<MOrderInfo>(selectStr);
                    var paged = query.ToPagedList(page, pageSize);
                    return View(paged);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Comm.WriteLog("OrderManageController.Index", ex.Message, Common.Enums.DebugLogLevel.Error, ex: ex);
                return View($"获取升级及退款订单记录发生异常：{ex.Message}");
            }
            #endregion
        }
    }
}