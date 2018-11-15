using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCard.DAL.Models
{
    /// <summary>
    /// 提现订单
    /// </summary>
    public class VipForwardOrder
    {
        public int ID { get; set; }

        public string Code { get; set; }

        public string PayCode { get; set; }

        public string UserID { get; set; }

        public Common.Enums.VipForwardState State { get; set; }

        public DateTime CreateDateTime { get; set; }

    }
}
