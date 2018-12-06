using AiCard.Common;
using AiCard.DAL.Models;
using AiCard.Models.VipForward;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static AiCard.Common.Comm;

namespace AiCard.Bll
{
    /// <summary>
    /// author：lym
    /// date:2018-11-27
    /// 提现订单相关业务层
    /// </summary>
    public sealed class VipForwardBLL
    {
        /// <summary>
        /// 提现申请
        /// </summary>
        /// <param name="model"></param>
        /// <param name="vUserID"></param>
        /// <returns></returns>
        public RequestResult VipForwardApply(MReqVipForward model, string vUserID)
        {
            #region
            RequestResult result = new RequestResult()
            {
                retCode = ReqResultCode.failed,
                retMsg = "申请提现失败"
            };

            int rows = 0;
            try
            {
                

            }
            catch (Exception ex)
            {
                result.retCode = ReqResultCode.excetion;
                result.retMsg = $"提现申请时发生异常：{ex.Message}";
                //调试日志
                Comm.WriteLog("exception", result.retMsg, Common.Enums.DebugLogLevel.Error, "Bll.VipForwardBLL.VipForwardApply");
            }

            return result;
            #endregion
        }

        /// <summary>
        /// 获取用户绑定的银行卡
        /// </summary>
        /// <param name="vUserID"></param>
        /// <returns></returns>
        public RequestResult GetBindBankAccount(string vUserID) {
            #region
            RequestResult result = new RequestResult()
            {
                retCode = ReqResultCode.failed,
                retMsg = "获取失败"
            };

            List<MBankAccount> list = new List<Models.VipForward.MBankAccount> ();
            try
            {
                List<VipForwardAccount> alist = null;
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    alist= db.VipForwardAccounts.Where(p => p.UserID == vUserID).ToList();
                }
                
                if (alist != null && alist.Count>0) {
                    alist.ForEach(bank => {
                        list.Add(new MBankAccount() {
                            BankAccountID = bank.ID,
                            bankName=bank.Bank,
                            cardNo= bank.ForwardAccount.Substring(bank.ForwardAccount.Length-4)//截取卡号后四位
                        });
                    });
                }
                result.retCode = ReqResultCode.success;
                result.retMsg = list?.Count > 0 ? "获取成功" : "尚未绑定银行卡";
                result.objectData = new
                {
                    total = list?.Count,
                    bankList = list
                };

            }
            catch (Exception ex)
            {
                result.retCode = ReqResultCode.excetion;
                result.retMsg = $"获取用户绑定的银行卡时发生异常：{ex.Message}";
                //调试日志
                Comm.WriteLog("exception", result.retMsg, Common.Enums.DebugLogLevel.Error, "Bll.VipForwardBLL.GetBindBankAccount");
            }

            return result;
            #endregion
        }
    }
}