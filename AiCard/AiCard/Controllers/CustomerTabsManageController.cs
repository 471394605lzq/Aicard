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
    [Authorize]
    public class CustomerTabsManageController : Controller
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
        public void Sidebar(string name = "客户标签")
        {
            ViewBag.Sidebar = name;
        }
        // GET: CustomerTabsManage
        [Authorize(Roles = SysRole.CustomerTabManageRead + "," + SysRole.ECustomerTabManageRead)]
        public ActionResult Index(string filter, int? page = 1)
        {
            Sidebar();
            var query = from ct in db.CustomerTabs
                        from ctg in db.CustomerTabGroups
                        where ct.GroupID == ctg.ID
                        select new CustomerTabViewModel
                        {
                            ID = ct.ID,
                            Name = ct.Name,
                            Sort = ct.Sort,
                            GroupName = ctg.Name,
                            EnterpriseID = ctg.EnterpriseID
                        };
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(s => s.Name.Contains(filter));
            }
            //如果是企业用户则只查询该企业信息
            if (AccontData.UserType == Enums.UserType.Enterprise)
            {
                query = query.Where(s => s.EnterpriseID == AccontData.EnterpriseID);
            }
            var paged = query.OrderByDescending(s => s.ID).ToPagedList(page.Value, 10); ;
            return View(paged);
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
        [Authorize(Roles = SysRole.CustomerTabManageCreate + "," + SysRole.ECustomerTabManageCreate)]
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
        [Authorize(Roles = SysRole.CustomerTabManageCreate + "," + SysRole.ECustomerTabManageCreate)]
        public ActionResult Create(CustomerTab customerTab)
        {
            var tempuser = db.Users.FirstOrDefault(s => s.Id == AccontData.UserID);
            //防止企业用户串号修改
            if ((AccontData.UserType == Enums.UserType.Enterprise
                && tempuser.EnterpriseID != AccontData.EnterpriseID) || AccontData.EnterpriseID <= 0)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
            Sidebar();
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
        [Authorize(Roles = SysRole.CustomerTabManageEdit + "," + SysRole.ECustomerTabManageEdit)]
        public ActionResult Edit(int? id)
        {
            var tempuser = db.Users.FirstOrDefault(s => s.Id == AccontData.UserID);
            //防止企业用户串号修改
            if ((AccontData.UserType == Enums.UserType.Enterprise
                && tempuser.EnterpriseID != AccontData.EnterpriseID) || AccontData.EnterpriseID <= 0)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
            Sidebar();
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
        [Authorize(Roles = SysRole.CustomerTabManageEdit + "," + SysRole.ECustomerTabManageEdit)]
        public ActionResult Edit(CustomerTab customerTab)
        {
            var tempuser = db.Users.FirstOrDefault(s => s.Id == AccontData.UserID);
            //防止企业用户串号修改
            if ((AccontData.UserType == Enums.UserType.Enterprise
                && tempuser.EnterpriseID != AccontData.EnterpriseID) || AccontData.EnterpriseID <= 0)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
            Sidebar();
            if (ModelState.IsValid)
            {
                db.Entry(customerTab).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var temp = db.CustomerTabs.FirstOrDefault(s => s.ID == customerTab.ID);
            if (ModelState.IsValid)
            {
                temp.Name = customerTab.Name;
                temp.Sort = customerTab.Sort;
                temp.GroupID = customerTab.GroupID;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GroupID = new SelectList(db.CustomerTabGroups, "ID", "Name", customerTab.GroupID);
            return View(customerTab);
        }

        // GET: CustomerTabsManage/Delete/5
        [Authorize(Roles = SysRole.CustomerTabMangeDelete + "," + SysRole.ECustomerTabMangeDelete)]
        public ActionResult Delete(int? id)
        {
            var tempuser = db.Users.FirstOrDefault(s => s.Id == AccontData.UserID);
            //防止企业用户串号修改
            if ((AccontData.UserType == Enums.UserType.Enterprise
                && tempuser.EnterpriseID != AccontData.EnterpriseID) || AccontData.EnterpriseID <= 0)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
            Sidebar();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var query = (from ct in db.CustomerTabs
                        from ctg in db.CustomerTabGroups
                        where ct.GroupID == ctg.ID
                        select new CustomerTabViewModel
                        {
                            ID = ct.ID,
                            Name = ct.Name,
                            Sort = ct.Sort,
                            GroupName = ctg.Name,
                            EnterpriseID = ctg.EnterpriseID
                        }).FirstOrDefault();
            //CustomerTab customerTab = db.CustomerTabs.Find(id);
            if (query == null)
            {
                return HttpNotFound();
            }
            return View(query);
        }

        // POST: CustomerTabsManage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SysRole.CustomerTabMangeDelete + "," + SysRole.ECustomerTabMangeDelete)]
        public ActionResult DeleteConfirmed(int id)
        {
            var tempuser = db.Users.FirstOrDefault(s => s.Id == AccontData.UserID);
            //防止企业用户串号修改
            if ((AccontData.UserType == Enums.UserType.Enterprise
                && tempuser.EnterpriseID != AccontData.EnterpriseID) || AccontData.EnterpriseID <= 0)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
            Sidebar();
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
