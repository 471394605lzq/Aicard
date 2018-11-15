using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCard.DAL.Models
{
    /// <summary>
    /// 提现订单
    /// </summary>
    public class VipForwardOrder
    {
        public int ID { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 支付流水号
        /// </summary>
        public string PayCode { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 类别（银行卡、微信红包等）
        /// </summary>
        public Common.Enums.VipForwardType Type { get; set; }

        /// <summary>
        /// 公司出账的账户
        /// </summary>
        public string FromAccount { get; set; }


        /// <summary>
        /// 用户入账的帐号
        /// </summary>
        public string ToAccount { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 应收金额
        /// </summary>
        public decimal ReceivableAmount { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public Common.Enums.VipForwardState State { get; set; }

        /// <summary>
        /// 备注（例如审核失败，转账失败时候记录等）
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 支付的返回结果
        /// </summary>
        public string PayResult { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }


        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PayDateTime { get; set; }

    }

    
}
