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
using System.Net;
using AiCard.WeChat;
using AiCard.Models.CommModels;
using AiCard.Common.Enums;
using AiCard.DAL.Models;

namespace AiCard.Controllers
{
    [Authorize]
    public class CardManageController : Controller
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
        //获取当前登录的用户信息
        AccountData AccontData
        {
            get
            {
                return this.GetAccountData();
            }
        }
        public void Sidebar(string name = "名片管理")
        {
            ViewBag.Sidebar = name;
        }
        //查询名片列表信息
        // GET: Card
        [Authorize(Roles = SysRole.CardManageRead + "," + SysRole.ECardManageRead)]
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
            //如果是企业用户则只查询该企业信息
            if (AccontData.UserType == UserType.Enterprise)
            {
                m = m.Where(s => s.EnterpriseID == AccontData.EnterpriseID);
            }
            var paged = m.OrderByDescending(s => s.ID).ToPagedList(page);
            return View(paged);
        }
        //新增
        [Authorize(Roles = SysRole.CardManageCreate + "," + SysRole.ECardManageCreate)]
        public ActionResult Create()
        {
            var tempuser = db.Users.FirstOrDefault(s => s.Id == AccontData.UserID);
            //防止企业用户串号修改
            if (AccontData.UserType == UserType.Enterprise
                && tempuser.EnterpriseID != AccontData.EnterpriseID)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
            Sidebar();
            var model = new CardCreateEditViewModel();
            model.Sort = (db.Cards
                .Where(s => s.EnterpriseID == AccontData.EnterpriseID)
                .Max(s => (int?)s.Sort) ?? 0) / 10 * 10 + 10;
            return View(model);
        }
        // POST: tests/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SysRole.CardManageCreate + "," + SysRole.ECardManageCreate)]
        public async Task<ActionResult> Create(CardCreateEditViewModel model)
        {
            Sidebar();
            if (ModelState.IsValid)
            {
                var hasname = db.Cards.Any(s => s.Name == model.Name);
                //检验名片是否存在
                if (hasname)
                {
                    ModelState.AddModelError("Name", "名片已存在");
                    return View(model);
                }
                else
                {
                    var tempuser = db.Users.FirstOrDefault(s => s.Id == AccontData.UserID);
                    //防止企业用户串号修改
                    if (AccontData.UserType == UserType.Enterprise
                        && tempuser.EnterpriseID != AccontData.EnterpriseID)
                    {
                        return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
                    }
                    //用名片用户的手机号创建一个账号,默认密码为手机号
                    var user = new ApplicationUser { UserName = model.Mobile, RegisterDateTime = DateTime.Now, EnterpriseID = AccontData.EnterpriseID, LastLoginDateTime = DateTime.Now, UserType = UserType.Personal };
                    var result = await UserManager.CreateAsync(user, model.Mobile);
                    //创建名片账号成功
                    if (result.Succeeded)
                    {
                        var card = new Card
                        {
                            Avatar = string.Join(",", model.Avatar.Images),
                            UserID = user.Id,
                            EnterpriseID = AccontData.EnterpriseID,
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
                            Video = string.Join(",", model.Video.Images),
                            Voice = string.Join(",", model.Voice.Images),
                            WeChatCode = model.WeChatCode,
                            Sort = model.Sort
                        };
                        db.Cards.Add(card);
                        db.SaveChanges();
                        var enterprise = db.Enterprises.FirstOrDefault(s => s.ID == AccontData.EnterpriseID);
                        enterprise.CardCount--;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(model);
        }
        //加载编辑数据
        [Authorize(Roles = SysRole.CardManageEdit + "," + SysRole.ECardManageEdit)]
        public ActionResult Edit(int? id)
        {
            Sidebar();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var temp = db.Cards.FirstOrDefault(s => s.ID == id);
            //防止企业用户串号修改
            if (AccontData.UserType == UserType.Enterprise
                && temp.EnterpriseID != AccontData.EnterpriseID)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
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
                //Video = model.Video,
                //Voice = model.Voice,
                WeChatCode = model.WeChatCode,
                UserID = model.UserID,
                EnterpriseID = model.EnterpriseID,
                //AdminName = model.User.UserName,
                Sort = model.Sort
            };
            models.Avatar.Images = model.Avatar?.Split(',') ?? new string[0];
            models.Images.Images = model.Images?.Split(',') ?? new string[0];
            models.Video.Images = model.Video?.Split(',') ?? new string[0];
            models.Voice.Images = model.Voice?.Split(',') ?? new string[0];
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
        [Authorize(Roles = SysRole.CardManageEdit + "," + SysRole.ECardManageEdit)]
        public ActionResult Edit(CardCreateEditViewModel model)
        {
            Sidebar();
            var temp = db.Cards.FirstOrDefault(s => s.ID == model.ID);

            var tempenterprise = db.Enterprises.FirstOrDefault(s => s.ID == temp.EnterpriseID);
            WeChatMinApi w = new WeChatMinApi(ConfigMini.AppID, ConfigMini.AppSecret);
            //防止企业用户串号修改
            if (AccontData.UserType == UserType.Enterprise
                && temp.EnterpriseID != AccontData.EnterpriseID)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
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
                t.Voice = string.Join(",", model.Voice.Images);
                t.Video = string.Join(",", model.Video.Images);
                t.Images = string.Join(",", model.Images.Images);
                t.Sort = model.Sort;
                if (string.IsNullOrWhiteSpace(temp.WeChatMiniQrCode))
                {
                    var scene = new Dictionary<string, string>();
                    scene.Add("CardId", model.ID.ToString());
                    t.WeChatMiniQrCode = w.GetWXACodeUnlimit(WeChatPage.CardDetail, scene);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        //删除
        [Authorize(Roles = SysRole.CardMangeDelete + "," + SysRole.ECardManageDelete)]
        public ActionResult Delete(int id)
        {
            var temp = db.Cards.FirstOrDefault(s => s.ID == id);
            //防止企业用户串号修改
            if (AccontData.UserType == UserType.Enterprise
                && temp.EnterpriseID != AccontData.EnterpriseID)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
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



        //public ActionResult GetPoster(int id)
        //{
        //    try
        //    {
        //        var query = (from c in db.Cards
        //                     from e in db.Enterprises
        //                     where c.EnterpriseID == e.ID && c.ID == id
        //                     select c).FirstOrDefault();
        //        if (query == null)
        //        {
        //            return Json(Comm.ToJsonResult("Error", "该名片不存在"), JsonRequestBehavior.AllowGet);
        //        }
        //        var qe = (from e in db.Enterprises
        //                  where e.ID == query.EnterpriseID
        //                  select e).FirstOrDefault();

        //        var cardm = (from ct in db.CardTabs
        //                     where ct.CardID == id
        //                     orderby ct.Count descending
        //                     select new
        //                     {
        //                         Name = ct.Name,
        //                         Count = ct.Count,
        //                         Style = ct.Style
        //                     }).Take(2).ToList();
        //        List<TagModel> listst = new List<TagModel>();
        //        if (cardm.Count > 0)
        //        {
        //            for (int i = 0; i < cardm.Count; i++)
        //            {
        //                if (i == 0)
        //                {
        //                    TagModel tm = new TagModel();
        //                    tm.TagName = cardm[i].Name + " " + cardm[i].Count.ToString();
        //                    tm.TagStyle = cardm[i].Style;
        //                    listst.Add(tm);
        //                }
        //            }
        //        }
        //        var dm = new DrawingPictureModel
        //        {
        //            AvatarPath = string.IsNullOrWhiteSpace(query.Avatar) ? "": DrawingPictures.DownloadImg(query.Avatar, "avatar.png", 834, 834),
        //            CompanyName = qe.Name,
        //            LogoPath = string.IsNullOrWhiteSpace(qe.Logo) ? "": DrawingPictures.DownloadImg(qe.Logo, "logo.png", 96, 96),
        //            Position = query.Position,
        //            QrPath = string.IsNullOrWhiteSpace(query.WeChatMiniQrCode) ? "":DrawingPictures.DownloadImg(query.WeChatMiniQrCode, "qrcode.png", 240, 240),
        //            Remark = query.Remark,
        //            UserName = query.Name,
        //            TagList = listst,
        //            PosterImageName = "cardid_" + id.ToString()
        //        };
        //        //调用生成海报方法
        //        string returnpath = Comm.MergePosterImage(dm);
        //        var data = new
        //        {
        //            Posterpath = returnpath
        //        };
        //        return Json(Comm.ToJsonResult("Success", "成功", data), JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
        //    }
        //}
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