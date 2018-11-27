using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCard.Commom.Aliyun.BankCard
{
    /// <summary>
    /// 银行卡基础信息
    /// </summary>
    public  class BankCardModel
    {
       
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string cardNo { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string idNo { get; set; }
        /// <summary>
        /// 开户名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string phoneNo { get; set; }
    }
    
}
