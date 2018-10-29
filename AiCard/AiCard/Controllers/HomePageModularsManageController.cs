using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
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
            ViewBag.Sidebar = "公司首页";

        }


        [Authorize(Roles = SysRole.EHomePageModularsManageRead)]
        public ActionResult Index(int? enterpriseID)
        {
            var eid = EnterpriseID == 0 ? enterpriseID.Value : EnterpriseID;
            Sidebar();
            var model = db.HomePageModulars
                .Where(s => s.EnterpriseID == eid)
                .OrderBy(s => s.Sort)
                .Select(s => new HomePageModular
                {
                    ID = s.ID,
                    Sort = s.Sort,
                    Title = s.Title,
                    Type = s.Type
                })
                .ToList();
            //if (model.Any(s => s.Type == Enums.HomePageModularType.Banner))
            //{
            //    model.Add(new HomePageModular
            //    {
            //        EnterpriseID = eid,
            //        Sort = -1,
            //        Title = "顶部轮播图",
            //        Type = Enums.HomePageModularType.Banner
            //    });
            //    model.Add(new HomePageModular
            //    {
            //        EnterpriseID = eid,
            //        Sort = 1,
            //        Title = "联系方式",
            //        Type = Enums.HomePageModularType.Contact
            //    });
            //    db.SaveChanges();
            //}
            model = model.OrderBy(s => s.Sort).ToList();
            return View(model);
        }


        [Authorize(Roles = SysRole.EnterpriseManageCreate)]
        public ActionResult CreateByHtml()
        {
            var model = new HomePageModularByHtml();
            return View(model);
        }

        [Authorize(Roles = SysRole.EnterpriseManageCreate)]
        public ActionResult CreateByImage()
        {
            var model = new HomePageModularByImage();
            return View(model);
        }



        //不同类型的模块提交时候都统一提交到这里
        [HttpPost]
        [Authorize(Roles = SysRole.EnterpriseManageCreate)]
        public ActionResult Create(IHomePageModular model)
        {
            if (!ModelState.IsValid)
            {
                return Json(Comm.ToJsonResult("Error", ModelState.FirstErrorMessage()));
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
            return Json(Comm.ToJsonResult("Success", "成功"));
        }

        [Authorize(Roles = SysRole.EHomePageModularsManageEdit)]
        public ActionResult Edit(IHomePageModular model)
        {
            if (!ModelState.IsValid)
            {
                return Json(Comm.ToJsonResult("Error", ModelState.FirstErrorMessage()));
            }
            var eId = EnterpriseID;
            var modular = db.HomePageModulars
                .FirstOrDefault(s => s.ID == model.ID
                    && s.EnterpriseID == EnterpriseID);
            if (modular == null)
            {
                return Json(Comm.ToJsonResult("NoFound", "模块不存在"));
            }
            modular.Content = model.Content;
            modular.Title = model.Title;
            //HomePageModular modular = new HomePageModular
            //{
            //    Content = model.Content,
            //    EnterpriseID = eId,
            //    Sort = model,
            //    Title = model.Title,
            //    Type = model.Type
            //};
            return Json(Comm.ToJsonResult("Success", "成功"));
        }

        [HttpPost]
        [Authorize(Roles = SysRole.EHomePageModularsManageDelete)]
        public ActionResult Delete(int id)
        {
            return Json("1");
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