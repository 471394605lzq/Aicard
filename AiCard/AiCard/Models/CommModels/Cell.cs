using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard.Models.CommModels
{
    public class Cell
    {
        public string ID { get; set; }

        public string Tilte { get; set; }

        public Enums.CellStyle Style { get; set; }

        public Enums.ActionType Type { get; set; }

        public object Data { get; set; }
    }
}