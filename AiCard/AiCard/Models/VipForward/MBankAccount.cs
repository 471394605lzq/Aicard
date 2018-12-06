using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard.Models.VipForward
{
    /// <summary>
    /// 绑定的银行卡
    /// </summary>
    public sealed class MBankAccount
    {
        /// <summary>
        /// 提现银行账户ID
        /// </summary>
        public int BankAccountID { get; set; }

        /// <summary>
        /// 银行卡号(尾号四位)
        /// </summary>
        public string cardNo { get; set; }
        /// <summary>
        /// 银行名称
        /// </summary>
        public string bankName { get; set; }

        /// <summary>
        /// 银行logo
        /// </summary>
        public string bankLogo { get; set; }

    }
}