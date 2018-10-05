using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace AiCard.Models
{
    public class UserManageCreateViewModel
    {
        [Required]
        [Display(Name = "帐号")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "密码")]
        [DataType(DataType.Password)]
        [StringLength(maximumLength: 99, MinimumLength = 6, ErrorMessage = "{0}最少{2}位")]
        public string Password { get; set; }


        [Display(Name = "角色")]
        public int? RoleGroupID { get; set; }

    }


    public class UserManageEditViewModel
    {
        public string ID { get; set; }

        [Required]
        [Display(Name = "帐号")]
        public string Name { get; set; }

        [Display(Name = "修改密码")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Display(Name = "角色")]
        public int? RoleGroupID { get; set; }

        [Display(Name = "角色")]
        public string RoleGroup { get; set; }

    }

    public class UserManageIndexViewModel
    {
        public string ID { get; set; }

        public string Name { get; set; }


        public string Role { get; set; }
        


    }



}