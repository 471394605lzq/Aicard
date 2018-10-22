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

        // GET: HomePageManage
        public ActionResult Index()
        {
            Sidebar();
            var eid = EnterpriseID;
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
            return View();
        }

        public void InitModularList()
        {

        }

        public ActionResult Check()
        {
            var eid = EnterpriseID;
            var m = new Bll.HomePageModulars(eid);
            return Json(Comm.ToJsonResult("Success", "", new { HasInit = eid }));
        }

        public ActionResult CreateByHtml()
        {
            var model = new HomePageModularByHtml();
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateByHtml(HomePageModularByHtml model)
        {

            return View();
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