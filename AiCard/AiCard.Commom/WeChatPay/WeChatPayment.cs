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

    }
}
