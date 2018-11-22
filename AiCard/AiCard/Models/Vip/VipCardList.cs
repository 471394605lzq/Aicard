using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Models.Vip
{
    /// <summary>
    /// VIP用户列表返回参数
    /// </summary>
    public class VipCardList
    {
        /// <summary>
        /// 用户账号
        /// </summary>
        [Display(Name = "用户账号")]
        public string UserName { get; set; }
        /// <summary>
        /// vip会员ID
        /// </summary>
        public int VipID { get; set; }
        /// <summary>
        /// 可提现余额
        /// </summary>
        [Display(Name = "余额")]
        public decimal Amount { get; set; }
        /// <summary>
        /// 佣金总额
        /// </summary>
        [Display(Name = "佣金总额")]
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// 二级会员数量
        /// </summary>
        [Display(Name = "二级会员数")]
        public int VipChild2ndCount { get; set; }
        /// <summary>
        /// 三级会员数量
        /// </summary>
        [Display(Name = "三级会员数")]
        public int VipChild3rdCount { get; set; }
        /// <summary>
        /// 注册用户数量
        /// </summary>
        [Display(Name = "注册用户数")]
        public int FreeChildCount { get; set; }
        [Display(Name = "状态")]
        public int State { get; set; }
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
    }
}