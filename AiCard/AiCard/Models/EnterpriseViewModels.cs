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
        //省份
        [Display(Name = "省份")]
        public string Province { get; set; }
        //城市
        [Display(Name = "城市")]
        public string City { get; set; }
        //地区
        [Display(Name = "地区")]
        public string District { get; set; }
        //详细地址
        [Display(Name = "详细地址")]
        public string Address { get; set; }
        [Display(Name = "官网主页")]
        public string HomePage { get; set; }
        [Display(Name = "简介")]
        public string Info { get; set; }
        [Display(Name = "编号")]
        public string Code { get; set; }
        [Display(Name = "邮箱")]
        public string Email { get; set; }

    }
}