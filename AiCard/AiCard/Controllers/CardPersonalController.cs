using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.DAL.Models;
namespace AiCard.Controllers
{
    public class CardPersonalController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: VipHome
        public ActionResult HomeList(int page = 1, int pageSize = 20)
        {
            var queryCards = (from c in db.CardPersonals
                              from v in db.Vips
                              where c.Enable && c.ID == v.CardID
                              select new
                              {
                                  c.ID,
                                  c.Avatar,
                                  c.Birthday,
                                  c.Gender,
                                  c.Position,
                                  VipType = v.Type,
                                  c.Industry,
                                  c.UserID,
                                  c.City,
                              });

            var paged = queryCards
                .OrderByDescending(s => s.ID)
                .ToPagedList(page, pageSize);
            Func<DateTime?, int?> getAge = dateOfBirth =>
            {
                if (!dateOfBirth.HasValue)
                {
                    return null;
                }
                int age = 0;
                age = DateTime.Now.Year - dateOfBirth.Value.Year;
                if (DateTime.Now.DayOfYear < dateOfBirth.Value.DayOfYear)
                    age = age - 1;
                return age;
            };

            var model = paged.Select(s => new
            {
                s.ID,
                s.UserID,
                s.Avatar,
                s.Gender,
                Age = getAge(s.Birthday),
                s.Position,
                s.Industry,
                s.City,
                s.VipType
            });

            return Json(Common.Comm.ToJsonResultForPagedList(paged, model));
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