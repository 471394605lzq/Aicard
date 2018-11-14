using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    /// <summary>
    /// 组管理
    /// </summary>
    public class RoleGroupViewModel
    {
        public RoleGroupViewModel()
        {
            RolesList = new SelectRoleListViewModel();
        }

        public int ID { get; set; }

        [Required]
        [Display(Name = "名称")]
        public string Name { get; set; }

        [Required(ErrorMessage = "未选择角色")]
        [Display(Name = "角色")]
        public string SelectedRoles { get; set; }
        public int EnterpriseID { get; set; }
        /// <summary>
        /// 全部组列表
        /// </summary>
        public SelectRoleListViewModel RolesList { get; set; }

        [Display(Name = "用户列表")]
        public IEnumerable<string> UserNames { get; set; }
    }
}