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
                menus.Add(new Models.CommModels.Menu { Name = "用户管理", Title = "用户管理", Url = "~/UserManage/Index",IconImage= "yonghu" });
            }
            if (p.IsInRole(SysRole.RoleManageRead) || p.IsInRole(SysRole.ERoleManageRead))
            {
                menus.Add(new Models.CommModels.Menu { Name = "权限管理", Title = "权限管理", Url = "~/RoleManage/Index", IconImage = "moban" });
            }
            if (p.IsInRole(SysRole.EnterpriseManageRead) || p.IsInRole(SysRole.EEnterpriseManageRead))
            {
                menus.Add(new Models.CommModels.Menu { Name = "企业管理", Title = "企业管理", Url = "~/EnterpriseManage/Index",IconImage = "iconqyxx" });
            }
            if (p.IsInRole(SysRole.EHomePageModularsManageRead))
            {
                menus.Add(new Models.CommModels.Menu { Name = "公司首页", Title = "公司首页", Url = "~/EnterpriseManage/Index",IconImage = "shouyeshouye" });
            }
            if (p.IsInRole(SysRole.CardManageRead) || p.IsInRole(SysRole.ECardManageRead))
            {
                menus.Add(new Models.CommModels.Menu { Name = "名片管理", Title = "名片管理", Url = "~/CardManage/Index",IconImage = "mingpianliebiao" });
            }
            if (p.IsInRole(SysRole.ProductKindManageRead) || p.IsInRole(SysRole.EProductKindManageRead))
            {
                menus.Add(new Models.CommModels.Menu { Name = "商品分类管理", Title = "商品分类管理", Url = "~/ProductKindsManage/Index",IconImage = "fenlei-" });
            }
            if (p.IsInRole(SysRole.ProductManageRead) || p.IsInRole(SysRole.EProductManageRead))
            {
                menus.Add(new Models.CommModels.Menu { Name = "商品管理", Title = "商品管理", Url = "~/ProductsManage/Index",IconImage = "shangpin" });
            }
            return menus;
        }
    }
}