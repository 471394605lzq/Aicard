using AiCard.Common.Enums;
using AiCard.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    public class ArticleIndexViewModels
    {
        public int ArticleID { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }

        public ArticleType Type { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public string DateTimeStr { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        public string LikeCount { get; set; }

        /// <summary>
        /// 评论数
        /// </summary>
        public string CommentCount { get; set; }

        /// <summary>
        /// 分享数
        /// </summary>
        public string ShareCount { get; set; }

        /// <summary>
        /// 正文
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 封面
        /// </summary>
        public string Cover { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public string[] Images { get; set; }

        /// <summary>
        /// 点赞用户
        /// </summary>
        public string[] LikeUser { get; set; }

        public string DateTime { get; set; }

        public bool HadLike { get; set; }
        [Display(Name = "状态")]
        public string State { get; set; }
    }

    public class ArticleViewModel
    {
        public int ID { get; set; }

        [Display(Name = "标题")]
        public string Title { get; set; }

        [Display(Name = "类别")]
        public ArticleType Type { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreateDateTime { get; set; }

        [Display(Name = "发布时间")]
        public DateTime UpdateDateTime { get; set; }

        [Display(Name = "企业")]
        public int? EnterpriseID { get; set; }
        [Display(Name = "企业名称")]
        public string EnterpriseName { get; set; }

        [Display(Name = "用户")]
        public string UserID { get; set; }

        [Display(Name = "发布人")]
        public string UserName { get; set; }

        [Display(Name = "内容")]
        public string Content { get; set; }

        [Display(Name = "图片")]
        public string Images { get; set; }

        [Display(Name = "视频")]
        public string Video { get; set; }

        [Display(Name = "点赞数")]
        public int Like { get; set; }

        [Display(Name = "评论数")]
        public int Comment { get; set; }

        [Display(Name = "转发")]
        public int Share { get; set; }

        [Display(Name = "状态")]
        public ArticleState State { get; set; }

        List<ArticleComment> Comments { get; set; }
    }

    public class ArticleCreateEditViewModel
    {
        public int ID { get; set; }

        [Display(Name = "标题")]
        public string Title { get; set; }

        [Display(Name = "封面")]
        public Common.CommModels.ImageResizer Cover { get; set; } = new Common.CommModels.ImageResizer("Cover", 132, 132);

        [Display(Name = "内容")]
        public string Content { get; set; }
    }
}