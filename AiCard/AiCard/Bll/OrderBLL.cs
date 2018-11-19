﻿using AiCard.Commom.WeChatPay;
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
            WeChatPayment payment = new WeChatPayment();
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
                    Amount = Amount,
                    Code = OrderCode,
                    ReceivableAmount = Amount,
                    State = Common.Enums.OrderState.UnHandle,
                    Channel = Common.Enums.PayChannel.WxPay,
                    Type = Common.Enums.OrderType.Receivable,
                    UserID = UserID,
                    CreateDateTime = DateTime.Now,
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
            //paySignpar += $"&nonceStr={payreturnData.GetValue("nonce_str")?.ToString()}";
            //paySignpar += $"&package=prepay_id={payreturnData.GetValue("prepay_id")?.ToString()}";
            //paySignpar += $"&signType=MD5";
            //paySignpar += $"&timeStamp={ts}";
            //paySignpar += $"&key={ConfigurationManager.AppSettings["wxPayKey"] ?? string.Empty}";
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


    }
}