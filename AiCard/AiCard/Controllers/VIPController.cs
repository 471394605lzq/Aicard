using AiCard.Bll;
using AiCard.Commom.WeChatPay;
using AiCard.Common;
using AiCard.Common.WeChat;
using AiCard.DAL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WxPayAPI;

namespace AiCard.Controllers
{
    public class VIPController : Controller
    {
        OrderBLL orderbll = new OrderBLL();
       
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
            if (string.IsNullOrWhiteSpace(code)) {
                return Json(Comm.ToJsonResult("Error", "code参数不能为空"), JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrWhiteSpace(UserID))
            {
                return Json(Comm.ToJsonResult("Error", "用户ID不能为空"), JsonRequestBehavior.AllowGet);
            }
            dynamic result = null;
            try
            {
                result= orderbll.CreateUpGradeOrder(code,UserID);
            }
            catch (Exception)
            {
                return Json(Comm.ToJsonResult("Error", "调用升级接口发生异常"), JsonRequestBehavior.AllowGet);

            }

            return Json(Comm.ToJsonResult(result.retCode, result.retMsg, result.objectData), JsonRequestBehavior.AllowGet);
        }
        
        
    }
}