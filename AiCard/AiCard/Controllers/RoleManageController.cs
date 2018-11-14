using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
using PagedList;
using PagedList.Mvc;
using AiCard.Common.Enums;
using AiCard.DAL.Models;

namespace AiCard.Controllers
{
    [Authorize]
    public class RoleManageController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();


        AccountData AccontData
        {
            get
            {
                return this.GetAccountData();
            }
        }
        RoleManager<ApplicationRole> _roleManager;
        public RoleManager<ApplicationRole> RoleManager
        {
            get
            {
                if (_roleManager == null)
                {
                    _roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(db));
                }
                return _roleManager;
            }
        }

        UserManager<ApplicationUser> _userManager;
        public UserManager<ApplicationUser> UserManager
        {
            get
            {
                if (_roleManager == null)
                {
                    _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                }
                return _userManager;
            }
        }

        public void Sidebar()
        {
            ViewBag.Sidebar = "权限管理";
        }

        [Authorize(Roles = SysRole.RoleManageRead + "," + SysRole.ERoleManageRead)]
        public ActionResult Index(int page = 1)
        {
            Sidebar();

            var usertype = AccontData.UserType;//从cookie中读取用户类型
            var enterpriseID = AccontData.EnterpriseID;//从cookie中读取enterpriseID
            string userID = AccontData.UserID;//从cookie中读取userid
            var modle = (from r in db.RoleGroups
                         join u in db.Users on r.ID equals u.RoleGroupID into ru
                         select new RoleGroupViewModel
                         {
                             ID = r.ID,
                             EnterpriseID = r.EnterpriseID,
                             Name = r.Name,
                             UserNames = ru.Select(s => s.UserName)
                         });
            //如果是企业用户则只查询该企业信息
            if (usertype == UserType.Enterprise)
            {
                modle = modle.Where(s => s.EnterpriseID == enterpriseID);
            }
            else
            {
                modle = modle.Where(s => s.EnterpriseID == 0);
            }
            var paged = modle.OrderByDescending(s => s.ID).ToPagedList(page);
            return View(paged);
        }

        [Authorize(Roles = SysRole.RoleManageCreate + "," + SysRole.ERoleManageCreate)]
        public ActionResult Create()
        {
            Sidebar();
            RoleGroupViewModel model = new RoleGroupViewModel();
            model.RolesList.List.AddRange(GetSelectRoleView());
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = SysRole.RoleManageCreate + "," + SysRole.ERoleManageCreate)]
        public ActionResult Create(RoleGroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                RoleGroup rg = new RoleGroup
                {
                    Name = model.Name,
                    Roles = model.SelectedRoles
                };
                db.RoleGroups.Add(rg);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            Sidebar();
            model.RolesList.List.AddRange(GetSelectRoleView(model.SelectedRoles));
            return View(model);
        }

        [Authorize(Roles = SysRole.RoleManageEdit + "," + SysRole.ERoleManageEdit)]
        public ActionResult Edit(int id)
        {
            var aData = this.GetAccountData();
            var enterpriseID = aData.EnterpriseID;
            Sidebar();
            RoleGroupViewModel model = new RoleGroupViewModel();
            var rg = db.RoleGroups.FirstOrDefault(s => s.ID == id);
            //防止企业用户串号修改
            if (aData.UserType == UserType.Enterprise
                && rg.EnterpriseID != enterpriseID)
            {
                return this.ToError("错误", "没有权限修改当前角色", Url.Action("Index"));
            }

            model.ID = rg.ID;
            model.Name = rg.Name;
            model.SelectedRoles = rg.Roles;
            model.RolesList.List.AddRange(GetSelectRoleView(rg.Roles));
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = SysRole.RoleManageEdit + "," + SysRole.ERoleManageEdit)]
        public ActionResult Edit(RoleGroupViewModel model)
        {
            var rg = db.RoleGroups.FirstOrDefault(s => s.ID == model.ID);
            //防止企业用户串号修改
            if (AccontData.UserType == UserType.Enterprise
                && rg.EnterpriseID != AccontData.EnterpriseID)
            {
                return this.ToError("错误", "没有权限修改当前角色", Url.Action("Index"));
            }
            if (ModelState.IsValid)
            {


                rg.Name = model.Name;
                rg.Roles = model.SelectedRoles;
                db.SaveChanges();
                var users = db.Users.Where(s => s.RoleGroupID == model.ID).ToList();
                var roles = rg.Roles.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                foreach (var user in users)
                {
                    user.Roles.Clear();
                }
                db.SaveChanges();
                foreach (var user in users)
                {
                    UserManager.AddToRoles(user.Id, roles);
                }
                db.SaveChanges();
                Sidebar();
                return RedirectToAction("Index");
            }
            model.RolesList.List.AddRange(GetSelectRoleView(model.SelectedRoles));
            return View(model);
        }

        [Authorize(Roles = SysRole.RoleManageDelete + "," + SysRole.ERoleManageDelete)]
        public ActionResult Delete(int id)
        {
            Sidebar();
            RoleGroupViewModel model = new RoleGroupViewModel();
            var rg = db.RoleGroups.FirstOrDefault(s => s.ID == id);
            //防止企业用户串号修改
            if (AccontData.UserType == UserType.Enterprise
                && rg.EnterpriseID != AccontData.EnterpriseID)
            {
                return this.ToError("错误", "没有权限修改当前角色", Url.Action("Index"));
            }
            model.ID = rg.ID;
            model.Name = rg.Name;
            model.RolesList.List.AddRange(GetSelectRoleView(rg.Roles));
            return View(model);
        }

        [HttpPost]
        [ActionName("Delete")]
        [Authorize(Roles = SysRole.RoleManageDelete + "," + SysRole.ERoleManageDelete)]
        public ActionResult DeleteConfirm(int id)
        {
            var rg = db.RoleGroups.FirstOrDefault(s => s.ID == id);
            //防止企业用户串号修改
            if (AccontData.UserType == UserType.Enterprise
                && rg.EnterpriseID != AccontData.EnterpriseID)
            {
                return this.ToError("错误", "没有权限修改当前角色", Url.Action("Index"));
            }
            db.Users.Where(s => s.RoleGroupID == id).ToList().ForEach(s => s.RoleGroupID = null);
            db.RoleGroups.Remove(rg);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public List<SelectRoleView> GetSelectRoleView(string selectedRole = "")
        {
            var userType = this.GetAccountData().UserType;
            if (selectedRole == null)
            {
                selectedRole = "";
            }

            var roles = RoleManager.Roles;
            //根据不同类型读取不同的权限
            switch (userType)
            {
                case UserType.Admin:
                    roles = roles.Where(s => s.Type == RoleType.System);
                    break;
                case UserType.Enterprise:
                    roles = roles.Where(s => s.Type == RoleType.Enterprise);
                    break;
                case UserType.Personal:
                default:
                    throw new Exception("普通用户没有权限获取");
                    break;
            }

            var lstSelectedRole = selectedRole.Split(',')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            return roles.Select(s => new SelectRoleView
            {
                Selected = lstSelectedRole.Any(x => x == s.Name),
                Name = s.Name,
                Description = s.Description,
                Group = s.Group
            }).ToList();
        }

        /// <summary>
        /// 更新配置的里面的用户权限，只有admin才能使用
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Users = "admin")]
        public ActionResult UpdateRoles()
        {

            var role = new Bll.Roles();
            role.Init();
            return Json(new { State = "Success", Message = "更新权限成功" });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
