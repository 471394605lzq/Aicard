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
    public class CustomerTabGroupsManageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //获取当前登录的用户信息
        AccountData AccontData
        {
            get
            {
                return this.GetAccountData();
            }
        }
        public void Sidebar(string name = "客户标签分组")
        {
            ViewBag.Sidebar = name;

        }

        // GET: CustomerTabGroupsManage
        public ActionResult Index()
        {
            return View(db.CustomerTabGroups.ToList());
        }

        // GET: CustomerTabGroupsManage/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerTabGroup customerTabGroup = db.CustomerTabGroups.Find(id);
            if (customerTabGroup == null)
            {
                return HttpNotFound();
            }
            return View(customerTabGroup);
        }

        // GET: CustomerTabGroupsManage/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CustomerTabGroupsManage/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CustomerTabGroup customerTabGroup)
        {
            if (ModelState.IsValid)
            {
                db.CustomerTabGroups.Add(customerTabGroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(customerTabGroup);
        }

        // GET: CustomerTabGroupsManage/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerTabGroup customerTabGroup = db.CustomerTabGroups.Find(id);
            if (customerTabGroup == null)
            {
                return HttpNotFound();
            }
            return View(customerTabGroup);
        }

        // POST: CustomerTabGroupsManage/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,EnterpriseID,Name,Style,Sort")] CustomerTabGroup customerTabGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customerTabGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customerTabGroup);
        }

        // GET: CustomerTabGroupsManage/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerTabGroup customerTabGroup = db.CustomerTabGroups.Find(id);
            if (customerTabGroup == null)
            {
                return HttpNotFound();
            }
            return View(customerTabGroup);
        }

        // POST: CustomerTabGroupsManage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CustomerTabGroup customerTabGroup = db.CustomerTabGroups.Find(id);
            db.CustomerTabGroups.Remove(customerTabGroup);
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
