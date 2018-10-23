using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    public class Card
    {
        public int ID { get; set; }

        [Display(Name = "企业")]
        public int? EnterpriseID { get; set; }

        public Enterprise Enterprise { get; set; }

        [Display(Name = "用户")]
        public string UserID { get; set; }

        public ApplicationUser User { get; set; }

        [Display(Name = "企业微信ID")]
        public string WeChatEID { get; set; }

        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Display(Name = "头像")]
        public string Avatar { get; set; }

        [Display(Name = "座机")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "微信号")]
        public string WeChatCode { get; set; }

        [Display(Name = "手机号")]
        public string Mobile { get; set; }

        [Display(Name = "启用")]
        public bool Enable { get; set; }

        [Display(Name = "职称")]
        public string Position { get; set; }

        [Display(Name = "性别")]
        public Enums.Gender Gender { get; set; }

        [Display(Name = "签名")]
        public string Remark { get; set; }

        [Display(Name = "信息")]
        public string Info { get; set; }

        [Display(Name = "语音")]
        public string Voice { get; set; }

        [Display(Name = "视频")]
        public string Video { get; set; }

        [Display(Name = "图片")]
        public string Images { get; set; }

        [Display(Name = "小程序分享二维码")]
        public string WeChatMiniQrCode { get; set; }


        public List<CardTab> CardTabs { get; set; }

    }

    public class CardTab
    {
        public int ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Display(Name = "名称")]
        public string Name { get; set; }

        /// <summary>
        /// 卡片ID
        /// </summary>
        [Display(Name = "卡片")]
        public int CardID { get; set; }

        public Card Card { get; set; }

        /// <summary>
        /// 样式
        /// </summary>
        [Display(Name = "样式")]
        public Enums.CardTabStyle Style { get; set; }

        /// <summary>
        /// 总的点击次数
        /// </summary>
        [Display(Name = "点击次数")]
        public int Count { get; set; }

    }
}