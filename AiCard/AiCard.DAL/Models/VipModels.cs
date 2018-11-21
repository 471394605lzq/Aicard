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
        /// 可提金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 总盈利金额
        /// </summary>
        public decimal TotalAmount { get; set; }


        /// <summary>
        /// 2级VIP用户的数量
        /// </summary>
        public int VipChild2ndCount { get; set; }

        /// <summary>
        /// 3级VIP用户的数量
        /// </summary>
        public int VipChild3rdCount { get; set; }

        /// <summary>
        /// 免费用户的数量
        /// </summary>
        public int FreeChildCount { get; set; }

        /// <summary>
        /// 用户类别
        /// </summary>
        public Common.Enums.VipRank Type { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public Common.Enums.VipState State { get; set; }

        /// <summary>
        /// 分享码（升级VIP后才有，免费用户是null）
        /// <para>六位随机数（数字+26大写字母）</para>
        /// <para>匹配的时候时候不区分大小写</para>
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 收入排名（全部）
        /// <para>0不进入排名</para>
        /// </summary>
        public int TotalAmountRank { get; set; }

        /// <summary>
        /// 收入排名（周榜）
        /// <para>上一周日到周六</para>
        /// </summary>
        public int TotalWeekAmountRank { get; set; }

        /// <summary>
        /// 收入排名（月榜）
        /// <para>上一个月1号到上个月底</para>
        /// </summary>
        public int TotalMonthAmountRank { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }
    }
}
