using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
namespace AiCard.Controllers
{
    public class UserManageController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: UserManage
        public ActionResult Index(string filter, Enums.UserType? userType, int page = 1)
        {
            var model = db.Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                model = model.Where(s => s.UserName == filter);
            }
            if (userType != null)
            {
                model = model.Where(s => s.UserType == userType);
            }
            var paged = model.ToPagedList(page);
            return View(paged);
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