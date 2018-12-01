using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCard.DAL.Models
{
    /// <summary>
    /// 验证码表
    /// </summary>
    public class VerificationCode
    {
        public int ID { get; set; }

        /// <summary>
        /// 发送对象（手机/邮箱地址）
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        
        /// <summary>
        /// 请求发送的IP（部分运营商要求对IP进行控制）
        /// </summary>
        public string IP { get; set; }
    }
}
