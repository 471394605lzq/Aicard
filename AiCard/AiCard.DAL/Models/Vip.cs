using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCard.DAL.Models
{
    public class Vip
    {
        public int ID { get; set; }

        public string UserID { get; set; }

        public DateTime CreateDateTime { get; set; }

        public Common.Enums.VipType Type { get; set; }

        public Common.Enums.VipState State { get; set; }
    }
}
