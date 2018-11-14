using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.DAL.Models
{
    public class RoleGroup
    {
        public int ID { get; set; }

        [Display(Name = "名称")]
        public string Name { get; set; }

        [Display(Name = "角色")]
        public string Roles { get; set; }

        [Display(Name = "企业")]
        public int EnterpriseID { get; set; }
    }

    

    
}