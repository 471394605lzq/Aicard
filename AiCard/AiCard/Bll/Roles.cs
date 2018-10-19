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
            addSystemERole(SysRole.EEnterpriseManageCreate, "企业权限管理", "企业权限管理创建");
            addSystemERole(SysRole.EEnterpriseManageEdit, "企业权限管理", "企业权限管理编辑");
            addSystemERole(SysRole.EEnterpriseManageDelete, "企业权限管理", "企业权限管理删除");

            addSystemERole(SysRole.ECardManageRead, "名片权限管理", "名片权限管理查看");
            addSystemERole(SysRole.ECardManageCreate, "名片权限管理", "名片权限管理创建");
            addSystemERole(SysRole.ECardManageEdit, "名片权限管理", "名片权限管理编辑");
            addSystemERole(SysRole.ECardMangeDelete, "名片权限管理", "名片权限管理删除");

            addSystemERole(SysRole.EHomePageModularsManageRead, "公司主页", "公司主页查看");
            addSystemERole(SysRole.EHomePageModularsManageEdit, "公司主页", "公司主页模块编辑");
            addSystemERole(SysRole.EHomePageModularsManageDelete, "公司主页", "公司主页模块删除");
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

            addSystemRole(SysRole.CardManageRead, "名片权限管理", "名片权限管理查看");
            addSystemRole(SysRole.CardManageCreate, "名片权限管理", "名片权限管理创建");
            addSystemRole(SysRole.CardManageEdit, "名片权限管理", "名片权限管理编辑");
            addSystemRole(SysRole.CardMangeDelete, "名片权限管理", "名片权限管理删除");
            #endregion


            foreach (var item in roles)
            {
                if (_appRoleManager.FindByName(item.Name) == null)
                {
                    _appRoleManager.Create(item);
                }
            }
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}