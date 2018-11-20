using AiCard.Commom.WeChatPay;
using AiCard.Common;
using AiCard.Common.WeChat;
using AiCard.DAL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using WxPayAPI;
using static AiCard.Common.Comm;

namespace AiCard.Bll
{
    /// <summary>
    /// author：lym
    /// date:2018-11-16
    /// 订单相关业务层
    /// </summary>
    public sealed class OrderBLL
    {
        Random rand = new Random();
        WeChatPayment payment = new WeChatPayment();
        
        /// <summary>
        /// 创建订单号
        /// </summary>
        /// <returns></returns>
        public string CreateOrderCode(string UserID)
        {
            #region
            string result = string.Empty;
            try
            {
                DateTime now = DateTime.Now;
                result = now.ToString("yyyyMMddHHmmssfff") + UserID.ToString().Substring(0, 8) + rand.Next(1, 99999).ToString().PadLeft(5, '1');
            }
            catch (Exception ex)
            {
                Comm.WriteLog("exception", ex.Message, Common.Enums.DebugLogLevel.Error, "Bll.OrderBLL.CreateOrderCode");
            }
            return result;
            #endregion
        }

        /// <summary>
        /// 创建升级支付订单
        /// </summary>
        /// <param name="code"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public object CreateUpGradeOrder(string code, string UserID)
        {
            //1.调用小程序登录API，获取openID
            WeChatMinApi miniApi = new WeChatMinApi(ConfigMini.AppID, ConfigMini.AppSecret);
            Jscode2sessionResult openIDResule = miniApi.Jscode2session(code);
            if (openIDResule == null || string.IsNullOrWhiteSpace(openIDResule.OpenID))
            {
                return new { retCode = "Error", retMsg = "无法获取openID,请确认code是否正确", objectData = "" };
            }

            string OrderCode = CreateOrderCode(UserID);//创建订单号
            decimal Amount = Comm.UpGradeAmount();//升级费用
            if (string.IsNullOrEmpty(OrderCode))
            {
                return new { retCode = "Error", retMsg = "订单号生成失败", objectData = "" };
            }
            //2.调用支付统一下单API
            #region 调用支付统一下单API
            UnifiedPayData payData = new UnifiedPayData()
            {
                attach = string.Empty,
                body = "个人升级成为VIP，提交支付费用",
                goods_tag = string.Empty,
                openid = openIDResule.OpenID,
                out_trade_no = OrderCode,
                //total_fee = (int)Amount * 100,
                total_fee = 1,//测试10分订单
                trade_type = "JSAPI"

            };
            
            RequestResult payResult = payment.GetUnifiedOrderResult(payData);
            WxPayData payreturnData = payResult.objectData;
            if (payResult.retCode != ReqResultCode.success || payreturnData == null)
            {
                return new { retCode = "Error", retMsg = "请求微信支付统一下单失败", objectData = "" };
            }
            #endregion

            //3.生成商户订单
            #region 生成商户订单
            int rows = 0;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //2.生成商户订单
                Order order = new Order()
                {
                    Amount = 0,
                    Code = OrderCode,
                    ReceivableAmount = Amount,
                    State = Common.Enums.OrderState.UnHandle,
                    Channel = Common.Enums.PayChannel.WxPay,
                    Type = Common.Enums.OrderType.Receivable,
                    UserID = UserID,
                    CreateDateTime = DateTime.Now,
                    PayCode=string.Empty,
                    PayInput = JsonConvert.SerializeObject(payData)
                };
                db.Orders.Add(order);
                rows = db.SaveChanges();
            }
            if (rows <= 0)
            {
                return new { retCode = "Error", retMsg = "保存订单数据失败", objectData = "" };
            }
            #endregion
            //4.返回支付参数:5个参数，生成签名再返回
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            long ts = (long)(DateTime.Now - startTime).TotalSeconds; // 相差秒数

            System.Text.StringBuilder paySignpar = new System.Text.StringBuilder();
            paySignpar.Append($"appId={payreturnData.GetValue("appid")?.ToString()}");
            paySignpar.Append($"&nonceStr={payreturnData.GetValue("nonce_str")?.ToString()}");
            paySignpar.Append($"&package=prepay_id={payreturnData.GetValue("prepay_id")?.ToString()}");
            paySignpar.Append($"&signType=MD5");
            paySignpar.Append($"&timeStamp={ts.ToString()}");
            paySignpar.Append($"&key={ConfigurationManager.AppSettings["wxPayKey"] ?? string.Empty}");
            string strPaySignpar = paySignpar.ToString();

