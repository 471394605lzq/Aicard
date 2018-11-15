using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
using AiCard.Common.Enums;
using AiCard.DAL.Models;
using AiCard.Common.Extensions;
using AiCard.Common;

namespace AiCard.Controllers
{
    [Authorize]
    public class HomePageModularsManageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public int EnterpriseID
        {
            get
            {
                return this.GetAccountData().EnterpriseID;
            }
        }

        public void Sidebar()
        {
            ViewBag.Sidebar = "公司主页";

        }


        [Authorize(Roles = SysRole.EHomePageModularsManageRead)]
        public ActionResult Index(int? enterpriseID)
        {
            var eid = EnterpriseID == 0 ? enterpriseID.Value : EnterpriseID;
            Sidebar();
            var filter = db.HomePageModulars
                .Where(s => s.EnterpriseID == eid)
                .Select(s => new
                {
                    s.ID,
                    s.Sort,
                    s.Title,
                    s.Type,
                    s.EnterpriseID,
                    s.Content
                });
            if (!filter.Any(s => s.Type == HomePageModularType.Banner))
            {
                db.HomePageModulars.Add(new HomePageModular
                {
                    EnterpriseID = eid,
                    Sort = -1,
                    Title = "顶部轮播图",
                    Type = HomePageModularType.Banner
                });
                db.HomePageModulars.Add(new HomePageModular
                {
                    EnterpriseID = eid,
                    Sort = 1,
                    Title = "联系方式",
                    Type = HomePageModularType.Contact
                });
                db.SaveChanges();
            }

            var model = filter.OrderBy(s => s.Sort).ToList()
                .Select(s => new HomePageModular
                {
                    Content = s.Content,
                    Sort = s.Sort,
                    EnterpriseID = s.EnterpriseID,
                    ID = s.ID,
                    Title = s.Title,
                    Type = s.Type
                });
            return View(model);
        }


