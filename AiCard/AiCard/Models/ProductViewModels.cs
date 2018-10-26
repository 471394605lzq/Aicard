using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    public class ProductViewModels
    {
        public int ID { get; set; }

        [Display(Name = "类别")]
        public Enums.ProductType Type { get; set; }

        [Display(Name = "分类")]
        [Required]
        public int KindID { get; set; }
        [Display(Name = "分类")]
        public ProductKind Kind { get; set; }
        [Display(Name = "分类名称")]
        public string KindName { get; set; }
        [Display(Name = "企业")]
        public int EnterpriseID { get; set; }
        [Display(Name = "企业名称")]
        public string EnterpriseName { get; set; }
        [Display(Name = "名称")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "库存")]
        [Required]
        public int Count { get; set; }

        [Display(Name = "总销量")]
        public int TotalSales { get; set; }

        [Display(Name = "销售单价")]
        [Required]
        public decimal Price { get; set; }

        [Display(Name = "原价")]
        [Required]
        public decimal OriginalPrice { get; set; }

        [Display(Name = "上架")]
        public bool Release { get; set; }

        [Display(Name = "图片")]
        //public string Images { get; set; }
        public Models.CommModels.FileUpload Images { get; set; } = new CommModels.FileUpload
        {
            AutoInit = true,
            Max = 5,
            Name = "Images",
            Server = CommModels.UploadServer.QinQiu,
            Sortable = true,
            Type = CommModels.FileType.Image,
        };

        [Display(Name = "商品信息")]
        public string Info { get; set; }

        [Display(Name = "排序")]
        public int Sort { get; set; }
    }

    public class ProductIndexViewModels
    {
        public int ID { get; set; }

        [Display(Name = "类别")]
        public Enums.ProductType Type { get; set; }

        [Display(Name = "分类")]
        public int KindID { get; set; }
        [Display(Name = "分类")]
        public ProductKind Kind { get; set; }
        [Display(Name = "分类名称")]
        public string KindName { get; set; }
        [Display(Name = "企业")]
        public int EnterpriseID { get; set; }
        [Display(Name = "企业名称")]
        public string EnterpriseName { get; set; }
        [Display(Name = "名称")]
        public string Name { get; set; }

        [Display(Name = "库存")]
        public int Count { get; set; }

        [Display(Name = "总销量")]
        public int TotalSales { get; set; }

        [Display(Name = "销售单价")]
        public decimal Price { get; set; }
        [Display(Name = "原价")]
        public decimal OriginalPrice { get; set; }

        [Display(Name = "上架")]
        public bool Release { get; set; }

        [Display(Name = "图片")]
        //public string Images { get; set; }
        public string Images { get; set; }

        [Display(Name = "商品信息")]
        public string Info { get; set; }

        [Display(Name = "排序")]
        public int Sort { get; set; }
    }
}