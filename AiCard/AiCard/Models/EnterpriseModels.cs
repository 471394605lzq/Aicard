using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    public class Enterprise : IDistrict, IGprs
    {

        public int ID { get; set; }

        [Display(Name = "编号")]
        public string Code { get; set; }

        [Display(Name = "名称")]
        public string Name { get; set; }


        public string Logo { get; set; }

        [Display(Name = "简介")]
        public string Info { get; set; }

        [Display(Name = "省")]
        public string Province { get; set; }

        [Display(Name = "市")]
        public string City { get; set; }

        [Display(Name = "地区")]
        public string District { get; set; }

        [Display(Name = "详细地址")]
        public string Address { get; set; }

        [Display(Name = "联系电话")]
        public string PhoneNumber { get; set; }


        public string Email { get; set; }

        [Display(Name = "官方主页")]
        public string HomePage { get; set; }

        [Display(Name = "管理员")]
        public string AdminID { get; set; }

        [Display(Name = "小程序AppID")]
        public string AppID { get; set; }

        [Display(Name = "剩余名片数量")]
        public int CardCount { get; set; }

        [Display(Name = "启动")]
        public bool Enable { get; set; }

        [Display(Name = "注册时间")]
        public DateTime RegisterDateTime { get; set; }

        public double? Lat { get; set; }

        public double? Lng { get; set; }

        [Display(Name = "企业微信ID")]
        public string WeChatWorkCorpid { get; set; }

        [Display(Name = "企业微信Secret")]
        public string WeChatWorkSecret { get; set; }
    }
}