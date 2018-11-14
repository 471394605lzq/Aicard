using AiCard.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
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
        public CardTabStyle Style { get; set; }

        [Display(Name = "排序")]
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
    }


}