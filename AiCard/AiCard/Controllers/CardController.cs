using AiCard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using AiCard.Enums;
using System.Net;
namespace AiCard.Controllers
{
    [Authorize]
    public class CardController : Controller
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
        public void Sidebar(string name = "名片管理")
        {
            ViewBag.Sidebar = name;
        }
        //查询名片列表信息
        // GET: Card
        public ActionResult Index(string filter, bool? enable = null, int page = 1)
        {
            Sidebar();
            var m = db.Cards.Include(s => s.Enterprise).Include(s => s.User);
            if (!string.IsNullOrWhiteSpace(filter))
            {
                m = m.Where(s => s.Name.Contains(filter));
            }
            if (enable != null)
            {
                m = m.Where(s => s.Enable == enable.Value);
            }
            var paged = m.OrderByDescending(s => s.ID).ToPagedList(page);
            return View(paged);
        }
        //新增
        public ActionResult Create()
        {
            Sidebar();
            var model = new CardCreateEditViewModel();
            return View(model);
        }
        // POST: tests/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CardCreateEditViewModel model)
        {
            Sidebar();
            if (ModelState.IsValid)
            {
                var hasname = db.Cards.Any(s => s.Name == model.Name);
                //检验企业编号是否存在
                if (hasname)
                {
                    ModelState.AddModelError("Code", "名片已存在");
                    return View(model);
                }
                else
                {
                    string cookieuserid = Request.Cookies["UserId"].Value;//Request.Cookies["UserId"].ToString();//从cookie中读取userid
                    string decryptuserid = Comm.Decrypt(cookieuserid);
                    var tempuser = db.Users.FirstOrDefault(s => s.Id == decryptuserid);
                    //用名片用户的手机号创建一个账号,默认密码为手机号
                    string username = model.Mobile;
                    var user = new ApplicationUser { UserName = model.Mobile, RegisterDateTime = DateTime.Now, EnterpriseID = tempuser.EnterpriseID, LastLoginDateTime = DateTime.Now, UserType = UserType.Personal };
                    var result = await UserManager.CreateAsync(user, model.Mobile);
                    //创建名片账号成功
                    if (result.Succeeded)
                    {
                        var card = new Card
                        {
                            Avatar = string.Join(",", model.Avatar.Images),
                            UserID = user.Id,
                            EnterpriseID = tempuser.EnterpriseID,
                            Name = model.Name,
                            Enable = model.Enable,
                            Email = model.Email,
                            Gender = model.Gender,
                            Images = string.Join(",", model.Images.Images),
                            Mobile = model.Mobile,
                            Info = model.Info,
                            PhoneNumber = model.PhoneNumber,
                            Position = model.Position,
                            Remark = model.Remark,
                            Video = model.Video,
                            Voice = model.Voice,
                            WeChatCode = model.WeChatCode
                        };
                        db.Cards.Add(card);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(model);
        }
        //加载编辑数据
        public ActionResult Edit(int? id)
        {
            Sidebar();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //var models = db.Cards.Include(s => s.Enterprise).Include(s => s.User).Where(s=>s.ID==id.Value);
            var model = (from e in db.Cards
                         from u in db.Users
                         from en in db.Enterprises
                         where e.UserID == u.Id && e.ID == id.Value && e.EnterpriseID == en.ID
                         select e).FirstOrDefault();
            var models = new CardCreateEditViewModel
            {
                ID = model.ID,
                Name = model.Name,
                Email = model.Email,
                Info = model.Info,
                Enable = model.Enable,
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender,
                Mobile = model.Mobile,
                Position = model.Position,
                Remark = model.Remark,
                Video = model.Video,
                Voice = model.Voice,
                WeChatCode = model.WeChatCode,
                UserID = model.UserID,
                EnterpriseID = model.EnterpriseID,
                //AdminName = model.User.UserName,
            };
            models.Avatar.Images = model.Avatar?.Split(',') ?? new string[0];
            models.Images.Images = model.Images?.Split(',') ?? new string[0];
            if (models == null)
            {
                return HttpNotFound();
            }
            return View(models);
        }

        //修改名片信息
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CardCreateEditViewModel model)
        {
            Sidebar();
            if (ModelState.IsValid)
            {
                var t = db.Cards.FirstOrDefault(s => s.ID == model.ID);
                t.Name = model.Name;
                t.Gender = model.Gender;
                t.Avatar = string.Join(",", model.Avatar.Images);
                t.PhoneNumber = model.PhoneNumber;
                t.Email = model.Email;
                t.WeChatCode = model.WeChatCode;
                t.Mobile = model.Mobile;
                t.Position = model.Position;
                t.Remark = model.Remark;
                t.Info = model.Info;
                t.Enable = model.Enable;
                t.Voice = model.Voice;
                t.Video = model.Video;
                t.Images = string.Join(",", model.Images.Images);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        //删除
        public ActionResult Delete(int id)
        {
            Card card = db.Cards.Find(id);
            var userid = card.UserID;
            //删除跟企业关联的管理员账号信息
            var user = db.Users.FirstOrDefault(s => s.Id == userid);
            if (user != null)
            {
                db.Users.Remove(user);
            }
            //删除企业信息
            if (card != null)
            {
                db.Cards.Remove(card);
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}