using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Net;

namespace AiCard.Controllers
{
    [Authorize]
    public class EnterpriseManageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
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

        //列表页
        public ActionResult Index(string filter, bool? enable = null, int page = 1)
        {
            var m = from e in db.Enterprises
                    from u in db.Users
                    where e.AdminID == u.Id
                    select new EnterpriseViewModels
                    {
                        ID = e.ID,
                        Enable = e.Enable,
                        Admin = u.UserName,
                        CardCount = e.CardCount,
                        Name = e.Name,
                        PhoneNumber = e.PhoneNumber,
                        AdminID = u.Id
                    };
            if (!string.IsNullOrWhiteSpace(filter))
            {
                m = m.Where(s => s.Name.Contains(filter));
            }
            if (enable != null)
            {
                m = m.Where(s => s.Enable == enable.Value);
            }
            var paged = m.OrderByDescending(s => s.AdminID).ToPagedList(page);
            return View(paged);
        }
        //新增
        public ActionResult Create()
        {
            return View();
        }
        // POST: tests/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Enterprise enterprise)
        {
            if (ModelState.IsValid)
            {
                var hascode = db.Enterprises.Any(s => s.Code == enterprise.Code);
                var hasUser = db.Enterprises.Any(s => s.Name == enterprise.Name);
                //检验企业编号是否存在
                if (hascode)
                {
                    ModelState.AddModelError("Code", "企业编号已存在");
                    return View(enterprise);
                }
                //检验企业名称是否存在
                else if (hasUser)
                {
                    ModelState.AddModelError("Name", "企业已经存在");
                    return View(enterprise);
                }
                else
                {
                    //查询type为0的用户权限
                    var listrg = RoleManager.Roles.Where(s => s.Type == 0).ToList();
                    //拼接RoleGroup表需要的Roles字段值
                    string temprolesstr = "";
                    for (int i = 0; i < listrg.Count; i++)
                    {
                        if (string.IsNullOrEmpty(temprolesstr))
                        {
                            temprolesstr = listrg[i].Name;
                        }
                        else
                        {
                            temprolesstr = temprolesstr + "," + listrg[i].Name;
                        }
                    }
                    //为企业单独设定一个管理员权限
                    RoleGroup rg = new RoleGroup
                    {
                        Name = enterprise.Name + "系统管理员",
                        Roles = temprolesstr
                    };
                    db.RoleGroups.Add(rg);
                    int saveresult = db.SaveChanges();
                    if (saveresult > 0)
                    {
                        var rgid = rg.ID;
                        //用企业编号自动创建一个企业管理账号,默认密码为"123456"
                        string username = enterprise.Code + "admin";
                        var user = new ApplicationUser { UserName = username, RegisterDateTime = DateTime.Now, LastLoginDateTime = DateTime.Now, RoleGroupID = rgid };
                        var result = await UserManager.CreateAsync(user, "123456");
                        if (result.Succeeded)
                        {
                            enterprise.AdminID = user.Id;
                            enterprise.RegisterDateTime = DateTime.Now;
                            db.Enterprises.Add(enterprise);
                            int r = db.SaveChanges();
                            if (r > 0)
                            {
                                var t = db.RoleGroups.FirstOrDefault(s => s.ID == rgid);
                                t.EnterpriseID = enterprise.ID;
                                db.SaveChanges();
                            }
                            return RedirectToAction("Index");
                        }
                    }
                }
            }
            return View(enterprise);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = (from e in db.Enterprises
                         from u in db.Users
                         where e.AdminID == u.Id && e.ID == id.Value
                         select e).FirstOrDefault();
            var models = new Enterprise
            {
                ID = model.ID,
                Code = model.Code,
                Province = model.Province,
                City = model.City,
                Address = model.Address,
                Email = model.Email,
                HomePage = model.HomePage,
                Logo = model.Logo,
                Info = model.Info,
                District = model.District,
                Enable = model.Enable,
                CardCount = model.CardCount,
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
                AdminID = model.AdminID,
                RegisterDateTime=model.RegisterDateTime,
                AppID=model.AppID,
                Lat=model.Lat,
                Lng=model.Lng
            };
            if (models == null)
            {
                return HttpNotFound();
            }
            return View(models);
        }

        // POST: tests/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Enterprise enterprise)
        {
            if (ModelState.IsValid)
            {
                var t = db.Enterprises.FirstOrDefault(s => s.ID == enterprise.ID);
                t.Name = enterprise.Name;
                t.Code = enterprise.Code;
                t.Province = enterprise.Province;
                t.City = enterprise.City;
                t.Address = enterprise.Address;
                t.Email = enterprise.Email;
                t.District = enterprise.District;
                t.HomePage = enterprise.HomePage;
                t.Info = enterprise.Info;
                t.Enable = enterprise.Enable;
                t.CardCount = enterprise.CardCount;
                t.PhoneNumber = enterprise.PhoneNumber;
                t.Logo = enterprise.Logo;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(enterprise);
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