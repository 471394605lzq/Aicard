using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Common.CommModels
{
    public class Map : IGprs
    {
        public Map() { }

        public Map(IGprs g)
        {
            Address = g.Address;
            Lat = g.Lat;
            Lng = g.Lng;
        }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 维度
        /// </summary>
        public double? Lat { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double? Lng { get; set; }

        /// <summary>
        /// 是否显示搜索
        /// </summary>
        public bool Search { get; set; } = true;

        /// <summary>
        /// 窗口宽度
        /// </summary>
        public double Width { get; set; } = 568;

        /// <summary>
        /// 窗口高度
        /// </summary>
        public double Height { get; set; } = 500;

    }
}