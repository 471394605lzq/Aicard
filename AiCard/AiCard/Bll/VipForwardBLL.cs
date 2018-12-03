using AiCard.Common;
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
    }
}