using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
using AiCard.Common.Enums;
using AiCard.DAL.Models;
using AiCard.Common;

namespace AiCard.Controllers
{
    public class HomePageModularsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        [AllowCrossSiteJson]
        public ActionResult Index(string userID, int cardID)
        {
            if (!db.Users.Any(s => s.Id == userID))
            {
                return Json(Comm.ToJsonResult("UserNoFound", "用户不存在"), JsonRequestBehavior.AllowGet);
            }
            var card = db.Cards.FirstOrDefault(s => s.ID == cardID && s.Enable && s.EnterpriseID.HasValue);
            if (card == null)
            {
                return Json(Comm.ToJsonResult("CardNoFound", "卡片不存在或卡片不是企业"), JsonRequestBehavior.AllowGet);
            }
            var ent = db.Enterprises.FirstOrDefault(s => s.ID == card.EnterpriseID);
            var models = db.HomePageModulars
                .Where(s => s.EnterpriseID == card.EnterpriseID)
                .OrderBy(s => s.Sort)
                .ToList();
            var modulars = models.Select(s =>
            {
                object item;
                switch (s.Type)
                {
                    default:
                    case HomePageModularType.Html:
                        item = new
                        {
                            Title = s.Title,
                            Content = s.Content,
                            Type = s.Type,
                        };
                        break;
                    case HomePageModularType.Banner:
                    case HomePageModularType.Images:
                        item = new
                        {
                            Title = s.Title,
                            Content = s.Content.SplitToArray<string>(),
                            Type = s.Type,
                        };
                        break;
                    case HomePageModularType.Contact:
                        item = new
                        {
                            Title = s.Title,
                            Content = new
                            {
                                Phone = ent.PhoneNumber,
                                Email = ent.Email,
                                Address = new
                                {
                                    Name = $"{ent.Province}{ent.City}{ent.District}{ent.Address}",
                                    Lat = ent.Lat,
                                    Lng = ent.Lng
                                }
                            },
                            Type = s.Type
                        };
                        break;
                }
                return item;
            });
            return Json(Comm.ToJsonResult("Success", "成功", modulars), JsonRequestBehavior.AllowGet);
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