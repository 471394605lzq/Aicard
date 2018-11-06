using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Models
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
        public Enums.CardTabStyle Style { get; set; }

        [Display(Name = "排序")]
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        public List<CustomerTab> Tabs { get; set; }

    }
    public class CustomerTabGroupViewModel
    {
        public int ID { get; set; }

        /// <summary>
        /// 企业
        /// </summary>
        public int EnterpriseID { get; set; }

        [Display(Name = "企业名称")]
        public string EnterpriseName { get; set; }

        [Display(Name = "分组名称")]
        /// <summary>
        /// 分组名称
        /// </summary>
        public string Name { get; set; }

        [Display(Name = "样式")]
        /// <summary>
        /// 样式
        /// </summary>
        public Enums.CardTabStyle Style { get; set; }

        [Display(Name = "排序")]
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
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