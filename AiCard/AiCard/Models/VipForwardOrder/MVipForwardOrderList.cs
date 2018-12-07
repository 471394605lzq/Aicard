using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Models.VipForwardOrder
{
    /// <summary>
    /// 提现申请订单
    /// </summary>
    public class MVipForwardOrderList
    {
        [Display(Name = "提现订单ID")]
        public int OrderID { get; set; }

        /// <summary>
        /// 提现用户
        /// </summary>
        [Display(Name = "提现用户")]
        public string UserName { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        [Display(Name = "联系电话")]
        public string Phone { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [Display(Name = "订单号")]
        public string Code { get; set; }

        /// <summary>
        /// 支付流水号
        /// </summary>
        [Display(Name = "支付流水号")]
        public string PayCode { get; set; }

        

        /// <summary>
        /// 类别（银行卡、微信红包等）
        /// </summary>
        [Display(Name = "支付渠道")]
        public Common.Enums.VipForwardType Type { get; set; }

        
        /// <summary>
        /// 用户入账的帐号
        /// </summary>
        [Display(Name = "收款账户")]
        public string ToAccount { get; set; }

        /// <summary>
        /// 应收金额
        /// </summary>
        [Display(Name = "提现金额")]
        public decimal ReceivableAmount { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "提现状态")]
        public Common.Enums.VipForwardState State { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        [Display(Name = "实际金额")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 备注（例如审核失败，转账失败时候记录等）
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }

      

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "申请时间")]
        public DateTime CreateDateTime { get; set; }


        /// <summary>
        /// 支付时间
        /// </summary>
        [Display(Name = "支付时间")]
        public DateTime? PayDateTime { get; set; }

        /// <summary>
        /// 支付的返回结果
        /// </summary>
        [Display(Name = "支付结果")]
        public string PayResult { get; set; }
    }
}