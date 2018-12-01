using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard.Models.VipForward
{
    /// <summary>
    /// 申请提现的参数
    /// </summary>
    public sealed class MReqBankForward
    {
        /// <summary>
        /// 提现入账账户ID
        /// </summary>
        public int bankAccountID { get; set; }
        /// <summary>
        /// 提现金额
        /// </summary>
        public decimal forwardAmount { get; set; }
    }
}