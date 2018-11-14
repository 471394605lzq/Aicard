using AiCard.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard.Common.CommModels
{
    public class Cell
    {
        public string ID { get; set; }

        public string Tilte { get; set; }

        public CellStyle Style { get; set; }

        public ActionType Type { get; set; }

        public object Data { get; set; }
    }
}