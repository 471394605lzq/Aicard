﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCard.Commom.WeChatPay
{
    /// <summary>
    /// 微信支付提交参数及返回参数模型
    /// 退款等参数实体
    /// </summary>
    public sealed  class WeChatPayModel
    {
    }

    /// <summary>
    /// 统一下单参数
    /// </summary>
    public sealed class UnifiedPayData {
        /// <summary>
        /// 用户标识
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 商品描述
        /// </summary>
        public string body { get; set; }
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string out_trade_no { get; set; }
        /// <summary>
        /// 订单金额(分)
        /// </summary>
        public int total_fee { get; set; }
        /// <summary>
        /// 附加数据
        /// </summary>
        public string attach { get; set; }
        /// <summary>
        /// 交易类型
        /// </summary>
        public string trade_type { get; set; }
        /// <summary>
        /// 订单优惠标记
        /// </summary>
        public string goods_tag { get; set; }
    }


    /// <summary>
    /// 退款申请参数
    /// </summary>
    public sealed class RefundApplyData {
        /// <summary>
        /// 微信订单号
        /// </summary>
        public string transaction_id { get; set; }
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string out_trade_no { get; set; }
        /// <summary>
        /// 订单总金额
        /// </summary>
        public string total_fee { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        public string refund_fee { get; set; }
        /// <summary>
        /// 商户退款单号
        /// </summary>
        public string out_refund_no { get; set; }
    }
   
}
