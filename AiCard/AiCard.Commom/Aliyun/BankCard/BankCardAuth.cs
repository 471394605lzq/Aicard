using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static AiCard.Common.Comm;

namespace AiCard.Commom.Aliyun.BankCard
{
    /// <summary>
    /// author：lym
    /// date:2018-11-27
    /// 阿里云的银行卡信息认证接口
    /// </summary>
    public sealed class BankCardAuth
    {
        #region 数据初始化
        /// <summary>
        /// 调用地址
        /// </summary>
        private static string aliBankcardUrl = string.Empty;
        /// <summary>
        /// 自己的Appcode
        /// </summary>
        private static string aliAppcode = string.Empty;
        /// <summary>
        /// 请求方式
        /// </summary>
        private const string method = "POST";
        /// <summary>
        /// 是否返回银行信息，（YES：返回银行信息；NO：不返回银行信息）
        /// </summary>
        private const string ReturnBankInfo = "YES";
        /// <summary>
        /// 错误码信息
        /// </summary>
        public static Dictionary<string, string> ErrorCode { get; set; }

        /// <summary>
        /// 在静态构造函数中初始化获取调用地址和appcode，只会获取一次
        /// </summary>
        static BankCardAuth() {
            aliBankcardUrl = ConfigurationManager.AppSettings["aliBankcardUrl"]?.ToString();
            aliAppcode = ConfigurationManager.AppSettings["aliAppcode"]?.ToString();
            //初始化错误码
            InitErrorCode();
        }
        #endregion

        /// <summary>
        /// 银行卡4要素认证接口
        /// </summary>
        /// <param name="bankModel"></param>
        /// <returns></returns>
        public static RequestResult BankAuthenticate(BankCardModel bankModel) {
            RequestResult result = new RequestResult() {
                retCode= ReqResultCode.failed,
                retMsg="认证请求失败"
            };
            #region 验证数据
            if (bankModel ==null) {
                result.retMsg = "需验证的银行卡信息不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(bankModel.cardNo)) {
                result.retMsg = "银行卡号不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(bankModel.idNo))
            {
                result.retMsg = "身份证号码不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(bankModel.name))
            {
                result.retMsg = "开户名不能为空";
                return result;
            }
            if (string.IsNullOrEmpty(bankModel.phoneNo))
            {
                result.retMsg = "手机号不能为空";
                return result;
            }
            #endregion
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;
            try
            {
                string bodys = $"ReturnBankInfo={ReturnBankInfo}&cardNo={bankModel.cardNo}&idNo={bankModel.idNo}&name={bankModel.name}&phoneNo={bankModel.phoneNo}";
                if (aliBankcardUrl.Contains("https://"))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(aliBankcardUrl));
                }
                else
                {
                    httpRequest = (HttpWebRequest)WebRequest.Create(aliBankcardUrl);
                }
                httpRequest.Method = method;
                httpRequest.Headers.Add("Authorization", "APPCODE " + aliAppcode);
                //根据API的要求，定义相对应的Content-Type
                httpRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                if (0 < bodys.Length)
                {
                    byte[] data = Encoding.UTF8.GetBytes(bodys);
                    using (Stream stream = httpRequest.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var responseStream = httpResponse.GetResponseStream())
                using (var mstream = new MemoryStream())
                {
                    responseStream.CopyTo(mstream);
                    string message = Encoding.UTF8.GetString(mstream.ToArray());
                    BankCardAuthResult data = JsonConvert.DeserializeObject<BankCardAuthResult>(message);
                    if (data != null && data.respCode == "0000")
                    {
                        result.retCode = ReqResultCode.success;
                        result.objectData = data;
                        result.retMsg = ErrorCode[data.respCode];
                    }
                    else {
                        result.retCode = ReqResultCode.failed;
                        result.objectData = data;
                        result.retMsg = data==null?"": ErrorCode[data.respCode];
                    }
                }
            }
            catch (Exception ex)
            {

                result.retCode = ReqResultCode.excetion;
                result.retMsg = $"调用阿里云银行卡认证时发生异常：{ex.Message}";
            }

            return result;
        }

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }


        /// <summary>
        /// 初始化错误码
        /// </summary>
        private static void InitErrorCode() {
            ErrorCode = new Dictionary<string, string>() {
                {"0000","结果匹配" },
                {"0001","开户名不能为空" },
                {"0002","银行卡号格式错误" },
                {"0003","身份证号格式错误" },
                {"0004","手机号不能为空" },
                {"0005","手机号格式错误" },
                {"0006","银行卡号不能为空" },
                {"0007","身份证号不能为空" },
                {"0008","信息不匹配" }
            };
        }
    }
}
