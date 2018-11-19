using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.DAL.Models;
namespace AiCard.Controllers
{
    public class VipHomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: VipHome
        public ActionResult Index(int page = 1, int pageSize = 20)
        {
            var cards = db.Cards.OrderByDescending(s => s.ID).ToPagedList(page, pageSize);
            
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