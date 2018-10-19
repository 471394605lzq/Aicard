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
using AiCard.Enums;
using AiCard.WeChatWork;
using AiCard.WeChatWork.Models;
using PagedList;
using PagedList.Mvc;

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
        public void Sidebar(string name = "企业管理")
        {
            ViewBag.Sidebar = name;

        }
        //列表页
        public ActionResult Index(string filter, bool? enable = null, int page = 1)
        {
            Sidebar();
            int usertype = (int)this.GetAccountData().UserType;//从cookie中读取用户类型
            string userID = this.GetAccountData().UserID;//从cookie中读取userid
            var user = db.Users.FirstOrDefault(s => s.Id == userID);
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
                        AdminID = u.Id,
                        Province = e.Province,
                        City = e.City,
                        District = e.District,
                        Address = e.Address,
                        HomePage = e.HomePage,
                        Info = e.Info,
                        Code = e.Code,
                        Email = e.Email
                    };
            if (!string.IsNullOrWhiteSpace(filter))
            {
                m = m.Where(s => s.Name.Contains(filter));
            }
            if (enable != null)
            {
                m = m.Where(s => s.Enable == enable.Value);
            }
            //如果是企业用户则只查询该企业信息
            if (usertype == 1)
            {
                m = m.Where(s => s.ID == user.EnterpriseID);
            }
            var paged = m.OrderByDescending(s => s.AdminID).ToPagedList(page);
            return View(paged);
        }

        public ActionResult Info()
        {
            Sidebar();
            string userID = this.GetAccountData().UserID;//从cookie中读取userid
            var user = db.Users.FirstOrDefault(s => s.Id == userID);
            if (user.Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = (from e in db.Enterprises
                         from u in db.Users
                         where e.AdminID == u.Id && e.ID == user.EnterpriseID
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
                RegisterDateTime = model.RegisterDateTime,
                AppID = model.AppID,
                Lat = model.Lat,
                Lng = model.Lng
            };
            if (models == null)
            {
                return HttpNotFound();
            }
            return View(models);
        }

        //同步企业微信中的微信用户信息
        public ActionResult CogradientWXUserInfo(int? id, int page = 1)
        {
            Sidebar();
            var em = db.Enterprises.FirstOrDefault(s => s.ID == id.Value);
            ViewBag.code = em.Code;
            ViewBag.name = em.Name;
            ViewBag.logo = em.Logo;

            List<User> list = new List<WeChatWork.Models.User>();
            //错误的时候返回该model
            PagedList.PagedList<AiCard.WeChatWork.Models.User> model = new PagedList<WeChatWork.Models.User>(list, page, 15);
            if (string.IsNullOrWhiteSpace(em.WeChatWorkCorpid) || string.IsNullOrWhiteSpace(em.WeChatWorkSecret))
            {
                ViewBag.errormsg = "错误提示：请先配置微信corpid和secrrt!";
                return View(model);
            }
            else
            {
                try
                {
                    ViewBag.errormsg = "";
                    WeChatWorkApi wxapi = new WeChatWorkApi(em.WeChatWorkCorpid, em.WeChatWorkSecret);
                    List<Department> listdepartment = wxapi.GetDepartment();
                    List<User> listwxuser = null;
                    if (listdepartment.Count > 0)
                    {
                        for (int i = 0; i < listdepartment.Count; i++)
                        {
                            listwxuser.AddRange(wxapi.GetUsesByDepID(listdepartment[i].ID));
                        }
                    }

                    var paged = listwxuser.OrderByDescending(s => s.ID).ToList();
                    PagedList.PagedList<AiCard.WeChatWork.Models.User> models = new PagedList<WeChatWork.Models.User>(paged, page, 15);
                    return View(models);
                }
                catch (Exception ex) {
                    ViewBag.errormsg = "错误提示："+ ex.ToString();
                    return View(model);
                }
            }
        }

        //新增
        public ActionResult Create()
        {
            Sidebar();
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

            Sidebar();
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
                    //saveresult>0表示新增权限组信息成功
                    if (saveresult > 0)
                    {
                        var rgid = rg.ID;
                        //用企业编号自动创建一个企业管理账号,默认密码为"123456"
                        string username = enterprise.Code + "admin";
                        var user = new ApplicationUser { UserName = username, RegisterDateTime = DateTime.Now, LastLoginDateTime = DateTime.Now, RoleGroupID = rgid, UserType = UserType.Enterprise };
                        var result = await UserManager.CreateAsync(user, "123456");
                        //创建企业管理员账号成功
                        if (result.Succeeded)
                        {
                            enterprise.AdminID = user.Id;
                            enterprise.RegisterDateTime = DateTime.Now;
                            db.Enterprises.Add(enterprise);
                            int r = db.SaveChanges();
                            //r>0表示保存企业信息成功
                            if (r > 0)
                            {
                                var t = db.RoleGroups.FirstOrDefault(s => s.ID == rgid);
                                t.EnterpriseID = enterprise.ID;
                                var u = db.Users.FirstOrDefault(s => s.Id == enterprise.AdminID);
                                u.EnterpriseID = enterprise.ID;
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
            Sidebar();
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
                RegisterDateTime = model.RegisterDateTime,
                AppID = model.AppID,
                Lat = model.Lat,
                Lng = model.Lng
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
            Sidebar();
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
        //删除
        public ActionResult Delete(int id)
        {
            Enterprise enterprise = db.Enterprises.Find(id);
            var adminid = enterprise.AdminID;
            //删除跟企业关联的管理员账号信息
            var user = db.Users.FirstOrDefault(s => s.Id == adminid);
            if (user != null)
            {
                db.Users.Remove(user);
            }
            //删除跟企业关联的权限组信息
            var rolegroup = db.RoleGroups.FirstOrDefault(s => s.EnterpriseID == id);
            if (rolegroup != null)
            {
                db.RoleGroups.Remove(rolegroup);
            }
            //删除企业信息

            if (enterprise != null)
            {
                db.Enterprises.Remove(enterprise);
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //配置企业和微信绑定
        public ActionResult Deploy(int? id)
        {

            Sidebar();
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return RedirectToAction("Index");
            }
            var model = (from e in db.Enterprises
                         from u in db.Users
                         where e.AdminID == u.Id && e.ID == id.Value
                         select e).FirstOrDefault();
            var models = new Enterprise
            {
                ID = model.ID,
                WeChatWorkCorpid = model.WeChatWorkCorpid,
                WeChatWorkSecret = model.WeChatWorkSecret
            };
            if (models == null)
            {
                return HttpNotFound();
            }
            return View(models);
        }

        //保存微信绑定设置
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deploy(Enterprise enterprise)
        {
            Sidebar();
            if (ModelState.IsValid)
            {
                var t = db.Enterprises.FirstOrDefault(s => s.ID == enterprise.ID);
                t.WeChatWorkCorpid = enterprise.WeChatWorkCorpid;
                t.WeChatWorkSecret = enterprise.WeChatWorkSecret;
                db.SaveChanges();
                return RedirectToAction("Deploy", new { id = enterprise.ID });
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