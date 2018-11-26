using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Models.Order
{
    /// <summary>
    /// 订单列表
    /// </summary>
    public sealed class MOrderInfo
    {
        public int OrderID { get; set; }

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
        [Display(Name = "订单类别")]
        public Common.Enums.OrderType Type { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        [Display(Name = "订单状态")]
        public Common.Enums.OrderState State { get; set; }

        /// <summary>
        /// 支付的订单号
        /// </summary>
        [Display(Name = "微信订单号")]
        public string PayCode { get; set; }

        /// <summary>
        /// 操作用户
        /// </summary>
        [Display(Name = "姓名")]
        public string UserName { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Display(Name = "手机号")]
        public string Mobile { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        [Display(Name = "实收金额")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 应收金额
        /// </summary>
        [Display(Name = "应收金额")]
        public decimal ReceivableAmount { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [Display(Name = "提交时间")]
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        [Display(Name = "处理时间")]
        public DateTime? PayDateTime { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        [Display(Name = "请求参数")]
        public string PayInput { get; set; }

        /// <summary>
        /// 请求结果
        /// </summary>
        [Display(Name = "请求结果")]
        public string PayResult { get; set; }

    }
}