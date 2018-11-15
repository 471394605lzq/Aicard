using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCard.DAL.Models
{
    public class VipRelationship
    {
        public int ID { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// VIPID
        /// </summary>
        public int VipID { get; set; }

        /// <summary>
        /// 父用户ID
        /// </summary>
        public string ParentUserID { get; set; }

        /// <summary>
        /// 父的VIPID
        /// </summary>
        public int ParentID { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

    }
}
