using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    public class Products
    {
        public int ID { get; set; }

        [Display(Name = "类别")]
        public Enums.ProductType Type { get; set; }

        [Display(Name = "分类")]
        public int KindID { get; set; }

        public ProductKind Kind { get; set; }

        [Display(Name = "企业")]
        public int EnterpriseID { get; set; }

        [Display(Name = "名称")]
        public string Name { get; set; }

        [Display(Name = "库存")]
        public int Count { get; set; }

        [Display(Name = "总销量")]
        public int TotalSales { get; set; }

        [Display(Name = "单价")]
        public decimal Price { get; set; }

        [Display(Name = "上架")]
        public bool Release { get; set; }

        [Display(Name = "图片")]
        public string Images { get; set; }

        [Display(Name = "商品信息")]
        public string Info { get; set; }

        [Display(Name = "排序")]
        public int Sort { get; set; }
    }

    public class ProductKind
    {
        public int ID { get; set; }

        [Display(Name = "名称")]
        public string Name { get; set; }

        public int EnterpriseID { get; set; }

        [Display(Name = "排序")]
        public int Sort { get; set; }

        public List<Products> Products { get; set; }

    }
}