using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WxPayAPI;
using static AiCard.Common.Comm;

namespace AiCard.Commom.WeChatPay
{
    /// <summary>
    /// author:lym
    /// date:2018-11-16
    /// 微信支付相关业务处理类
    /// </summary>
    public sealed class WeChatPayment
    {

        /// <summary>
        /// 统一下单，返回下单结果
        /// </summary>
        /// <returns></returns>
        public RequestResult GetUnifiedOrderResult(UnifiedPayData payData)
        {
            #region
            RequestResult result = new RequestResult
            {
                retCode = ReqResultCode.failed,
                retMsg = ""
            };
            if (payData == null)
            {
                result.retMsg = "请求参数不能为空";
            }
            //统一下单
            WxPayData data = new WxPayData();
            data.SetValue("body", payData.body ?? string.Empty);
            data.SetValue("attach", payData.attach ?? string.Empty);
            data.SetValue("out_trade_no", payData.out_trade_no ?? string.Empty);
            data.SetValue("total_fee", payData.total_fee);
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss"));
            data.SetValue("time_expire", DateTime.Now.AddMinutes(10).ToString("yyyyMMddHHmmss"));
            data.SetValue("goods_tag", payData.goods_tag ?? string.Empty);
            data.SetValue("trade_type", payData.trade_type ?? "JSAPI");
            data.SetValue("openid", payData.openid ?? string.Empty);

            WxPayData ret = WxPayApi.UnifiedOrder(data);
            if (!ret.IsSet("appid") || !ret.IsSet("prepay_id") || ret.GetValue("prepay_id").ToString() == "")
            {
                Log.Error(this.GetType().ToString(), "UnifiedOrder response error!");
                throw new WxPayException("UnifiedOrder response error!");
            }
            else
            {
                result.retCode = ReqResultCode.success;
                result.retMsg = "请求成功";
                result.objectData = ret;//把结果返回到业务层
            }

            return result;
            #endregion
        }

        /// <summary>
        /// 微信退款申请
        /// </summary>
        /// <param name="refundData">退款申请参数</param>
        /// <returns></returns>
        public RequestResult RefundApply(RefundApplyData refundData) {
            #region
            RequestResult result = new RequestResult
            {
                retCode = ReqResultCode.failed,
                retMsg = ""
            };
            if (refundData == null)
            {
                result.retMsg = "请求参数不能为空";
            }
            try
            {
                Log.Info("Refund", "Refund is processing...");

                WxPayData data = new WxPayData();
                if (!string.IsNullOrEmpty(refundData.transaction_id))//微信订单号存在的条件下，则已微信订单号为准
                {
                    data.SetValue("transaction_id", refundData.transaction_id);
                }
                else//微信订单号不存在，才根据商户订单号去退款
                {
                    data.SetValue("out_trade_no", refundData.out_trade_no);
                }

                data.SetValue("total_fee", int.Parse(refundData.total_fee));//订单总金额
                data.SetValue("refund_fee", int.Parse(refundData.refund_fee));//退款金额
                data.SetValue("out_refund_no", refundData.out_refund_no);//随机生成商户退款单号
                data.SetValue("op_user_id", WxPayConfig.GetConfig().GetMchID());//操作员，默认为商户号

                WxPayData ret = WxPayApi.Refund(data);//提交退款申请给API，接收返回数据
                Log.Info("Refund", "Refund process complete, result : " + ret.ToXml());
                if (ret.GetValue("return_code").ToString()== "SUCCESS" && ret.GetValue("result_code").ToString() == "SUCCESS")
                {
                    result.retCode = ReqResultCode.success;
                    result.retMsg = "请求成功";
                    result.objectData = ret;//把结果返回到业务层
                }
                else
                {
                    Log.Error(this.GetType().ToString(), "微信退款申请失败!");
                    result.retMsg = "微信退款申请失败";
                }
            }
            catch (Exception ex)
            {
                Log.Error(this.GetType().ToString(), $"微信退款申请发生异常：{ex.Message}");
                throw new WxPayException($"微信退款申请发生异常：{ex.Message}");
            }
            
            return result;
            #endregion

        }
    }
}
