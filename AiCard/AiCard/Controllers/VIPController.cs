using AiCard.Bll;
using AiCard.Commom.WeChatPay;
using AiCard.Common;
using AiCard.Common.WeChat;
using AiCard.DAL.Models;
using AiCard.Models.Vip;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WxPayAPI;

namespace AiCard.Controllers
{
    /// <summary>
    /// author:lym
    /// date:2018-11-16
    /// VIP会员控制器
    /// </summary>
    public class VIPController : Controller
    {
        OrderBLL orderbll = new OrderBLL();

        /// <summary>
        /// 获取VIP会员名片分页列表
        /// </summary>
        /// <param name="sReqParameter">请求的参数</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult Index(string sReqParameter = "")
        {
            #region
            ReqVipCardList reqParam = JsonConvert.DeserializeObject<ReqVipCardList>(sReqParameter);
            if (reqParam == null)
            {
                reqParam = new Models.Vip.ReqVipCardList()
                {
                    filter = string.Empty,
                    Page = 1,
                    PageSize = 20
                };
            }
            string selectStr = string.Empty;
            try
            {
                #region
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    selectStr = $@"select t1.ID as VipID,t1.Amount,t1.TotalAmount,t1.VipChild2ndCount,t1.VipChild3rdCount,t1.FreeChildCount,
                                    t1.[State] ,t2.Name,t2.Avatar,t2.Mobile,t2.Gender,t3.ID as UserID
                                    from Vips t1 
                                    left join CardPersonals t2 on t1.CardID=t2.ID
                                    left join AspNetUsers t3 on t1.UserID=t3.ID
                                    where t1.[Type]=={Common.Enums.VipRank.Vip99} 
                                    order by ";

                    db.Database.SqlQuery<VipCardList>(selectStr);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Comm.WriteLog("VIPCotroller.Index", ex.Message, Common.Enums.DebugLogLevel.Error, ex: ex);
                return Json(Comm.ToJsonResult("Error", "调用获取vip用户列表接口发生异常"), JsonRequestBehavior.AllowGet);
            }
            return Json(Comm.ToJsonResult("Success", "成功", ""), JsonRequestBehavior.AllowGet);
            #endregion
        }

        /// <summary>
        /// 升级VIP，创建订单及预调起支付
        /// </summary>
        /// <param name="code">微信小程序登录返回的code</param>
        /// /// <param name="UserID">用户ID</param>
        /// <returns></returns>
        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult UpGradeVIP(string code, string UserID)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return Json(Comm.ToJsonResult("Error", "code参数不能为空"), JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(UserID))
            {
                return Json(Comm.ToJsonResult("Error", "用户ID不能为空"), JsonRequestBehavior.AllowGet);
            }
            dynamic result = null;
            try
            {
                result = orderbll.CreateUpGradeOrder(code, UserID);
            }
            catch (Exception ex)
            {
                Comm.WriteLog("WeChatPay", ex.Message, Common.Enums.DebugLogLevel.Error, ex: ex);
                return Json(Comm.ToJsonResult("Error", "调用升级接口发生异常"), JsonRequestBehavior.AllowGet);

            }

            return Json(Comm.ToJsonResult(result.retCode, result.retMsg, result.objectData), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 退款测试
        /// </summary>
        /// <param name="code"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult Refund(string code, string UserID)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return Json(Comm.ToJsonResult("Error", "code参数不能为空"), JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(UserID))
            {
                return Json(Comm.ToJsonResult("Error", "用户ID不能为空"), JsonRequestBehavior.AllowGet);
            }
            dynamic result = null;
            try
            {
                result = orderbll.RefundApply(code, UserID);
            }
            catch (Exception ex)
            {
                Comm.WriteLog("Refund", ex.Message, Common.Enums.DebugLogLevel.Error, ex: ex);
                return Json(Comm.ToJsonResult("Error", "调用退款接口发生异常"), JsonRequestBehavior.AllowGet);

            }

            return Json(Comm.ToJsonResult(result.retCode, result.retMsg, result.objectData), JsonRequestBehavior.AllowGet);
        }

    }
}