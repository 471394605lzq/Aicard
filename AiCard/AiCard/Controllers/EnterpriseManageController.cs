using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net;
using AiCard.Common.WeChatWork;
using AiCard.Common.WeChatWork.Models;
using PagedList;
using Newtonsoft.Json;
using AiCard.Common.Enums;
using AiCard.Common;
using AiCard.Common.CommModels;
using AiCard.DAL.Models;

namespace AiCard.Controllers
{
    [Authorize]
    public class EnterpriseManageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;


        //获取当前登录的用户信息
        AccountData AccountData
        {
            get
            {
                return this.GetAccountData();
            }
        }
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

        public void AccountDataViewBag()
        {
            ViewBag.AccontData = AccountData;
        }

        //列表页
        [Authorize(Roles = SysRole.EnterpriseManageRead + "," + SysRole.EEnterpriseManageRead)]
        public ActionResult Index(string filter, bool? enable = null, int page = 1)
        {
            Sidebar();
            //int usertype = (int)this.GetAccountData().UserType;//从cookie中读取用户类型
            //string userID = this.GetAccountData().UserID;//从cookie中读取userid
            //var user = db.Users.FirstOrDefault(s => s.Id == userID);
            var m = from e in db.Enterprises
                    from u in db.Users
                    where e.AdminID == u.Id
                    select new EnterpriseShowViewModels
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
            if (AccountData.UserType == UserType.Enterprise)
            {
                m = m.Where(s => s.ID == AccountData.EnterpriseID);
            }
            var paged = m.OrderByDescending(s => s.AdminID).ToPagedList(page);
            return View(paged);
        }

