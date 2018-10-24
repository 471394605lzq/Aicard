using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
namespace AiCard.Controllers
{
    public class CardTabController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: CardTab
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult Create(int cardID, string name, string userID)
        {
            name = name.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                return Json(Comm.ToJsonResult("NameIsNull", "标签名不能为空"));
            }
            if (!db.Cards.Any(s => s.ID == cardID && s.UserID == userID))
            {
                return Json(Comm.ToJsonResult("CardNoFound", "卡片不存在"));
            }
            if (db.Cards.Any(s => s.ID == cardID && s.Name == name))
            {
                return Json(Comm.ToJsonResult("HadAdd", $"{name}已经存在"));
            }
            var randam = Comm.Random.Next(3);
            var tab = new CardTab { CardID = cardID, Name = name, Style = (Enums.CardTabStyle)randam };
            db.CardTabs.Add(tab);
            db.SaveChanges();
            return Json(Comm.ToJsonResult("Success", "添加成功"));
        }

        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult Delete(int cardID, int tabID, string userID)
        {
            if (!db.Cards.Any(s => s.ID == cardID && s.UserID == userID))
            {
                return Json(Comm.ToJsonResult("CardNoFound", "卡片不存在"));
            }
            var tab = db.CardTabs.FirstOrDefault(s => s.CardID == cardID && s.ID == tabID);
            if (tab == null)
            {
                return Json(Comm.ToJsonResult("TabNoFound", "标签不存在"));
            }

            db.CardTabs.Remove(tab);
            db.SaveChanges();
            return Json(Comm.ToJsonResult("Success", "删除成功"));
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