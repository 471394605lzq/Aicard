using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCard.DAL.Models
{
    public class VipForwardAccount
    {
        public int ID { get; set; }

        /// <summary>
        /// 收款账号类型
        /// </summary>
        public Common.Enums.VipForwardType ForwardType { get; set; }

        /// <summary>
        /// 收款的帐号
        /// </summary>
        public string ForwardAccount { get; set; }

        /// <summary>
        /// 收款的人
        /// </summary>
        public string ForwardName { get; set; }

        /// <summary>
        /// 银行
        /// </summary>
        public string Bank { get; set; }

        /// <summary>
        /// 银行编号
        /// </summary>
        public string BankCode { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// 证件号
        /// </summary>
        public string CerCode { get; set; }

        /// <summary>
        /// 绑定的手机号
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 关联的VIP
        /// </summary>
        public int VipID { get; set; }

        /// <summary>
        /// 关联的用户
        /// </summary>
        public string UserID { get; set; }
    }
}
