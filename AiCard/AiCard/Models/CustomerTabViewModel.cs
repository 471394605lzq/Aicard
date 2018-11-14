using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    public class CustomerTabViewModel
    {
        public int ID { get; set; }

        public int EnterpriseID { get; set; }
        [Display(Name = "分组名称")]
        /// <summary>
        /// 分组
        /// </summary>
        public int GroupID { get; set; }

        [Display(Name = "分组名称")]
        public string GroupName { get; set; }

        [Display(Name = "标签名称")]
        /// <summary>
        /// 文本
        /// </summary>
        public string Name { get; set; }

        [Display(Name = "排序")]
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
    }

}