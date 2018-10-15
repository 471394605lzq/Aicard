using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    public class RolesModel
    {
        public int id { get; set; }
        //名称
        public string Name { get; set; }
        //类型
        public int Type { get; set; }
        //分组
        public string Group { get; set; }
        //描述
        public string Description { get; set; }
        //标识
        public string Discriminator { get; set; }
    }
}