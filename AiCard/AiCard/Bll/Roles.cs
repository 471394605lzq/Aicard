using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AiCard.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AiCard.Bll
{
    public class Roles : IDisposable
    {
        public Roles()
        {
            _appRoleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(db));
            _appUserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        private ApplicationDbContext db = new ApplicationDbContext();

        private RoleManager<ApplicationRole> _appRoleManager;

        private UserManager<ApplicationUser> _appUserManager;

        public void EditUserRole(string userID, IEnumerable<string> roles)
        {
            var old = _appUserManager.GetRoles(userID).ToArray();
            _appUserManager.RemoveFromRoles(userID, old);
            foreach (var item in roles)
            {
                _appUserManager.AddToRole(userID, item);
            }
        }

        public void EditUserRoleByGroupID(string userID, int groupID)
        {
            var roles = db.RoleGroups.FirstOrDefault(s => s.ID == groupID).Roles.SplitToArray<string>();
            var old = _appUserManager.GetRoles(userID).ToArray();
            _appUserManager.RemoveFromRoles(userID, old);
            foreach (var item in roles)
            {
                _appUserManager.AddToRole(userID, item);
            }
        }



        public bool IsInRole(string userID, string role)
        {
            return _appUserManager.IsInRole(userID, role);
        }


        public IEnumerable<string> GetRoles(string userID)
        {
            return _appUserManager.GetRoles(userID);
        }

        public void Init()
        {
            List<ApplicationRole> roles = new List<ApplicationRole>();
            #region 企业权限
            Action<string, string, string> addSystemERole = (name, gourp, desc) =>
            {
                roles.Add(new ApplicationRole
                {
                    Description = desc,
                    Group = gourp,
                    Name = name,
                    Type = Enums.RoleType.Enterprise
                });
            };

            addSystemERole(SysRole.EUserManageRead, "系统用户", "系统用户查看");
            addSystemERole(SysRole.EUserManageCreate, "系统用户", "系统用户创建");
            addSystemERole(SysRole.EUserManageEdit, "系统用户", "系统用户编辑");
            addSystemERole(SysRole.EUserManageDelete, "系统用户", "系统用户删除");

            addSystemERole(SysRole.ERoleManageRead, "后台权限管理", "后台权限管理查看");
            addSystemERole(SysRole.ERoleManageCreate, "后台权限管理", "后台权限管理创建");
            addSystemERole(SysRole.ERoleManageEdit, "后台权限管理", "后台权限管理编辑");
            addSystemERole(SysRole.ERoleManageDelete, "后台权限管理", "后台权限管理删除");

            addSystemERole(SysRole.EEnterpriseManageRead, "企业权限管理", "企业权限管理查看");
            addSystemERole(SysRole.EEnterpriseManageEdit, "企业权限管理", "企业权限管理编辑");
            addSystemERole(SysRole.EEnterpriseManageDeploy, "企业权限管理", "企业权限管理绑定企业微信");
            addSystemERole(SysRole.EEnterpriseManageCogradient, "企业权限管理", "企业权限管理同步企业微信用户");

            addSystemERole(SysRole.ECardManageRead, "名片权限管理", "名片权限管理查看");
            addSystemERole(SysRole.ECardManageCreate, "名片权限管理", "名片权限管理创建");
            addSystemERole(SysRole.ECardManageEdit, "名片权限管理", "名片权限管理编辑");
            addSystemERole(SysRole.ECardManageDelete, "名片权限管理", "名片权限管理删除");

            addSystemERole(SysRole.EHomePageModularsManageRead, "公司主页", "公司主页查看");
            addSystemERole(SysRole.EHomePageModularsManageCreate, "公司主页", "公司主页创建");
            addSystemERole(SysRole.EHomePageModularsManageEdit, "公司主页", "公司主页模块编辑");
            addSystemERole(SysRole.EHomePageModularsManageDelete, "公司主页", "公司主页模块删除");

            addSystemERole(SysRole.EProductKindManageRead, "商品分类权限管理", "商品分类权限管理查看");
            addSystemERole(SysRole.EProductKindManageCreate, "商品分类权限管理", "商品分类权限管理创建");
            addSystemERole(SysRole.EProductKindManageEdit, "商品分类权限管理", "商品分类权限管理编辑");
            addSystemERole(SysRole.EProductKindMangeDelete, "商品分类权限管理", "商品分类权限管理删除");

            addSystemERole(SysRole.EProductManageRead, "商品权限管理", "商品权限管理查看");
            addSystemERole(SysRole.EProductManageCreate, "商品权限管理", "商品权限管理创建");
            addSystemERole(SysRole.EProductManageEdit, "商品权限管理", "商品权限管理编辑");
            addSystemERole(SysRole.EProductMangeDelete, "商品权限管理", "商品权限管理删除");

            addSystemERole(SysRole.ECustomerTabGroupsManageRead, "客户标签分组权限管理", "客户标签分组权限管理查看");
            addSystemERole(SysRole.ECustomerTabGroupsManageCreate, "客户标签分组权限管理", "客户标签分组权限管理创建");
            addSystemERole(SysRole.ECustomerTabGroupsManageEdit, "客户标签分组权限管理", "客户标签分组权限管理编辑");
            addSystemERole(SysRole.ECustomerTabGroupsMangeDelete, "客户标签分组权限管理", "客户标签分组权限管理删除");

            addSystemERole(SysRole.ECustomerTabManageRead, "客户标签分组权限管理", "客户标签分组权限管理查看");
            addSystemERole(SysRole.ECustomerTabManageCreate, "客户标签分组权限管理", "客户标签分组权限管理创建");
            addSystemERole(SysRole.ECustomerTabManageEdit, "客户标签分组权限管理", "客户标签分组权限管理编辑");
            addSystemERole(SysRole.ECustomerTabMangeDelete, "客户标签分组权限管理", "客户标签分组权限管理删除");

            addSystemERole(SysRole.EArticlesManageRead, "动态管理", "动态管理查看");
            addSystemERole(SysRole.EArticlesManageCreate, "动态管理", "动态管理创建");
            addSystemERole(SysRole.EArticlesManageEdit, "动态管理", "动态管理编辑");
            addSystemERole(SysRole.EArticlesMangeDelete, "动态管理", "动态管理删除");
            addSystemERole(SysRole.EArticlesMangeCheck, "动态管理", "动态管理审核");

            #endregion
            #region 后台权限
            Action<string, string, string> addSystemRole = (name, gourp, desc) =>
            {
                roles.Add(new ApplicationRole
                {
                    Description = desc,
                    Group = gourp,
                    Name = name,
                    Type = Enums.RoleType.System
                });

            };
            addSystemRole(SysRole.UserManageRead, "系统用户", "系统用户查看");
            addSystemRole(SysRole.UserManageCreate, "系统用户", "系统用户创建");
            addSystemRole(SysRole.UserManageEdit, "系统用户", "系统用户编辑");
            addSystemRole(SysRole.UserManageDelete, "系统用户", "系统用户删除");

            addSystemRole(SysRole.RoleManageRead, "权限管理", "权限管理查看");
            addSystemRole(SysRole.RoleManageCreate, "权限管理", "权限管理创建");
            addSystemRole(SysRole.RoleManageEdit, "权限管理", "权限管理编辑");
            addSystemRole(SysRole.RoleManageDelete, "权限管理", "权限管理删除");

            addSystemRole(SysRole.EnterpriseManageRead, "企业权限管理", "企业权限管理查看");
            addSystemRole(SysRole.EnterpriseManageCreate, "企业权限管理", "企业权限管理创建");
            addSystemRole(SysRole.EnterpriseManageEdit, "企业权限管理", "企业权限管理编辑");
            addSystemRole(SysRole.EnterpriseManageDelete, "企业权限管理", "企业权限管理删除");
            addSystemRole(SysRole.EnterpriseManageDeploy, "企业权限管理", "企业权限管理绑定企业微信");
            addSystemRole(SysRole.EnterpriseManageCogradient, "企业权限管理", "企业权限管理同步企业微信用户");

            addSystemRole(SysRole.CardManageRead, "名片权限管理", "名片权限管理查看");
            addSystemRole(SysRole.CardManageCreate, "名片权限管理", "名片权限管理创建");
            addSystemRole(SysRole.CardManageEdit, "名片权限管理", "名片权限管理编辑");
            addSystemRole(SysRole.CardMangeDelete, "名片权限管理", "名片权限管理删除");

            addSystemRole(SysRole.ProductKindManageRead, "商品分类权限管理", "商品分类权限管理查看");
            addSystemRole(SysRole.ProductKindManageCreate, "商品分类权限管理", "商品分类权限管理创建");
            addSystemRole(SysRole.ProductKindManageEdit, "商品分类权限管理", "商品分类权限管理编辑");
            addSystemRole(SysRole.ProductKindMangeDelete, "商品分类权限管理", "商品分类权限管理删除");

            addSystemRole(SysRole.ProductManageRead, "商品权限管理", "商品权限管理查看");
            addSystemRole(SysRole.ProductManageCreate, "商品权限管理", "商品权限管理创建");
            addSystemRole(SysRole.ProductManageEdit, "商品权限管理", "商品权限管理编辑");
            addSystemRole(SysRole.ProductMangeDelete, "商品权限管理", "商品权限管理删除");


            addSystemRole(SysRole.CustomerTabGroupsManageRead, "客户标签分组权限管理", "客户标签分组权限管理查看");
            addSystemRole(SysRole.CustomerTabGroupsManageCreate, "客户标签分组权限管理", "客户标签分组权限管理创建");
            addSystemRole(SysRole.CustomerTabGroupsManageEdit, "客户标签分组权限管理", "客户标签分组权限管理编辑");
            addSystemRole(SysRole.CustomerTabGroupsMangeDelete, "客户标签分组权限管理", "客户标签分组权限管理删除");


            addSystemRole(SysRole.CustomerTabManageRead, "客户标签权限管理", "客户标签权限管理查看");
            addSystemRole(SysRole.CustomerTabManageCreate, "客户标签权限管理", "客户标签权限管理创建");
            addSystemRole(SysRole.CustomerTabManageEdit, "客户标签权限管理", "客户标签权限管理编辑");
            addSystemRole(SysRole.CustomerTabMangeDelete, "客户标签权限管理", "客户标签权限管理删除");

            addSystemRole(SysRole.ArticlesManageRead, "动态管理", "动态管理查看");
            addSystemRole(SysRole.ArticlesManageCreate, "动态管理", "动态管理创建");
            addSystemRole(SysRole.ArticlesManageEdit, "动态管理", "动态管理编辑");
            addSystemRole(SysRole.ArticlesMangeDelete, "动态管理", "动态管理删除");
            addSystemRole(SysRole.ArticlesMangeCheck, "动态管理", "动态管理审核");
            #endregion

            var dbRoles = _appRoleManager.Roles.ToList();
       
            foreach (var item in roles)
            {
                //如果没有当前权限就添加到数据库
                if (!dbRoles.Any(s => s.Name == item.Name && s.Type == item.Type))
                {
                    _appRoleManager.Create(item);
                }
            }

            foreach (var item in dbRoles)
            {
                //如果数据库存在，当时配置里面不出在就删除
                if (!roles.Any(s => s.Name == item.Name && s.Type == item.Type))
                {
                    _appRoleManager.Delete(item);
                }
            }

        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}