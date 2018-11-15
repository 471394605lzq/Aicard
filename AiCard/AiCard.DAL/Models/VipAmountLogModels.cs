using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCard.DAL.Models
{
    /// <summary>
    /// VIP的金额明细记录
    /// </summary>
    public class VipAmountLog
    {
        public int ID { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public string UserID { get; set; }


        /// <summary>
        /// 金额
        /// <para>收入为+,提现为-</para>
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 变动前用户金额
        /// <para>操作前把用户的总金额存到下来</para>
        /// </summary>
        public decimal Before { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public Common.Enums.VipAmountLogType Type { get; set; }

        /// <summary>
        /// 对象，新用户，新的子集
        /// </summary>
        public string SourceUserID { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }


    }
}
