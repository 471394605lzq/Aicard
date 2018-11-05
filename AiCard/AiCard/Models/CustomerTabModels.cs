using System;
using System.Collections.Generic;
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
        /// 分组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 样式
        /// </summary>
        public Enums.CardTabStyle Style { get; set; }

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

        /// <summary>
        /// 分组
        /// </summary>
        public int GroupID { get; set; }

        public CustomerTabGroup Group { get; set; }

        /// <summary>
        /// 文本
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public string Sort { get; set; }
    }
}