using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Models
{

    /// <summary>
    /// 角色列表
    /// </summary>
    public class SelectRoleView
    {
        /// <summary>
        /// 是否选择角色
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "描述")]
        public string Description { get; set; }

        /// <summary>
        /// 组名
        /// </summary>
        public string Group { get; set; }
    }
}