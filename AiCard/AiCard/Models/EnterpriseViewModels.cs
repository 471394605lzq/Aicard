using AiCard.Common;
using AiCard.Common.CommModels;
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

        [Display(Name = "企业名称")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "企业logo")]
        //public string Logo { get; set; }
        public Common.CommModels.ImageResizer Logo { get; set; } = new Common.CommModels.ImageResizer("Logo", 120, 120)
        {
            AutoInit = true,
            Name = "Logo",
            Server = UploadServer.QinQiu,
        };
        [Display(Name = "小程序AppID")]
        public string AppID { get; set; }
        [Display(Name = "管理员")]
        public string AdminID { get; set; }
        [Display(Name = "用户名")]
        public string Admin { get; set; }
        [Display(Name = "剩余名片数量")]
        public int CardCount { get; set; }

        [Display(Name = "启动")]
        public bool Enable { get; set; }
        [Display(Name = "联系电话(座机)")]
        [RegularExpression(Reg.PHONE, ErrorMessage = "{0}格式不正确")]
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
        public Map Address { get; set; } = new Map()
        {
            Lat =null,
            Lng = null,
            Address=""
        };

        [Display(Name = "官网主页")]
        public string HomePage { get; set; }

        [Display(Name = "简介")]
        [DataType(DataType.MultilineText)]
        public string Info { get; set; }

        [Display(Name = "编号")]
        [Required]
        public string Code { get; set; }

        [Display(Name = "邮箱")]
        [DataType(DataType.EmailAddress, ErrorMessage = "{0}格式不正确")]
        public string Email { get; set; }

        [Display(Name = "注册时间")]
        public DateTime RegisterDateTime { get; set; }

        public double? Lat { get; set; }

        public double? Lng { get; set; }

        [Display(Name = "企业微信ID")]
        public string WeChatWorkCorpid { get; set; }

        [Display(Name = "企业微信Secret")]
        public string WeChatWorkSecret { get; set; }

    }

    public class EnterpriseShowViewModels
    {
        public int ID { get; set; }

        [Display(Name = "企业名称")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "企业logo")]
        //public string Logo { get; set; }
        public Common.CommModels.ImageResizer Logo { get; set; } = new Common.CommModels.ImageResizer("Logo", 120, 120)
        {
            AutoInit = true,
            Name = "Logo",
            Server = UploadServer.QinQiu,
        };
        [Display(Name = "小程序AppID")]
        public string AppID { get; set; }
        [Display(Name = "管理员")]
        public string AdminID { get; set; }
        [Display(Name = "用户名")]
        public string Admin { get; set; }
        [Display(Name = "剩余名片数量")]
        public int CardCount { get; set; }

        [Display(Name = "启动")]
        public bool Enable { get; set; }
        [Display(Name = "联系电话(座机)")]
        [RegularExpression(Reg.PHONE, ErrorMessage = "{0}格式不正确")]
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
        [Required]
        public string Code { get; set; }
        [Display(Name = "邮箱")]
        [DataType(DataType.EmailAddress, ErrorMessage = "{0}格式不正确")]
        public string Email { get; set; }

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