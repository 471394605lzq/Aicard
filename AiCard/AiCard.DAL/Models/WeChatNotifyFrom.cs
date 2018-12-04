using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCard.DAL.Models
{
    public class WeChatNotifyFrom
    {
        public int ID { get; set; }

        public string UserID { get; set; }

        public string OpenID { get; set; }

        public string AppID { get; set; }

        public DateTime CreateDateTime { get; set; }
    }
}
