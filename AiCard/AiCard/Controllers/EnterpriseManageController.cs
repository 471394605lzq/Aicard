using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
namespace AiCard.Controllers
{
    [Authorize]
    public class EnterpriseManageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult Index(string filter, bool? enable = null, int page = 1)
        {
            var m = from e in db.Enterprises
                    from u in db.Users
                    where e.AdminID == u.Id
                    select new EnterpriseViewModels
                    {
                        ID = e.ID,
                        Enable = e.Enable,
                        Admin = u.UserName,
                        CardCount = e.CardCount,
                        Name = e.Name,
                        PhoneNumber = e.PhoneNumber,
                        AdminID = u.Id
                    };
            if (!string.IsNullOrWhiteSpace(filter))
            {
                m = m.Where(s => s.Name.Contains(filter));
            }
            if (enable != null)
            {
                m = m.Where(s => s.Enable == enable.Value);
            }
            var paged = m.OrderByDescending(s => s.AdminID).ToPagedList(page);
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