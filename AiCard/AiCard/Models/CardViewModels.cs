﻿using AiCard.Common;
using AiCard.Common.CommModels;
using AiCard.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    public class CardCreateEditViewModel
    {

        public CardCreateEditViewModel()
        {

        }

        public string UserID { get; set; }

        public string AdminName { get; set; }

        public int? EnterpriseID { get; set; }

        public int ID { get; set; }


        [Display(Name = "企业微信ID")]
        public string WeChatEID { get; set; }

        [Display(Name = "姓名")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "头像")]
        public FileUpload Avatar { get; set; } = new FileUpload
        {
            AutoInit = true,
            Max = 5,
            Name = "Avatar",
            Server = UploadServer.QinQiu,
            Sortable = true,
            Type = Common.CommModels.FileType.Image,
        };

        [Display(Name = "座机")]
        [RegularExpression(Reg.PHONE, ErrorMessage = "{0}格式不正确")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email")]
        [RegularExpression(Reg.EMAIL, ErrorMessage = "{0}格式不正确")]
        public string Email { get; set; }

        [Display(Name = "微信号")]
        public string WeChatCode { get; set; }

        [Display(Name = "手机号")]
        [Required]
        [RegularExpression(Reg.MOBILE, ErrorMessage = "{0}格式不正确")]
        public string Mobile { get; set; }

        [Display(Name = "启用")]
        public bool Enable { get; set; }

        [Display(Name = "职称")]
        public string Position { get; set; }

        [Display(Name = "性别")]
        public Gender Gender { get; set; }

        [Display(Name = "签名")]
        public string Remark { get; set; }

        [Display(Name = "信息")]
        public string Info { get; set; }

        [Display(Name = "语音")]
        //public string Voice { get; set; }
        public FileUpload Voice { get; set; } = new FileUpload
        {
            AutoInit = true,
            Max = 5,
            Name = "Voice",
            Server = UploadServer.QinQiu,
            Sortable = true,
            Type = Common.CommModels.FileType.Sound,
        };

        [Display(Name = "视频")]
        //public string Video { get; set; }
        public FileUpload Video { get; set; } = new FileUpload
        {
            AutoInit = true,
            Max = 5,
            Name = "Video",
            Server = UploadServer.QinQiu,
            Sortable = true,
            Type = Common.CommModels.FileType.Video,
        };

        [Display(Name = "图片")]
        public FileUpload Images { get; set; } = new FileUpload
        {
            AutoInit = true,
            Max = 5,
            Name = "Images",
            Server = UploadServer.QinQiu,
            Sortable = true,
            Type = Common.CommModels.FileType.Image,
        };

        [Display(Name = "排序")]
        public int Sort { get; set; }

    }

    public class CardListViewModel
    {
        /// <summary>
        /// 名片ID
        /// </summary>
        public int CardID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>

        public string Name { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>

        public string Mobile { get; set; }

        /// <summary>
        /// 职位
        /// </summary>

        public string Email { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 公司LOGO
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsTop { get; set; }

        public DateTime? CreateDateTime { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
    }
    public class CardEditViewModel
    {
        public int CardID { get; set; }
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

        [Display(Name = "职称")]
        public string Position { get; set; }

        [Display(Name = "性别")]
        public Gender Gender { get; set; }

        [Display(Name = "签名")]
        public string Remark { get; set; }

        [Display(Name = "信息")]
        public string Info { get; set; }

        [Display(Name = "语音")]
        //public string Voice { get; set; }
        public string Voice { get; set; }

        [Display(Name = "视频")]
        //public string Video { get; set; }
        public string Video { get; set; }

        [Display(Name = "图片")]
        public string Images { get; set; }

    }


    public class CardShowModel
    {
        public int ID { get; set; }

        [Display(Name = "企业")]
        public int? EnterpriseID { get; set; }

        public string EnterpriseName { get; set; }

        [Display(Name = "用户")]
        public string UserID { get; set; }

        public string UserName { get; set; }

        [Display(Name = "企业微信ID")]
        public string WeChatEID { get; set; }

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
        public Gender Gender { get; set; }

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

        [Display(Name = "小程序分享二维码")]
        public string WeChatMiniQrCode { get; set; }

        [Display(Name = "海报")]
        public string Poster { get; set; }

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
        [Display(Name = "激活二维码")]
        public string ActiveImage { get; set; }
    }

}