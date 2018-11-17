using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCard.DAL.Models
{
    /// <summary>
    /// 员工的商品置顶
    /// <para>删除商品的时候要把对应的置顶给删了</para>
    /// </summary>
    public class CardUserProductTop
    {
        public int ID { get; set; }

        /// <summary>
        /// 员工用户ID
        /// </summary>
        [Display(Name = "员工")]
        public string UserID { get; set; }

        /// <summary>
        /// 对应的名片ID
        /// </summary>
        [Display(Name = "名片")]
        public int CardID { get; set; }


        /// <summary>
        /// 对应的商品
        /// </summary>
        [Display(Name = "商品")]
        public int ProductID { get; set; }


        /// <summary>
        /// 排序
        /// <para>小的排在前</para>
        /// </summary>
        [Display(Name = "排序")]
        public int Sort { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }
    }
}
