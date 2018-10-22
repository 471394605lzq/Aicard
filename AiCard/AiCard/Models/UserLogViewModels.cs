using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    public class UserLogListViewModel
    {
        public int ID { get; set; }

        public string UserID { get; set; }

        public string UserAvatar { get; set; }

        public string UserNickName { get; set; }

        public string TargetUserID { get; set; }


        public int? TargetEnterpriseID { get; set; }


        public string Remark { get; set; }


        public DateTime CreateDateTime { get; set; }

        public Enums.UserLogType Type { get; set; }


        public int RelationID { get; set; }
    }
}