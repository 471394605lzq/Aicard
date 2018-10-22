using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    public class ProductKindsViewModels
    {
        public int ID { get; set; }

        [Display(Name = "名称")]
        public string Name { get; set; }
        [Display(Name = "企业名称")]
        public int EnterpriseID { get; set; }
        [Display(Name = "企业名称")]
        public string EnterpriseName { get; set; }

        [Display(Name = "排序")]
        public int Sort { get; set; }

        public List<Product> Products { get; set; }

    }
}