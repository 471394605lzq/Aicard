using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    public class EnterpriseViewModels
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string AdminID { get; set; }

        public string Admin { get; set; }

        public int CardCount { get; set; }


        public bool Enable { get; set; }

        public string PhoneNumber { get; set; }
    }
}