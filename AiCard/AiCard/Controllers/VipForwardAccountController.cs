using AiCard.Bll;
using AiCard.Commom.Aliyun.BankCard;
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
    public class VipForwardAccountController : Controller
    {
        VipForwardAccountBLL bll = new VipForwardAccountBLL();
        // GET: VipForwardAccount
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 认证提现银行卡
        /// </summary>
        /// <param name="model">银行卡信息</param>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult CheckBankInfo(BankCardModel model, string userID)
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
            if (string.IsNullOrWhiteSpace(model.idNo))
            {
                return Json(Comm.ToJsonResult("Error", "证件号不能为空"));
            }
            if (string.IsNullOrWhiteSpace(model.cardNo))
            {
                return Json(Comm.ToJsonResult("Error", "银行卡号不能为空"));
            }
            if (string.IsNullOrWhiteSpace(model.name))
            {
                return Json(Comm.ToJsonResult("Error", "开户名不能为空"));
            }
            if (string.IsNullOrWhiteSpace(model.phoneNo))
            {
                return Json(Comm.ToJsonResult("Error", "预留电话号码不能为空"));
            }
            #endregion
            
            #region 认证银行卡
            RequestResult result = bll.CheckBankInfo(model, userID);
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
        /// 绑定提现银行卡
        /// </summary>
        /// <param name="model">银行卡信息</param>
        /// <param name="userID">用户ID</param>
        /// <param name="phoneNumber">用户手机号</param>
        /// <param name="validCode">短信验证码</param>
        /// <returns></returns>
        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult BindBankAccount(MReqBankAccount model, string userID,string phoneNumber, string validCode)
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
            if (string.IsNullOrWhiteSpace(model.idNo))
            {
                return Json(Comm.ToJsonResult("Error", "证件号不能为空"));
            }
            if (string.IsNullOrWhiteSpace(model.cardNo))
            {
                return Json(Comm.ToJsonResult("Error", "银行卡号不能为空"));
            }
            if (string.IsNullOrWhiteSpace(model.name))
            {
                return Json(Comm.ToJsonResult("Error", "开户名不能为空"));
            }
            if (string.IsNullOrWhiteSpace(model.phoneNo))
            {
                return Json(Comm.ToJsonResult("Error", "银行预留手机号不能为空"));
            }
            if (string.IsNullOrWhiteSpace(model.bankCode))
            {
                return Json(Comm.ToJsonResult("Error", "银行编号不能为空"));
            }
            if (string.IsNullOrWhiteSpace(model.bankName))
            {
                return Json(Comm.ToJsonResult("Error", "银行名称不能为空"));
            }
            if (string.IsNullOrWhiteSpace(validCode))
            {
                return Json(Comm.ToJsonResult("Error", "手机验证码不能为空"));
            }
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return Json(Comm.ToJsonResult("Error", "用户手机号不能为空"));
            }
            #endregion

            #region 绑定银行卡
            RequestResult result = bll.CreateBankAccount(model, userID, phoneNumber, validCode);
            if (result.retCode == ReqResultCode.success)
            {
                return Json(Comm.ToJsonResult("Success", result.retMsg));
            }
            else
            {
                return Json(Comm.ToJsonResult("Error", result.retMsg));
            }
            #endregion
        }


        /// <summary>
        /// 删除提现银行卡
        /// </summary>
        /// <param name="bankAccountID">银行卡账户ID</param>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult DeleteBankAccount( int bankAccountID, string userID)
        {
            #region 校验数据
            if (string.IsNullOrWhiteSpace(userID))
            {
                return Json(Comm.ToJsonResult("Error", "userID参数不能为空"));
            }
            if (bankAccountID <=0)
            {
                return Json(Comm.ToJsonResult("Error", "银行账户ID不能为空"));
            }
            #endregion
            RequestResult result = bll.DeleteBankAccount(bankAccountID, userID);
            if (result.retCode == ReqResultCode.success)
            {
                return Json(Comm.ToJsonResult("Success", result.retMsg));
            }
            else
            {
                return Json(Comm.ToJsonResult("Error", result.retMsg));
            }
        }
    }
}