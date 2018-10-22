using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public string Name { get; set; }

        [Display(Name = "头像")]
        public Models.CommModels.FileUpload Avatar { get; set; } = new CommModels.FileUpload
        {
            AutoInit = true,
            Max = 5,
            Name = "Avatar",
            Server = CommModels.UploadServer.QinQiu,
            Sortable = true,
            Type = CommModels.FileType.Image,
        };

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
        public Enums.Gender Gender { get; set; }

        [Display(Name = "签名")]
        public string Remark { get; set; }

        [Display(Name = "信息")]
        public string Info { get; set; }

        [Display(Name = "语音")]
        public string Voice { get; set; }

        [Display(Name = "视频")]
        public string Video { get; set; }

        [Display(Name = "图片")]
        public Models.CommModels.FileUpload Images { get; set; } = new CommModels.FileUpload
        {
            AutoInit = true,
            Max = 5,
            Name = "Images",
            Server = CommModels.UploadServer.QinQiu,
            Sortable = true,
            Type = CommModels.FileType.Image,
        };
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
    }

    public class CardDetailViewModel : IGprs
    {
        /// <summary>
        /// 名片ID
        /// </summary>
        public int CardID { get; set; }

        /// <summary>
        /// 企业ID
        /// </summary>
        public int EnterpriseID { get; set; }

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
        public List<string> Avatar { get; set; } = new List<string>();

        /// <summary>
        /// 公司LOGO
        /// </summary>
        public string Logo { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string EnterpriseName { get; set; }

        /// <summary>
        /// 公司地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 座机
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 微信号
        /// </summary>
        public string WeChatCode { get; set; }
        
        /// <summary>
        /// 纬度
        /// </summary>
        public double? Lat { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double? Lng { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 语音
        /// </summary>
        public string Voice { get; set; }

        /// <summary>
        /// 视频
        /// </summary>
        public string Video { get; set; }

        /// <summary>
        /// 我的照片
        /// </summary>
        public List<string> Images { get; set; } = new List<string>();

        /// <summary>
        /// 最近的查看
        /// </summary>
        public List<string> Viewers { get; set; }

        /// <summary>
        /// 靠谱
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// 人气
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// 是否已经点赞过
        /// </summary>
        public bool HadLike { get; set; }

        /// <summary>
        /// 未读消息数量
        /// </summary>
        public int NoReadCount { get; set; }
    }

}