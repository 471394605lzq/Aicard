using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCard.Common
{
    /// <summary>
    /// 分区
    /// </summary>
    public interface IDistrict
    {
        string Province { get; set; }

        string City { get; set; }

        string District { get; set; }
        
    }
}