        //企业首页
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
        [Authorize(Roles = SysRole.EnterpriseManageCogradient + "," + SysRole.EEnterpriseManageCogradient)]
        public ActionResult CogradientWXUserInfo(int? id, int page = 1)
        {
            Sidebar();
            if (id == null)
            {
                ViewBag.errormsg = "错误提示：企业不存在";
                return View();
            }
            var em = db.Enterprises.FirstOrDefault(s => s.ID == id.Value);

            ViewBag.code = em.Code;
            ViewBag.name = em.Name;
            ViewBag.logo = em.Logo;
            ViewBag.enterpriseid = em.ID;

            List<User> list = new List<Common.WeChatWork.Models.User>();
            //错误的时候返回该model
            PagedList.PagedList<Common.WeChatWork.Models.User> model = new PagedList<Common.WeChatWork.Models.User>(list, page, 15);
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
                    List<User> listwxuser = new List<Common.WeChatWork.Models.User>();
                    //var m= db.Cards.Select(s => s.EnterpriseID == em.ID) as List<Card>;
                    List<Card> listcard = db.Cards.Where(s => s.EnterpriseID == em.ID).ToList();
                    if (listdepartment.Count > 0)
                    {
                        for (int i = 0; i < listdepartment.Count; i++)
                        {
                            List<User> listt = wxapi.GetUsesByDepID(listdepartment[i].ID);
                            listwxuser.AddRange(listt);
                        }
                    }
                    if (listcard != null)
                    {
                        //遍历数据库该公司原有的用户和微信获取到的用户进行比对，如果数据库已存在则标识为已存在 ishave=true 
                        for (int i = 0; i < listcard.Count; i++)
                        {
                            for (int j = 0; j < listwxuser.Count; j++)
                            {
                                if (listcard[i].Mobile == listwxuser[j].Mobile)
                                {
                                    listwxuser[j].ishave = true;
                                    listwxuser[j].ischeck = true;
                                }
                            }
                        }
                    }
                    var paged = listwxuser.OrderByDescending(s => s.ID).ToList();
                    PagedList.PagedList<Common.WeChatWork.Models.User> models = new PagedList<Common.WeChatWork.Models.User>(paged, page, 15);
                    return View(models);
                }
                catch (Exception ex)
                {
                    ViewBag.errormsg = "错误提示：" + ex.ToString();
                    return View(model);
                }
            }
        }
        [HttpPost]
        [AllowCrossSiteJson]
        [Authorize(Roles = SysRole.EnterpriseManageCogradient + "," + SysRole.EEnterpriseManageCogradient)]
        public async Task<ActionResult> CogradientWXUserInfo(string listu, int enterpriseid)
        {
            Sidebar();
            var users = JsonConvert.DeserializeObject<List<User>>(listu);//post过来需要保存数据库的用户数据
            int usertype = (int)this.GetAccountData().UserType;//从cookie中读取用户类型
            string userID = this.GetAccountData().UserID;//从cookie中读取userid
            var user = db.Users.FirstOrDefault(s => s.Id == userID);
            var em = db.Enterprises.FirstOrDefault(s => s.ID == enterpriseid);
            int cardcount = em.CardCount;

            if (cardcount > users.Count)
            {
                foreach (var item in users)
                {
                    try
                    {
                        //判断数据库中是否存在(只保存不存在的数据)
                        if (!item.ishave)
                        {
                            var img = this.Download(item.Avatar);
                            string username = item.Mobile;
                            var tempuser = new ApplicationUser { UserName = item.Mobile, RegisterDateTime = DateTime.Now, EnterpriseID = em.ID, LastLoginDateTime = DateTime.Now, UserType = UserType.Personal };
                            var result = await UserManager.CreateAsync(tempuser, item.Mobile);
                            //用户创建成果
                            if (result.Succeeded)
                            {
                                Common.Qiniu.QinQiuApi qin = new Common.Qiniu.QinQiuApi();
                                var path = Server.MapPath(img);
                                var img2 = qin.UploadFile(path);
                                Card card = new Card
                                {
                                    Name = item.Name,
                                    Avatar = img2,
                                    UserID = tempuser.Id,
                                    Email = item.Email,
                                    EnterpriseID = em.ID,
                                    Gender = item.Gender,
                                    Mobile = item.Mobile,
                                    PhoneNumber = item.Telephone,
                                    Position = item.Position
                                };
                                db.Cards.Add(card);
                                db.SaveChanges();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(Comm.ToJsonResult("Success", "同步成功", new { data = users }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Comm.ToJsonResult("Fail", "该企业剩余名片数量为" + cardcount.ToString() + ",可用数量不够！", new { data = users }), JsonRequestBehavior.AllowGet);
            }
        }

        //新增
        [Authorize(Roles = SysRole.EnterpriseManageCreate)]
        public ActionResult Create()
        {
            Sidebar();
            ViewBag.Province = ChinaPCAS.GetP();//省份
            ViewBag.City = ChinaPCAS.GetC("北京市");//城市
            ViewBag.District = ChinaPCAS.GetA("北京市", "东城区");//街道
            var model = new EnterpriseViewModels();
            return View(model);
        }
        // POST: tests/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SysRole.EnterpriseManageCreate)]
        public async Task<ActionResult> Create(EnterpriseViewModels enterprise)
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
                    //查询企业权限
                    var listrg = RoleManager.Roles.Where(s => s.Type == RoleType.Enterprise).ToList();

                    //为企业单独设定一个管理员权限
                    RoleGroup rg = new RoleGroup
                    {
                        Name = enterprise.Name + "系统管理员",
                        //拼接RoleGroup表需要的Roles字段值
                        Roles = string.Join(",", listrg.Select(s => s.Name))
                    };
                    db.RoleGroups.Add(rg);
                    int saveresult = db.SaveChanges();
                    //saveresult>0表示新增权限组信息成功
                    if (saveresult > 0)
                    {
                        var rgid = rg.ID;
                        //用企业编号自动创建一个企业管理账号,默认密码为"123456"
                        string username = enterprise.Code + "admin";
                        var user = new ApplicationUser
                        {
                            UserName = username,
                            RegisterDateTime = DateTime.Now,
                            LastLoginDateTime = DateTime.Now,
                            RoleGroupID = rgid,
                            UserType = UserType.Enterprise
                        };
                        var result = await UserManager.CreateAsync(user, "123456");
                        //创建企业管理员账号成功
                        if (result.Succeeded)
                        {
                            //给企业管理添加权限
                            UserManager.AddToRoles(user.Id, listrg.Select(s => s.Name).ToArray());
                            var model = new Enterprise
                            {
                                AdminID = user.Id,
                                Address = enterprise.Address.Address,
                                AppID = enterprise.AppID,
                                CardCount = enterprise.CardCount,
                                City = enterprise.City,
                                Code = enterprise.Code,
                                District = enterprise.District,
                                Email = enterprise.Email,
                                Enable = enterprise.Enable,
                                HomePage = enterprise.HomePage,
                                Info = enterprise.Info,
                                Lng = enterprise.Address.Lng,
                                Lat = enterprise.Address.Lat,
                                Logo = enterprise.Logo.ImageUrl,
                                Name = enterprise.Name,
                                PhoneNumber = enterprise.PhoneNumber,
                                Province = enterprise.Province,
                                WeChatWorkCorpid = enterprise.WeChatWorkCorpid,
                                WeChatWorkSecret = enterprise.WeChatWorkSecret,
                                RegisterDateTime = DateTime.Now
                            };
                            db.Enterprises.Add(model);
                            int r = db.SaveChanges();
                            //r>0表示保存企业信息成功
                            if (r > 0)
                            {
                                var renterprise = db.Enterprises.FirstOrDefault(s => s.Code == enterprise.Code);
                                var t = db.RoleGroups.FirstOrDefault(s => s.ID == rgid);
                                t.EnterpriseID = renterprise.ID;
                                var u = db.Users.FirstOrDefault(s => s.Id == user.Id);
                                u.EnterpriseID = renterprise.ID;
                                db.SaveChanges();
                            }
                            return RedirectToAction("Index");
                        }
                    }
                }
            }
            Sidebar();
            AccountDataViewBag();
            return View(enterprise);
        }
        [Authorize(Roles = SysRole.EnterpriseManageEdit + "," + SysRole.EEnterpriseManageEdit)]
        public ActionResult Edit(int? id)
        {
            AccountDataViewBag();
            var temp = db.Enterprises.FirstOrDefault(s => s.ID == id);
            //防止企业用户串号修改
            if (AccountData.UserType == UserType.Enterprise
                && temp.ID != AccountData.EnterpriseID)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
            Sidebar();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var model = (from e in db.Enterprises
                         from u in db.Users
                         where e.AdminID == u.Id && e.ID == id.Value
                         select e).FirstOrDefault();
            var models = new EnterpriseViewModels
            {
                ID = model.ID,
                Code = model.Code,
                Province = model.Province,
                City = model.City,
                Email = model.Email,
                HomePage = model.HomePage,
                Info = model.Info,
                District = model.District,
                Enable = model.Enable,
                CardCount = model.CardCount,
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
                AdminID = model.AdminID,
                RegisterDateTime = model.RegisterDateTime,
                AppID = model.AppID
                //Lat = model.Lat,
                //Lng = model.Lng
            };
            models.Logo.ImageUrl = model.Logo;
            models.Address.Address = model.Address;
            models.Address.Lat = model.Lat;
            models.Address.Lng = model.Lng;
            ViewBag.Province = ChinaPCAS.GetP();//省份
            ViewBag.City = ChinaPCAS.GetC(model.Province);//城市
            ViewBag.District = ChinaPCAS.GetA(model.Province, model.City);//街道
            Map te = new Map
            {
                Address = model.Address,
                Lat = model.Lat,
                Lng = model.Lng
            };
            models.Address = new Map(te);
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
        [Authorize(Roles = SysRole.EnterpriseManageEdit + "," + SysRole.EEnterpriseManageEdit)]
        public ActionResult Edit(EnterpriseViewModels enterprise)
        {
            var temp = db.Enterprises.FirstOrDefault(s => s.ID == enterprise.ID);
            //防止企业用户串号修改
            if (AccountData.UserType == UserType.Enterprise
                && temp.ID != AccountData.EnterpriseID)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
            if (AccountData.EnterpriseID != 0)
            {
                ModelState.Remove("Code");
            }

            if (ModelState.IsValid)
            {
                var t = db.Enterprises.FirstOrDefault(s => s.ID == enterprise.ID);
                t.Name = enterprise.Name;
                if (AccountData.EnterpriseID != 0)
                {
                    t.Code = enterprise.Code;
                    t.CardCount = enterprise.CardCount;
                }
                t.Province = enterprise.Province;
                t.City = enterprise.City;
                t.Address = enterprise.Address.Address;
                t.Email = enterprise.Email;
                t.District = enterprise.District;
                t.HomePage = enterprise.HomePage;
                t.Info = enterprise.Info;
                t.Enable = enterprise.Enable;
                t.PhoneNumber = enterprise.PhoneNumber;
                t.Logo = enterprise.Logo.ImageUrl; //enterprise.Logo;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            Sidebar();
            ViewBag.Province = ChinaPCAS.GetP();//省份
            ViewBag.City = ChinaPCAS.GetC(enterprise.Province);//城市
            ViewBag.District = ChinaPCAS.GetA(enterprise.Province, enterprise.City);//街道
            ViewBag.AccontData = AccountData;
            return View(enterprise);
        }
        //删除
        [Authorize(Roles = SysRole.EnterpriseManageDelete)]
        public ActionResult Delete(int id)
        {
            Enterprise enterprise = db.Enterprises.Find(id);
            //防止企业用户串号修改
            if (AccountData.UserType == UserType.Enterprise
                && enterprise.ID != AccountData.EnterpriseID)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
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
        [Authorize(Roles = SysRole.EnterpriseManageDeploy + "," + SysRole.EEnterpriseManageDeploy)]
        public ActionResult Deploy(int? id)
        {
            var temp = db.Enterprises.FirstOrDefault(s => s.ID == id.Value);
            //防止企业用户串号修改
            if (AccountData.UserType == UserType.Enterprise
                && temp.ID != AccountData.EnterpriseID)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
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
        [Authorize(Roles = SysRole.EnterpriseManageDeploy + "," + SysRole.EEnterpriseManageDeploy)]
        public ActionResult Deploy(Enterprise enterprise)
        {
            var temp = db.Enterprises.FirstOrDefault(s => s.ID == enterprise.ID);
            //防止企业用户串号修改
            if (AccountData.UserType == UserType.Enterprise
                && temp.ID != AccountData.EnterpriseID)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
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