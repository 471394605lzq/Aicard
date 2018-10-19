using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace AiCard.Models
{
    public class HomePageModularByHtml
    {
        public int ID { get; set; }

        public string Title { get; set; }

        [DataType(DataType.MultilineText)]

        [AllowHtml]
        public string Content { get; set; }

        public Enums.HomePageModularType Type = Enums.HomePageModularType.Html;


    }
}