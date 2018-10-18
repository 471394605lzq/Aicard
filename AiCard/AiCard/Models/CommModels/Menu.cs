using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard.Models.CommModels
{
    public class Menu
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string Image { get; set; }

        public string Title { get; set; }

        public bool Active { get; set; }

        public List<Menu> Child { get; set; }
    }
}