using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard.Models.VipForward
{
    /// <summary>
    /// 提现提交的参数
    /// </summary>
    public sealed class MReqVipForward
    {
        /// <summary>
        /// 收款的银行账户ID
        /// </summary>
        public int bankAccountID { get; set; }
        /// <summary>
        /// 提现金额
        /// </summary>
        public decimal forwardAmount { get; set; }
        
    }
}