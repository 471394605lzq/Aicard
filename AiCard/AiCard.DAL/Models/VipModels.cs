using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCard.DAL.Models
{
    /// <summary>
    /// 个人版名片扩展
    /// </summary>
    public class Vip
    {
        public int ID { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 名片
        /// </summary>
        public int CardID { get; set; }

        /// <summary>
        /// 可提现
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 父的VIPID
        /// </summary>
        public int? ParentID { get; set; }

        /// <summary>
        /// 父的UserID
        /// </summary>
        public string ParentUserID { get; set; }

        /// <summary>
        /// 用户类别
        /// </summary>
        public Common.Enums.VipRank Type { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public Common.Enums.VipState State { get; set; }

        /// <summary>
        /// 分享码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }
    }
}
