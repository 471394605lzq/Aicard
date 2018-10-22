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
            if (p.IsInRole(SysRole.UserManageRead) || p.IsInRole(SysRole.EUserManageRead))
            {
                menus.Add(new Models.CommModels.Menu { Name = "用户管理", Title = "用户管理", Url = "~/UserManage/Index" });
            }
            if (p.IsInRole(SysRole.RoleManageRead) || p.IsInRole(SysRole.ERoleManageRead))
            {
                menus.Add(new Models.CommModels.Menu { Name = "权限管理", Title = "权限管理", Url = "~/RoleManage/Index" });
            }
            if (p.IsInRole(SysRole.EnterpriseManageRead) || p.IsInRole(SysRole.EEnterpriseManageRead))
            {
                menus.Add(new Models.CommModels.Menu { Name = "企业管理", Title = "企业管理", Url = "~/EnterpriseManage/Index" });
            }
            if (p.IsInRole(SysRole.EHomePageModularsManageRead))
            {
                menus.Add(new Models.CommModels.Menu { Name = "公司首页", Title = "公司首页", Url = "~/EnterpriseManage/Index" });
            }
            if (p.IsInRole(SysRole.EnterpriseManageRead) || p.IsInRole(SysRole.EEnterpriseManageRead))
            {
                menus.Add(new Models.CommModels.Menu { Name = "名片管理", Title = "名片管理", Url = "~/CardManage/Index" });
            }
            if (p.IsInRole(SysRole.EnterpriseManageRead) || p.IsInRole(SysRole.EEnterpriseManageRead))
            {
                menus.Add(new Models.CommModels.Menu { Name = "商品分类管理", Title = "商品分类管理", Url = "~/ProductKindsManage/Index" });
            }
            if (p.IsInRole(SysRole.EnterpriseManageRead) || p.IsInRole(SysRole.EEnterpriseManageRead))
            {
                menus.Add(new Models.CommModels.Menu { Name = "商品管理", Title = "商品管理", Url = "~/ProductsManage/Index" });
            }
            return menus;
        }
    }
}