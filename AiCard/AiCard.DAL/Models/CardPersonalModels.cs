using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCard.DAL.Models
{
    public class CardPersonal : Common.IDistrict, Common.IGprs, Common.ICard
    {
        public int ID { get; set; }


        [Display(Name = "公司")]
        public string Enterprise { get; set; }

        [Display(Name = "用户")]
        public string UserID { get; set; }

        public ApplicationUser User { get; set; }


        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Display(Name = "头像")]
        public string Avatar { get; set; }

        [Display(Name = "座机")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "微信号")]
        public string WeChatCode { get; set; }

        [Display(Name = "手机号")]
        public string Mobile { get; set; }

        [Display(Name = "启用")]
        public bool Enable { get; set; }

        [Display(Name = "职称")]
        public string Position { get; set; }

        [Display(Name = "性别")]
        public Common.Enums.Gender Gender { get; set; }

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


        [Display(Name = "点赞")]
        public int Like { get; set; }

        [Display(Name = "阅览数")]
        public int View { get; set; }
        
        [Display(Name = "生日")]
        public DateTime? Birthday { get; set; }

        [Display(Name = "排序")]
        public int Sort { get; set; }

        [Display(Name = "省")]
        public string Province { get; set; }

        [Display(Name = "市")]
        public string City { get; set; }

        [Display(Name = "区")]
        public string District { get; set; }

        [Display(Name = "维度")]
        public double? Lat { get; set; }

        [Display(Name = "经度")]
        public double? Lng { get; set; }

        [Display(Name = "详细地址")]
        public string Address { get; set; }
    }
}
