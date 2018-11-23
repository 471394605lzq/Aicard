using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Models.Vip
{
    /// <summary>
    /// vip用户下级用户信息
    /// </summary>
    public class VipChildList
    {
        /// <summary>
        /// 时间
        /// </summary>
        [Display(Name = "操作时间")]
        public DateTime CreateDateTime { get; set; }
        /// <summary>
        /// 会员类型
        /// </summary>
        [Display(Name = "会员类型")]
        public Common.Enums.VipRank Type { get; set; }

        [Display(Name = "公司")]
        public string Enterprise { get; set; }
       

        [Display(Name = "性别")]
        public string Gender { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        public string Name { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        [Display(Name = "头像")]
        public string Avatar { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [Display(Name = "手机号")]
        public string Mobile { get; set; }

        [Display(Name = "座机")]
        public string PhoneNumber { get; set; }
    }
}