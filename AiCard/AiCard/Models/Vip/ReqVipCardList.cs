using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard.Models.Vip
{
    /// <summary>
    /// vip用户列表请求参数
    /// </summary>
    public sealed class ReqVipCardList :ReqPage
    {
        /// <summary>
        /// 模糊查询 姓名/手机号
        /// </summary>
        public string filter { get; set; }
    }
}