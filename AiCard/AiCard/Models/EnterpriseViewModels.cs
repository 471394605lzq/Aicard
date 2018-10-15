using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    public class EnterpriseViewModels
    {
        public int ID { get; set; }

        [Display(Name ="企业名称")]
        public string Name { get; set; }

        [Display(Name = "管理员")]
        public string AdminID { get; set; }
        [Display(Name = "用户名")]
        public string Admin { get; set; }
        [Display(Name = "剩余名片数量")]
        public int CardCount { get; set; }

        [Display(Name = "启动")]
        public bool Enable { get; set; }
        [Display(Name = "联系电话")]
        public string PhoneNumber { get; set; }
    }
}