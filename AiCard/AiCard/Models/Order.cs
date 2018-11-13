using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    public class Order
    {
        public int ID { get; set; }

        public string Code { get; set; }

        public string UserID { get; set; }

        public decimal Amount { get; set; }

        public decimal ReceivableAmount { get; set; }

        public Enums.PayType PayType { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime? PayDateTime { get; set; }
    }
}