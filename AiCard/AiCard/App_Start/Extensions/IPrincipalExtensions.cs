using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard
{
    public static class IPrincipalExtensions
    {
        public static List<Models.CommModels.Menu> Menus(this System.Security.Principal.IPrincipal p)
        {
            var menus = new List<Models.CommModels.Menu>();
            if (p.IsInRole(SysRole.UserManageRead))
            {
                menus.Add(new Models.CommModels.Menu { Name = "用户管理", Title = "用户管理", Url = "~/UserManage/Index" });
            }
            if (p.IsInRole(SysRole.RoleManageRead))
            {
                menus.Add(new Models.CommModels.Menu { Name = "权限管理", Title = "权限管理", Url = "~/RoleManage/Index" });
            }
            if (p.IsInRole(SysRole.ERoleManageRead)||p.IsInRole(SysRole.EnterpriseManageRead))
            {
                menus.Add(new Models.CommModels.Menu { Name = "企业管理", Title = "企业管理", Url = "~/EnterpriseManage/Index" });
            }
            return menus;
        }
    }
}