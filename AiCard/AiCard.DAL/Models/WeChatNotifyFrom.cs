using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCard.DAL.Models
{
    public class WeChatMiniNotifyForm
    {
        public int ID { get; set; }

        public string UserID { get; set; }

        public string OpenID { get; set; }

        public string AppID { get; set; }

        public string FormID { get; set; }

        public DateTime CreateDateTime { get; set; }

        public DateTime EndDateTime { get; set; }
    }
}
