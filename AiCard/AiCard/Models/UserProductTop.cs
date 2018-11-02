using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    public class UserProductTop
    {
        public int ID { get; set; }

        public string UserID { get; set; }

        public int ProductID { get; set; }

        public DateTime CreateDateTime { get; set; }
    }
}