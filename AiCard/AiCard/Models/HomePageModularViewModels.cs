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
    /// <summary>
    /// 公司主页模块接口
    /// </summary>
    public interface IHomePageModular
    {
        int ID { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// 模块类别   
        /// </summary>
        Enums.HomePageModularType Type { get; }

        /// <summary>
        /// 内容
        /// </summary>
        string Content { get; set; }
    }

    public class HomePageModularByHtml : IHomePageModular
    {
        public int ID { get; set; }

        public string Title { get; set; }

        [DataType(DataType.MultilineText)]

        [AllowHtml]
        public string Content { get; set; }

        HomePageModularType IHomePageModular.Type { get { return HomePageModularType.Html; } }

        public int Sort { get; set; }

        public Enums.HomePageModularType Type = Enums.HomePageModularType.Html;


    }

    public class HomePageModularByImage : IHomePageModular
    {


        public int ID { get; set; }

        public string Title { get; set; }

        [DataType(DataType.MultilineText)]

        public CommModels.FileUpload Images { get; set; } = new CommModels.FileUpload
        {
            AutoInit = false,
            Max = 50,
            Name = "Images",
            Server = CommModels.UploadServer.QinQiu,
            Sortable = true,
        };

        public string Content
        {
            get
            {
                return string.Join(",", Images);
            }
            set
            {
                Images.Images = value.SplitToArray<string>().ToArray();
            }
        }

        HomePageModularType IHomePageModular.Type { get { return HomePageModularType.Html; } }

        public int Sort { get; set; }

        public Enums.HomePageModularType Type = Enums.HomePageModularType.Html;


    }
}