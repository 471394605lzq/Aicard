using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.DAL.Models;
namespace AiCard.Controllers
{
    public class VipAmountController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: VipAmountLog
        public ActionResult Top(int top)
        {
            var query = from v in db.Vips
                        from c in db.CardPersonals
                        where c.ID == v.CardID
                        select new
                        {
                            v.TotalAmount,
                            c.Name,

                        };
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