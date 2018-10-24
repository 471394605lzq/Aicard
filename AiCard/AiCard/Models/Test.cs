using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    public class Test
    {
        [Display(Name ="区域")]
        public Models.CommModels.ChinaPCASCom p { get; set; } = new CommModels.ChinaPCASCom("北京市","东城区")
        {
        };
    }
}