using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
namespace AiCard.Controllers
{
    [Authorize]
    public class UserManageController : Controller
    {
        public string UserID
        {
            get
            {
                return User.Identity.GetUserId();

            }
        }


        ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }



        public void Sidebar(string name = "用户管理")
        {
            ViewBag.Sidebar = name;

        }

        [Authorize(Roles = SysRole.UserManageRead)]
        public ActionResult Index(string filter, Enums.UserType? userType, int page = 1)
        {
            Sidebar();
            var users = from u in db.Users
                        from r in db.RoleGroups
                        where u.RoleGroupID == r.ID
                        select new
                        {
                            User = u,
                            Role = r
                        };
            if (!string.IsNullOrWhiteSpace(filter))
            {
                users = users.Where(s => s.User.UserName == filter);
            }
            if (userType != null)
            {
                users = users.Where(s => s.User.UserType == userType);
            }

            var paged = users.OrderByDescending(s => s.User.RegisterDateTime)
                .Select(s => new UserManageIndexViewModel
                {
                    ID = s.User.Id,
                    Name = s.User.UserName,
                    Role = s.Role.Name
                }).ToPagedList(page);
            return View(paged);
        }


        public void SetRoleGroup()
        {
            var u = db.Users.FirstOrDefault(s => s.Id == UserID);
            var roles = db.RoleGroups.Where(s => s.EnterpriseID == u.EnterpriseID)
                 .Select(s => new
                 {
                     s.Name,
                     s.ID
                 }).ToList();
            ViewBag.RoleGroup = new SelectList(roles, dataValueField: "ID", dataTextField: "Name");
        }

        [Authorize(Roles = SysRole.UserManageCreate)]
        public ActionResult Create()
        {
            Sidebar();
            SetRoleGroup();
            return View();
        }


        [HttpPost]
        [Authorize(Roles = SysRole.UserManageCreate)]
        public ActionResult Create(UserManageCreateViewModel model)
        {
            Sidebar();
            var hasUser = db.Users.Any(s => s.UserName == model.Name);
            if (hasUser)
            {
                ModelState.AddModelError("Name", "帐号已经存在");
                SetRoleGroup();
                return View(model);
            }
            var user = new ApplicationUser
            {
                UserName = model.Name,
                UserType = Enums.UserType.Admin,
                RegisterDateTime = DateTime.Now,
                RoleGroupID = model.RoleGroupID,
                LastLoginDateTime = DateTime.Now
            };
            var result = UserManager.Create(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                SetRoleGroup();
                return View(model);
            }
            new Bll.Roles().EditUserRoleByGroupID(user.Id, user.RoleGroupID.Value);
            return RedirectToAction("Index");
        }


        [Authorize(Roles = SysRole.UserManageEdit)]
        public ActionResult Edit(string id)
        {
            Sidebar();
            SetRoleGroup();
            var user = db.Users.FirstOrDefault(s => s.Id == id);
            var model = new UserManageEditViewModel
            {
                Name = user.UserName,
                RoleGroupID = user.RoleGroupID,
                ID = user.Id
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = SysRole.UserManageEdit)]
        public ActionResult Edit(UserManageEditViewModel model)
        {

            var user = db.Users.FirstOrDefault(s => s.Id == model.ID);
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                UserManager.RemovePassword(model.ID);
                var result = UserManager.AddPassword(model.ID, model.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("Password", error);
                    }
                    Sidebar();
                    SetRoleGroup();
                    return View(model);
                }
            }
            if (user.RoleGroupID != model.RoleGroupID)
            {
                user.RoleGroupID = model.RoleGroupID;
                db.SaveChanges();
                new Bll.Roles().EditUserRoleByGroupID(model.ID, model.RoleGroupID ?? 0);
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = SysRole.UserManageDelete)]
        public ActionResult Delete(string id)
        {
            var user = (from u in db.Users
                        from r in db.RoleGroups
                        where u.Id == id && u.RoleGroupID == r.ID
                        select new UserManageEditViewModel
                        {
                            ID = u.Id,
                            Name = u.UserName,
                            RoleGroup = r.Name
                        }).FirstOrDefault();
            if (user == null)
            {
                return this.ToError("错误", "用户不存在", Url.Action("Index"));
            }

            return View(user);
        }

        [HttpPost]
        [ActionName("Delete")]
        [Authorize(Roles = SysRole.UserManageDelete)]
        public ActionResult DeleteConfirm(string id)
        {
            var user = db.Users.FirstOrDefault(s => s.Id == id);
            new Bll.Roles().EditUserRole(user.Id, new List<string>());
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
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