using AiCard.Common;
using AiCard.DAL.Models;
using AiCard.Models.Vip;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AiCard.Controllers
{
    public class VIPManageController : Controller
    {
        #region 后台方法
        /// <summary>
        /// 获取VIP会员名片分页列表
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
                    if (!string.IsNullOrWhiteSpace(filter))
                    {
                        sw = $" and (t2.Mobile like '%{filter}%' or t2.Name like '%{filter}%') ";
                    }
                    selectStr = $@"select t1.ID as VipID,t1.Amount,t1.TotalAmount,t1.VipChild2ndCount,t1.VipChild3rdCount,t1.FreeChildCount,
                                    (case when t1.[State]=1 then '启用' else '禁用' end) as StateName ,t2.Name,t2.Avatar,t2.Mobile,
                                    (case when t2.Gender=1 then '男' when t2.Gender=2 then '女' else '未设置' end) as Gender,t3.UserName as UserName
                                    from Vips t1 
                                    inner join CardPersonals t2 on t1.CardID=t2.ID
                                    left join AspNetUsers t3 on t1.UserID=t3.ID
                                    where t1.[Type]={(int)Common.Enums.VipRank.Vip99}  {sw}
                                    order by t1.CreateDateTime desc";

                    var query = db.Database.SqlQuery<VipCardList>(selectStr);
                    var paged = query.ToPagedList(page, pageSize);
                    return View(paged);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Comm.WriteLog("VIPCotroller.Index", ex.Message, Common.Enums.DebugLogLevel.Error, ex: ex);
                return View($"获取vip用户信息发生异常：{ex.Message}");
            }
            #endregion
        }

        /// <summary>
        /// 根据vip会员ID获取名片详情
        /// </summary>
        /// <param name="VipID"></param>
        /// <returns></returns>
        public ActionResult Details(int VipID)
        {
            #region
            if (VipID <= 0) return View("vip用户ID不符合规范");
            string selectStr = string.Empty;
            try
            {
                #region
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    selectStr = $@"select t2.Name,t2.Avatar,t2.Mobile,
                                    (case when t2.Gender=1 then '男' when t2.Gender=2 then '女' else '未设置' end) as Gender,t3.UserName as UserName,
                                    t2.Enterprise,t2.PhoneNumber,t2.Email,t2.WeChatCode,t2.Position,t2.Remark,t2.Info,t2.Voice,t2.Video,t2.Images,t2.WeChatMiniQrCode,
                                    t2.Poster,t2.Industry,t2.[Like],t2.[View]
                                    from Vips t1 
                                    inner join CardPersonals t2 on t1.CardID=t2.ID
                                    left join AspNetUsers t3 on t1.UserID=t3.ID
                                    where t1.ID={VipID} ";

                    VipCardDetail detail = db.Database.SqlQuery<VipCardDetail>(selectStr).FirstOrDefault();

                    return View(detail);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Comm.WriteLog("VIPCotroller.Details", ex.Message, Common.Enums.DebugLogLevel.Error, ex: ex);
                return View($"获取vip用户详情发生异常：{ex.Message}");
            }
            #endregion
        }

        /// <summary>
        /// 获取vip用户账户收支明细
        /// </summary>
        /// <param name="VipID"></param>
        /// <returns></returns>
        public ActionResult AccountDetail(int VipID, int page = 1, int pageSize = 15)
        {
            #region
            if (VipID <= 0) return View("vip用户ID不符合规范");
            string selectStr = string.Empty;
            try
            {
                #region
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    selectStr = $@"select t1.CreateDateTime,t1.[Type],t1.Amount,t1.Before, isnull(t2.Name,'') as SourceUserName
                                    from VipAmountLogs t1 
                                    left join CardPersonals t2 on t1.SourceUserID=t2.UserID
                                    where t1.VipID={VipID} 
                                    order by t1.CreateDateTime desc";

                    var query = db.Database.SqlQuery<VipAccountDetail>(selectStr);
                    var paged = query.ToPagedList(page, pageSize);
                    return View(paged);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Comm.WriteLog("VIPCotroller.AccountDetail", ex.Message, Common.Enums.DebugLogLevel.Error, ex: ex);
                return View($"获取vip用户账户收支明细发生异常：{ex.Message}");
            }
            #endregion
        }

        /// <summary>
        /// 获取会员下级用户
        /// </summary>
        /// <param name="VipID"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult ChildList(int VipID, int page = 1, int pageSize = 15) {
            #region
            if (VipID <= 0) return View("vip用户ID不符合规范");
            string selectStr = string.Empty;
            try
            {
                #region
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    
                    return View();
                }
                #endregion
            }
            catch (Exception ex)
            {
                Comm.WriteLog("VIPCotroller.ChildList", ex.Message, Common.Enums.DebugLogLevel.Error, ex: ex);
                return View($"获取会员下级用户发生异常：{ex.Message}");
            }
            #endregion
        }

        #endregion
    }
}