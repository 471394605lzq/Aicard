using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
using AiCard.Common.Enums;
using AiCard.DAL.Models;

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
            if (db.CardTabs.Any(s => s.CardID == cardID && s.Name == name))
            {
                return Json(Comm.ToJsonResult("HadAdd", $"{name}已经存在"));
            }
            var randam = Comm.Random.Next(3);
            var tab = new CardTab { CardID = cardID, Name = name, Style = (CardTabStyle)randam };
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


        /// <summary>
        /// 获取我的标签
        /// </summary>
        /// <param name="cardID">名片id</param>
        /// <returns>我的标签列表json集合</returns>
        [AllowCrossSiteJson]
        public ActionResult GetMyCardTabsList(int cardID)
        {
            //db.Database.SqlQuery()
            if (!db.Cards.Any(s => s.ID == cardID))
            {
                return Json(Comm.ToJsonResult("CardNoFound", "名片不存在"));
            }
            var query = from ct in db.CardTabs
                        where ct.CardID == cardID
                        select new
                        {
                            ID=ct.ID,
                            Name = ct.Name,
                            Count = ct.Count,
                            Style = ct.Style
                        };
            var data = query.ToList();
            return Json(Comm.ToJsonResult("Success", "获取成功", data), JsonRequestBehavior.AllowGet);
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