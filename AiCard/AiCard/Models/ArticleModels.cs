using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    public class Article
    {
        public int ID { get; set; }

        [Display(Name = "标题")]
        public string Title { get; set; }

        [Display(Name = "类别")]
        public Enums.ArticleType Type { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreateDateTime { get; set; }

        [Display(Name = "发布时间")]
        public DateTime UpdateDateTime { get; set; }

        [Display(Name = "企业")]
        public int? EnterpriseID { get; set; }

        [Display(Name = "用户")]
        public string UserID { get; set; }

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
        public Enums.ArticleState State { get; set; }

        List<ArticleComment> Comments { get; set; }
    }

    public class ArticleComment
    {
        public int ID { get; set; }

        [Display(Name = "文章")]
        public int ArticleID { get; set; }


        public Article Article { get; set; }

        [Display(Name = "内容")]
        public string Content { get; set; }

        [Display(Name = "用户")]
        public string UserID { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreateDateTime { get; set; }


    }

}