using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Models.Vip
{
    /// <summary>
    /// vip用户账户收支明细
    /// </summary>
    public sealed class MVipAccountDetail
    {
        /// <summary>
        /// 时间
        /// </summary>
        [Display(Name = "操作时间")]
        public DateTime CreateDateTime { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        [Display(Name = "操作类型")]
        public Common.Enums.VipAmountLogType Type { get; set; }
        /// <summary>
        /// 金额
        /// <para>收入为+,提现为-</para>
        /// </summary>
        [Display(Name = "变更金额")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 变动前用户金额
        /// <para>操作前把用户的总金额存到下来</para>
        /// </summary>
        [Display(Name = "变更前金额")]
        public decimal Before { get; set; }

        /// <summary>
        /// 来源的用户（子用户）
        /// </summary>
        [Display(Name = "来源用户")]
        public string SourceUserName { get; set; }


    }
}