using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    public class ArticleIndexViewModels
    {
        public int ID { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }


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
        public string LikeUser { get; set; }

        public DateTime DateTime { get; set; }
    }
}