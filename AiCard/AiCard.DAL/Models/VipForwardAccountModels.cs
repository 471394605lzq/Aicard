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
        /// 收款的帐号
        /// </summary>
        public string ForwardName { get; set; }

        /// <summary>
        /// 银行
        /// </summary>
        public string Bank { get; set; }
    }
}
