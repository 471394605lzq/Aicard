using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    public class UserCardTop
    {
        public int ID { get; set; }

        [Display(Name = "用户")]
        public string UserID { get; set; }

        [Display(Name = "卡片")]
        public int CardID { get; set; }

        [Display(Name = "时间")]
        public DateTime CreateDateTime { get; set; }
    }
}