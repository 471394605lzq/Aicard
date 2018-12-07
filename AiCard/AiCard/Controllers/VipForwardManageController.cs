using AiCard.Commom;
using AiCard.Common;
using AiCard.DAL.Models;
using AiCard.Models.VipForwardOrder;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AiCard.Controllers
{
    public class VipForwardManageController : Controller
    {
        /// <summary>
        /// 获取提现申请数据
        /// </summary>
        /// <param name="sReqParameter">请求的参数</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult Index(string filter, int page = 1, int pageSize = 15)
        {
            #region
            string selectStr = string.Empty;
            try
            {
                #region
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    string sw = string.Empty;
                    //if (!string.IsNullOrWhiteSpace(filter))
                    //{
                    //    sw = $" and (t2.Mobile like '%{filter}%' or t2.Name like '%{filter}%') ";
                    //}
                    selectStr = $@"select t1.ID as OrderID,t1.Amount,t1.ReceivableAmount,t1.Code,t1.PayCode,t2.Name as UserName,
                                    t2.PhoneNumber as Phone ,t1.[Type],(t3.Bank+'/'+t3.ForwardName+'/'+t1.ToAccount) as ToAccount,t1.[State],t1.Remark,t1.CreateDateTime,t1.PayDateTime,
                                    t1.PayResult
                                    from VipForwardOrders t1 
                                    inner join CardPersonals t2 on t1.UserID=t2.UserID
                                    left join VipForwardAccounts t3 on t1.UserID=t3.UserID and t3.ForwardAccount=t1.ToAccount
                                    where 1=1 {sw}
                                    order by t1.[Type] asc, t1.CreateDateTime desc";

                    var query = db.Database.SqlQuery<MVipForwardOrderList>(selectStr);
                    var paged = query.ToPagedList(page, pageSize);
                    return View(paged);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Comm.WriteLog("VipForwardManageController.Index", ex.Message, Common.Enums.DebugLogLevel.Error, ex: ex);
                return View($"获取提现申请数据发生异常：{ex.Message}");
            }
            #endregion
        }

        /// <summary>
        /// 导出待审核的提现订单
        /// </summary>
        /// <param name="sReqParameter">请求的参数</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult ExportToExcel()
        {
            #region
            string selectStr = string.Empty;
            try
            {
                #region
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    
                    //selectStr = $@"";
                    
                    //List<MVipForwardOrderList> list = db.Database.SqlQuery<MVipForwardOrderList>(selectStr).ToList();
                    //if (list != null && list.Count > 0) {
                    //    DataTable table= ModelConvertHelper.ConvertToDataTable(list);
                    //    return  ControllerExtensions.Excel(this, table,$"提现申请数据{DateTime.Now.ToString("yyyyMMddhhmmss")}");
                    //}
                     
                }
                #endregion
            }
            catch (Exception ex)
            {
                Comm.WriteLog("VipForwardManageController.Index", ex.Message, Common.Enums.DebugLogLevel.Error, ex: ex);
                return View($"获取提现申请数据发生异常：{ex.Message}");
            }
            return View($"已无待审核的提现申请记录");
            #endregion
        }

    }
}