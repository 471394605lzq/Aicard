using AiCard.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.DAL.Models
{
    /// <summary>
    /// 预设用户标签分组
    /// </summary>
    public class CustomerTabGroup
    {
        public int ID { get; set; }

        /// <summary>
        /// 企业
        /// </summary>
        public int EnterpriseID { get; set; }

        [Display(Name ="分组名称")]
        /// <summary>
        /// 分组名称
        /// </summary>
        public string Name { get; set; }

        [Display(Name = "样式")]
        /// <summary>
        /// 样式
        /// </summary>
        public CardTabStyle Style { get; set; }

        [Display(Name = "排序")]
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        public List<CustomerTab> Tabs { get; set; }

    }
   
    /// <summary>
    /// 预设用户标签
    /// </summary>
    public class CustomerTab
    {
        public int ID { get; set; }

        [Display(Name = "分组名称")]
        /// <summary>
        /// 分组
        /// </summary>
        public int GroupID { get; set; }

        [Display(Name ="分组名称")]
        public CustomerTabGroup Group { get; set; }

        /// <summary>
        /// 文本
        /// </summary>
        [Display(Name ="标签名称")]
        public string Name { get; set; }

        [Display(Name = "排序")]
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
    }

   
}