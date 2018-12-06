using AiCard.Bll;
using AiCard.Common;
using AiCard.Models.VipForward;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static AiCard.Common.Comm;

namespace AiCard.Controllers
{
    public class VipForwardController : Controller
    {
        VipForwardBLL bll = new VipForwardBLL();
        // GET: VipForward
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 提现申请
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult VipForwardApply(MReqVipForward model, string userID)
        {
            #region 校验数据
            if (string.IsNullOrWhiteSpace(userID))
            {
                return Json(Comm.ToJsonResult("Error", "userID参数不能为空"));
            }
            if (model == null)
            {
                return Json(Comm.ToJsonResult("Error", "提交的数据不能为空"));
            }
            if (model.bankAccountID<=0)
            {
                return Json(Comm.ToJsonResult("Error", "请选择银行卡"));
            }
            if (model.forwardAmount<=0)
            {
                return Json(Comm.ToJsonResult("Error", "请输入提现金额"));
            }
            if (model.forwardAmount < 50)
            {
                return Json(Comm.ToJsonResult("Error", "提现金额不能少于50"));
            }
            if (model.forwardAmount > 20000)
            {
                return Json(Comm.ToJsonResult("Error", "提现金额不能高于2万"));
            }
            if (model.forwardAmount % 50 != 0)
            {
                return Json(Comm.ToJsonResult("Error", "提现金额必须为50的整数倍"));
            }
            #endregion

            #region 
            RequestResult result = bll.VipForwardApply(model,userID);
            if (result.retCode == ReqResultCode.success)
            {
                return Json(Comm.ToJsonResult("Success", result.retMsg, result.objectData));
            }
            else
            {
                return Json(Comm.ToJsonResult("Error", result.retMsg));
            }
            #endregion
        }

        /// <summary>
        /// 获取用户绑定的提现银行卡
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult GetBindBankAccount(string userID)
        {
            #region 校验数据
            if (string.IsNullOrWhiteSpace(userID))
            {
                return Json(Comm.ToJsonResult("Error", "userID参数不能为空"));
            }
            #endregion

            #region 
            RequestResult result = bll.GetBindBankAccount(userID);
            if (result.retCode == ReqResultCode.success)
            {
                return Json(Comm.ToJsonResult("Success", result.retMsg,result.objectData));
            }
            else
            {
                return Json(Comm.ToJsonResult("Error", result.retMsg));
            }
            #endregion
        }
    }
}