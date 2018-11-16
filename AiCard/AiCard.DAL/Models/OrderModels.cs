using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace AiCard.DAL.Models
{
    /// <summary>
    /// 订单
    /// </summary>
    public class Order
    {
        public int ID { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [Display(Name = "订单号")]
        public string Code { get; set; }

        /// <summary>
        /// 支付渠道
        /// </summary>
        [Display(Name = "支付渠道")]
        public Common.Enums.PayChannel Channel { get; set; }

        /// <summary>
        /// 订单类别
        /// </summary>
        public Common.Enums.OrderType Type { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        [Display(Name = "订单状态")]
        public Common.Enums.OrderState State { get; set; }

        /// <summary>
        /// 支付的订单号
        /// </summary>
        [Display(Name = "支付的订单号")]
        public string PayCode { get; set; }

        /// <summary>
        /// 支付人的用户ID
        /// </summary>
        [Display(Name = "支付人的用户ID")]
        public int UserID { get; set; }

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
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        [Display(Name = "支付时间")]
        public DateTime? PayDateTime { get; set; }

        /// <summary>
        /// 传入参数
        /// </summary>
        public string PayInput { get; set; }

        /// <summary>
        /// 支付的结果
        /// </summary>
        public string PayResult { get; set; }


    }
}