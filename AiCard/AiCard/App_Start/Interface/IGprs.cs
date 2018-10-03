using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCard
{
    public interface IGprs
    {
        /// <summary>
        /// 维度
        /// </summary>
        double? Lat { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        double? Lng { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        string Address { get; set; }
    }
}
