using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard.Models.VipForward
{
    /// <summary>
    /// 绑定银行卡参数
    /// </summary>
    public sealed class MReqBankAccount
    {
        /// <summary>
        /// 银行名称
        /// </summary>
        public string bankName { get; set; }
        
        /// <summary>
        /// 银行代码
        /// </summary>
        public string bankCode { get; set; }
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string cardNo { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string idNo { get; set; }
        /// <summary>
        /// 开户名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string phoneNo { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string sign { get; set; }

    }
}