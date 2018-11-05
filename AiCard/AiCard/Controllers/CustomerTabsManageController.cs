using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;

namespace AiCard.Controllers
{
    public class CustomerTabsManageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CustomerTabsManage
        public ActionResult Index()
        {
            var customerTabs = db.CustomerTabs.Include(c => c.Group);
            return View(customerTabs.ToList());
        }

        // GET: CustomerTabsManage/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerTab customerTab = db.CustomerTabs.Find(id);
            if (customerTab == null)
            {
                return HttpNotFound();
            }
            return View(customerTab);
        }

        // GET: CustomerTabsManage/Create
        public ActionResult Create()
        {
            ViewBag.GroupID = new SelectList(db.CustomerTabGroups, "ID", "Name");
            return View();
        }

        // POST: CustomerTabsManage/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,GroupID,Name,Sort")] CustomerTab customerTab)
        {
            if (ModelState.IsValid)
            {
                db.CustomerTabs.Add(customerTab);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GroupID = new SelectList(db.CustomerTabGroups, "ID", "Name", customerTab.GroupID);
            return View(customerTab);
        }

        // GET: CustomerTabsManage/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerTab customerTab = db.CustomerTabs.Find(id);
            if (customerTab == null)
            {
                return HttpNotFound();
            }
            ViewBag.GroupID = new SelectList(db.CustomerTabGroups, "ID", "Name", customerTab.GroupID);
            return View(customerTab);
        }

        // POST: CustomerTabsManage/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,GroupID,Name,Sort")] CustomerTab customerTab)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customerTab).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GroupID = new SelectList(db.CustomerTabGroups, "ID", "Name", customerTab.GroupID);
            return View(customerTab);
        }

        // GET: CustomerTabsManage/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerTab customerTab = db.CustomerTabs.Find(id);
            if (customerTab == null)
            {
                return HttpNotFound();
            }
            return View(customerTab);
        }

        // POST: CustomerTabsManage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CustomerTab customerTab = db.CustomerTabs.Find(id);
            db.CustomerTabs.Remove(customerTab);
            db.SaveChanges();
            return RedirectToAction("Index");
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
