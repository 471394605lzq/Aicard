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

        // GET: HomePageManage
        public ActionResult Index()
        {
            return View();
        }

        public void SetModularList()
        {

        }

        public ActionResult Create()
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