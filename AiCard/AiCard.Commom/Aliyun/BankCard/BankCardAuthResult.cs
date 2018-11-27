using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCard.Commom.Aliyun.BankCard
{
    public sealed class BankCardAuthResult: BankCardModel
    {

        /// <summary>
        /// 返回消息
        /// </summary>
        public string respMessage { get; set; }
        /// <summary>
        /// 返回信息码
        /// </summary>
        public string respCode { get; set; }
        /// <summary>
        /// 银行名称
        /// </summary>
        public string bankName { get; set; }
        /// <summary>
        /// 银行卡类型
        /// </summary>
        public string bankKind { get; set; }
        /// <summary>
        /// 卡类别
        /// </summary>
        public string bankType { get; set; }
        /// <summary>
        /// 银行代码
        /// </summary>
        public string bankCode { get; set; }
    }
}
