using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    public class HomePageModular
    {
        public int ID { get; set; }

        /// <summary>
        /// 企业ID
        /// </summary>
        [Display(Name = "企业ID")]
        public int EnterpriseID { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Display(Name = "标题")]
        public string Title { get; set; }

        /// <summary>
        /// 模块类别   
        /// </summary>
        [Display(Name = "模块类别")]
        public Enums.HomePageModularType Type { get; set; }

        /// <summary>
        /// 排序   
        /// </summary>
        [Display(Name = "排序")]
        public int Sort { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Display(Name = "内容")]
        public string Content { get; set; }


    }
}