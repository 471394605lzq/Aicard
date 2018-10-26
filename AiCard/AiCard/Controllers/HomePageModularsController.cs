using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
namespace AiCard.Controllers
{
    public class HomePageModularsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: HomePageModulars
        public ActionResult Index(string userID, int cardID)
        {
            if (!db.Users.Any(s => s.Id == userID))
            {
                return Json(Comm.ToJsonResult("UserNoFound", "用户不存在"));
            }
            var card = db.Cards.FirstOrDefault(s => s.ID == cardID && s.Enable && s.EnterpriseID.HasValue);
            if (card == null)
            {
                return Json(Comm.ToJsonResult("CardNoFound", "卡片不存在或卡片不是企业"));
            }
            db.HomePageModulars.Where(s => s.EnterpriseID == card.EnterpriseID).OrderBy(s => s.Sort);
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