            var sign = GetMd5Hash(strPaySignpar).ToUpper();
            dynamic retModel = new
            {
                timeStamp = ts.ToString(),
                nonceStr = payreturnData.GetValue("nonce_str")?.ToString(),
                package = "prepay_id=" + payreturnData.GetValue("prepay_id")?.ToString(),
                signType = "MD5",
                paySign = sign,
            };
            return new { retCode = "Success", retMsg = "成功", objectData = retModel };
        }

        /// <summary>
        /// 退款申请
        /// </summary>
        /// <param name="orderCode">商户订单号</param>
        /// <param name="UserID">用户ID</param>
        /// <returns></returns>
        public object RefundApply(string orderCode, string UserID) {
            if (string.IsNullOrEmpty(orderCode) || string.IsNullOrEmpty(UserID))
            {
                return new { retCode = "Error", retMsg = "商户订单号和用户ID均不能为空", objectData = "" };
            }
            int rows = 0;
            WxPayData refundreturnData = null;
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    //1.查询本地订单
                    Order order = db.Orders.FirstOrDefault(p => p.Code == orderCode);
                    if (order != null)
                    {
                        if (order.UserID.Equals(UserID))
                        {
                            if (order.State == Common.Enums.OrderState.Success)
                            {
                                string OrderCode = CreateOrderCode(UserID);//创建退款订单号
                                //退款参数
                                RefundApplyData refundData = new RefundApplyData()
                                {
                                    out_refund_no = OrderCode,
                                    out_trade_no = order.Code,
                                    refund_fee = ((int)(order.Amount * 100)).ToString(),
                                    total_fee = ((int)(order.Amount * 100)).ToString(),
                                    transaction_id = order.PayCode
                                };
                                //2.创建本地退款订单
                                Order refund = new Order()
                                {
                                    Channel = Common.Enums.PayChannel.WxPay,
                                    Code = OrderCode,
                                    CreateDateTime = DateTime.Now,
                                    PayCode = string.Empty,
                                    PayInput = JsonConvert.SerializeObject(refundData),
                                    ReceivableAmount = order.Amount,
                                    State = Common.Enums.OrderState.UnHandle,
                                    Type = Common.Enums.OrderType.Refund,
                                    UserID = UserID
                                };
                                db.Orders.Add(refund);
                                rows = db.SaveChanges();
                                if (rows > 0)
                                {
                                    //3.微信退款申请
                                    RequestResult payResult = payment.RefundApply(refundData);
                                    refundreturnData = payResult.objectData;
                                    if (payResult.retCode != ReqResultCode.success || refundreturnData == null)
                                    {
                                        return new { retCode = "Error", retMsg = "请求微信退款失败", objectData = "" };
                                    }
                                    //4.更新订单状态
                                    refund.PayCode = refundreturnData.GetValue("refund_id").ToString();//微信退款单号
                                    refund.State = Common.Enums.OrderState.Success;
                                    refund.Amount = Convert.ToDecimal(refundreturnData.GetValue("refund_fee").ToString()) / 100m;
                                    refund.PayDateTime = DateTime.Now;
                                    refund.PayResult = refundreturnData.ToJson();
                                    rows = db.SaveChanges();
                                }
                            }
                            else
                            {
                                return new { retCode = "Error", retMsg = "订单未支付成功，不能退款", objectData = "" };
                            }
                        }
                        else
                        {
                            return new { retCode = "Error", retMsg = "此订单不是当前用户的订单", objectData = "" };
                        }
                    }
                    else
                    {
                        return new { retCode = "Error", retMsg = "订单不存在", objectData = "" };
                    }
                }
            }
            catch (Exception ex)
            {

                return new { retCode = "Error", retMsg = $"申请微信退款发生异常：{ex.Message}", objectData = "" };
            }
           
            return new { retCode = "Success", retMsg = "成功", objectData = refundreturnData };
        }
    }
}