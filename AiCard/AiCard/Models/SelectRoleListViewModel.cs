using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    /// <summary>
    /// 全部组列表
    /// </summary>
    public class SelectRoleListViewModel
    {
        public SelectRoleListViewModel() { List = new List<SelectRoleView>(); }

        /// <summary>
        /// 角色列表
        /// </summary>
        public List<SelectRoleView> List
        {
            get; set;
        }
    }
}