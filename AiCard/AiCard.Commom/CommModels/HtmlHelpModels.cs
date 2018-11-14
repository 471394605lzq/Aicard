using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard.Common.CommModels
{
    public class SelectOption<Txt, Val>
    {
        public Txt Text { get; set; }

        public Val Value { get; set; }

        public string ID { get; set; }

        public string Name { get; set; }

        public string CssClass { get; set; }

    }
}