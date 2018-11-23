using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Models.Vip
{
    /// <summary>
    /// VIP用户详情返回参数
    /// </summary>
    public class VipCardDetail
    {
        [Display(Name = "公司")]
        public string Enterprise { get; set; }
        /// <summary>
        /// 用户账号
        /// </summary>
        [Display(Name = "用户账号")]
        public string UserName { get; set; }
        
       
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

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "微信号")]
        public string WeChatCode { get; set; }
        [Display(Name = "职称")]
        public string Position { get; set; }

        [Display(Name = "签名")]
        public string Remark { get; set; }

        [Display(Name = "信息")]
        public string Info { get; set; }

        [Display(Name = "语音")]
        public string Voice { get; set; }

        [Display(Name = "视频")]
        public string Video { get; set; }

        [Display(Name = "图片")]
        public string Images { get; set; }

        /// <summary>
        /// 小程序分享二维码
        /// </summary>
        [Display(Name = "小程序分享二维码")]
        public string WeChatMiniQrCode { get; set; }

        /// <summary>
        /// 海报
        /// </summary>
        [Display(Name = "海报")]
        public string Poster { get; set; }

        /// <summary>
        /// 行业
        /// </summary>
        [Display(Name = "行业")]
        public string Industry { get; set; }


        [Display(Name = "点赞数")]
        public int Like { get; set; }

        [Display(Name = "阅览数")]
        public int View { get; set; }
    }
}