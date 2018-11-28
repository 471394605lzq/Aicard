using AiCard.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    public class PersonalCardViewModels : IDistrict
    {
        [Required]
        [Display(Name = "名片")]
        public int PCardID { get; set; }

        [Required]
        [Display(Name = "用户")]
        public string UserID { get; set; }

        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Display(Name = "头像")]
        public string Avatar { get; set; }

        [Display(Name = "座机")]
        [RegularExpression(Reg.PHONE, ErrorMessage = "{0}格式不正确")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email")]
        [RegularExpression(Reg.EMAIL, ErrorMessage = "{0}格式不正确")]
        public string Email { get; set; }

        [Display(Name = "微信号")]
        public string WeChatCode { get; set; }

        [Display(Name = "手机号")]
        [RegularExpression(Reg.MOBILE, ErrorMessage = "{0}格式不正确")]
        public string Mobile { get; set; }

        [Display(Name = "职位")]
        public string Position { get; set; }

        [Display(Name = "性别")]
        public Common.Enums.Gender? Gender { get; set; }

        [Display(Name = "签名")]
        public string Remark { get; set; }

        [Display(Name = "信息")]
        public string Info { get; set; }

        [Display(Name = "语音")]
        public string Voice { get; set; }

        [Display(Name = "视频")]
        public string Video { get; set; }

        [Display(Name = "图片")]
        public List<string> Images { get; set; }

        [Display(Name = "地址")]
        public string Address { get; set; }

        [Display(Name = "纬度")]
        public string Lat { get; set; }

        [Display(Name = "经度")]
        public string Lng { get; set; }

        [Display(Name = "省")]
        public string Province { get; set; }

        [Display(Name = "市")]
        public string City { get; set; }

        [Display(Name = "区")]
        public string District { get; set; }

        [Display(Name = "生日")]
        public string Birthday { get; set; }

        [Display(Name = "行业")]
        public string Industry { get; set; }

        [Display(Name = "企业")]
        public string Enterprise { get; set; }


    }
}