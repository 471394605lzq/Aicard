using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace AiCard.Models
{
    public class Order
    {
        public int ID { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [Display(Name = "订单号")]
        public string Code { get; set; }

        /// <summary>
        /// 支付的订单号
        /// </summary>
        [Display(Name = "支付的订单号")]
        public string PayCode { get; set; }

        /// <summary>
        /// 支付人的用户ID
        /// </summary>
        [Display(Name = "支付人的用户ID")]
        public string UserID { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        [Display(Name = "金额")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 应收金额
        /// </summary>
        [Display(Name = "应收金额")]
        public decimal ReceivableAmount { get; set; }

        /// <summary>
        /// 支付类型
        /// </summary>
        [Display(Name = "支付类型")]
        public Common.Enums.PayType PayType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        [Display(Name = "支付时间")]
        public DateTime? PayDateTime { get; set; }
    }
}