        #region Html
        [Authorize(Roles = SysRole.EHomePageModularsManageCreate)]
        public ActionResult CreateByHtml()
        {
            Sidebar();
            var model = new HomePageModularByHtml();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = SysRole.EHomePageModularsManageCreate)]
        public ActionResult CreateByHtml(HomePageModularByHtml model)
        {
            if (!ModelState.IsValid)
            {
                Sidebar();
                return View(model);
            }
            var eId = EnterpriseID;
            var maxSort = db.HomePageModulars.Where(s => s.EnterpriseID == eId).Max(s => s.Sort) + 1;
            HomePageModular modular = new HomePageModular
            {
                Content = model.Content,
                EnterpriseID = eId,
                Sort = maxSort,
                Title = model.Title,
                Type = model.Type
            };
            db.HomePageModulars.Add(modular);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = SysRole.EHomePageModularsManageEdit)]
        public ActionResult EditByHtml(int id)
        {
            Sidebar();
            var eId = EnterpriseID;
            var m = db.HomePageModulars
                .FirstOrDefault(s => s.ID == id
                    && s.EnterpriseID == eId);

            if (m == null)
            {
                return this.ToError("错误", "模块不存在", Url.Action("Index"));
            }
            var model = new HomePageModularByHtml()
            {
                Content = m.Content,
                ID = m.ID,
                Title = m.Title
            };
            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = SysRole.EHomePageModularsManageCreate)]
        public ActionResult EditByHtml(HomePageModularByHtml model)
        {
            if (!ModelState.IsValid)
            {
                Sidebar();
                return View(model);
            }
            var eId = EnterpriseID;
            var m = db.HomePageModulars
                .FirstOrDefault(s => s.ID == model.ID
                    && s.EnterpriseID == eId);
            if (m == null)
            {
                return this.ToError("错误", "模块不存在", Url.Action("Index"));
            }
            m.Content = model.Content;
            m.Title = model.Title;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion

        #region Image
        [Authorize(Roles = SysRole.EHomePageModularsManageCreate)]
        public ActionResult CreateByImage()
        {
            Sidebar();
            var model = new HomePageModularByImage();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = SysRole.EHomePageModularsManageCreate)]
        public ActionResult CreateByImage(HomePageModularByImage model)
        {
            if (!ModelState.IsValid)
            {
                Sidebar();
                return View(model);
            }
            var eId = EnterpriseID;
            var maxSort = db.HomePageModulars.Where(s => s.EnterpriseID == eId).Max(s => s.Sort) + 1;
            HomePageModular modular = new HomePageModular
            {
                Content = string.Join(",", model.Images.Images),
                EnterpriseID = eId,
                Sort = maxSort,
                Title = model.Title,
                Type = model.Type
            };
            db.HomePageModulars.Add(modular);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = SysRole.EHomePageModularsManageEdit)]
        public ActionResult EditByImage(int id)
        {
            Sidebar();
            var eId = EnterpriseID;
            var m = db.HomePageModulars
                .FirstOrDefault(s => s.ID == id
                    && s.EnterpriseID == eId);

            if (m == null)
            {
                return this.ToError("错误", "模块不存在", Url.Action("Index"));
            }
            var model = new HomePageModularByImage()
            {
                ID = m.ID,
                Title = m.Title
            };
            model.Images.Images = m.Content.SplitToArray<string>(',')?.ToArray() ?? new string[0];
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = SysRole.EHomePageModularsManageCreate)]
        public ActionResult EditByImage(HomePageModularByImage model)
        {
            if (!ModelState.IsValid)
            {
                Sidebar();
                return View(model);
            }
            var eId = EnterpriseID;
            var m = db.HomePageModulars
                .FirstOrDefault(s => s.ID == model.ID
                    && s.EnterpriseID == eId);
            if (m == null)
            {
                return this.ToError("错误", "模块不存在", Url.Action("Index"));
            }
            m.Content = string.Join(",", model.Images.Images);
            m.Title = model.Title;

            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion

        #region Contact
        [Authorize(Roles = SysRole.EHomePageModularsManageCreate)]
        public ActionResult CreateByContact()
        {
            Sidebar();
            var contact = db.HomePageModulars
                .FirstOrDefault(s => s.EnterpriseID == EnterpriseID
                && s.Type == HomePageModularType.Contact);
            if (contact != null)
            {
                return this.ToError("警告", "联系方式模块已存在", Url.Action("Index"));
            }
            else
            {
                var enterprise = db.Enterprises.FirstOrDefault(s => s.ID == EnterpriseID);
                var modlur = new HomePageModularByContact
                {
                    Address = $"{enterprise.Province}{enterprise.City}{enterprise.District}{enterprise.Address}",
                    Email = enterprise.Email,
                    Phone = enterprise.PhoneNumber,
                    Title = "联系方式",
                };
                return View(modlur);
            }

        }

        [HttpPost]
        [Authorize(Roles = SysRole.EHomePageModularsManageCreate)]
        public ActionResult CreateByContact(HomePageModularByContact model)
        {
            Sidebar();
            var contact = db.HomePageModulars
                .FirstOrDefault(s => s.EnterpriseID == EnterpriseID
                && s.Type == HomePageModularType.Contact);
            if (contact != null)
            {
                return this.ToError("警告", "联系方式模块已存在", Url.Action("Index"));
            }
            else
            {
                var enterprise = db.Enterprises.FirstOrDefault(s => s.ID == EnterpriseID);
                var modular = new HomePageModular
                {
                    Type = model.Type,
                    Sort = db.HomePageModulars.Where(s => s.EnterpriseID == EnterpriseID).Max(s => s.Sort) + 1,
                    Title = model.Title,
                    EnterpriseID = EnterpriseID,
                };
                db.HomePageModulars.Add(modular);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

        }

        [Authorize(Roles = SysRole.EHomePageModularsManageEdit)]
        public ActionResult EditByContact(int id)
        {
            Sidebar();
            var contact = db.HomePageModulars
                .FirstOrDefault(s => s.ID == id 
                && s.EnterpriseID == EnterpriseID
                && s.Type == HomePageModularType.Contact);
            if (contact == null)
            {
                return this.ToError("错误", "模块不存在", Url.Action("Index"));
            }
            else
            {
                var enterprise = db.Enterprises.FirstOrDefault(s => s.ID == EnterpriseID);
                var modlur = new HomePageModularByContact
                {
                    Address = $"{enterprise.Province}{enterprise.City}{enterprise.District}{enterprise.Address}",
                    Email = enterprise.Email,
                    Phone = enterprise.PhoneNumber,
                    Title = contact.Title,
                };
                return View(modlur);
            }

        }

        [HttpPost]
        [Authorize(Roles = SysRole.EHomePageModularsManageEdit)]
        public ActionResult EditByContact(HomePageModularByContact model)
        {
            Sidebar();
            var contact = db.HomePageModulars
                .FirstOrDefault(s => s.ID == model.ID 
                && s.EnterpriseID == EnterpriseID
                && s.Type == HomePageModularType.Contact);
            if (contact == null)
            {
                return this.ToError("错误", "模块不存在", Url.Action("Index"));
            }
            else
            {
                contact.Title = model.Title;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

        }
        #endregion

        [HttpPost]
        [Authorize(Roles = SysRole.EHomePageModularsManageDelete)]
        public ActionResult Delete(int id)
        {
            var modular = db.HomePageModulars
                .FirstOrDefault(s => s.ID == id && s.EnterpriseID == EnterpriseID);
            if (modular == null)
            {
                return Json(Comm.ToJsonResult("ModularNoFound", "成功"));
            }
            else
            {
                db.HomePageModulars.Remove(modular);
                db.SaveChanges();
                return Json(Comm.ToJsonResult("Success", "成功"));
            }
        }

        [HttpPost]
        [Authorize(Roles = SysRole.EHomePageModularsManageEdit)]
        public ActionResult Sort(string ids)
        {
            var idsList = ids.SplitToArray<int>();
            var modular = db.HomePageModulars
                .Where(s => s.EnterpriseID == EnterpriseID
                    && idsList.Contains(s.ID))
                .ToList();
            foreach (var item in modular)
            {
                var i = idsList.FirstOrDefault(s => s == item.ID);
                var index = idsList.IndexOf(i);
                if (index >= 0)
                {
                    item.Sort = index;
                }
            }
            var res = db.SaveChanges();
            return Json(Comm.ToJsonResult("Success", "成功"));
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