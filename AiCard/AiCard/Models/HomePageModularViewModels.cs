using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using AiCard.Enums;

namespace AiCard.Models
{

    public class HomePageModularByHtml
    {
        public int ID { get; set; }

        [Display(Name = "标题")]
        public string Title { get; set; }


        [Display(Name = "内容")]
        [AllowHtml]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        [Display(Name = "类别")]
        public HomePageModularType Type { get { return HomePageModularType.Html; } }


    }

    public class HomePageModularByImage
    {

        public int ID { get; set; }

        [Display(Name = "标题")]
        public string Title { get; set; }

        [Display(Name = "图片")]
        public CommModels.FileUpload Images { get; set; } = new CommModels.FileUpload
        {
            AutoInit = true,
            Max = 50,
            Name = "Images",
            Server = CommModels.UploadServer.QinQiu,
            Sortable = true,
        };

        [Display(Name = "类型")]
        public HomePageModularType Type { get { return HomePageModularType.Images; } }

    }
}