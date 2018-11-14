using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
using AiCard.Common.Enums;
using AiCard.DAL.Models;

namespace AiCard.Controllers
{
    [Authorize]
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
        [Authorize(Roles = SysRole.CustomerTabGroupsManageRead + "," + SysRole.ECustomerTabGroupsManageRead)]
        public ActionResult Index(string filter,int? page=1)
        {
            Sidebar();
            var query = from ct in db.CustomerTabGroups
                        from e in db.Enterprises
                        where ct.EnterpriseID == e.ID
                        select new CustomerTabGroupViewModel
                        {
                            EnterpriseID = ct.EnterpriseID,
                            ID = ct.ID,
                            EnterpriseName = e.Name,
                            Name = ct.Name,
                            Sort = ct.Sort,
                            Style = ct.Style
                        };
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(s => s.Name.Contains(filter));
            }
            //如果是企业用户则只查询该企业信息
            if (AccontData.UserType == UserType.Enterprise)
            {
                query = query.Where(s => s.EnterpriseID == AccontData.EnterpriseID);
            }
            var paged = query.OrderByDescending(s => s.ID).ToPagedList(page.Value, 10);
            return View(paged);
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
        [Authorize(Roles = SysRole.CustomerTabGroupsManageCreate + "," + SysRole.ECustomerTabGroupsManageCreate)]
        public ActionResult Create()
        {
            Sidebar();
            return View();
        }

        // POST: CustomerTabGroupsManage/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SysRole.CustomerTabGroupsManageCreate + "," + SysRole.ECustomerTabGroupsManageCreate)]
        public ActionResult Create(CustomerTabGroup customerTabGroup)
        {
            var tempuser = db.Users.FirstOrDefault(s => s.Id == AccontData.UserID);
            //防止企业用户串号修改
            if ((AccontData.UserType == UserType.Enterprise
                && tempuser.EnterpriseID != AccontData.EnterpriseID)||AccontData.EnterpriseID<=0)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
            Sidebar();
            if (ModelState.IsValid)
            {
                customerTabGroup.EnterpriseID = AccontData.EnterpriseID;
                db.CustomerTabGroups.Add(customerTabGroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customerTabGroup);
        }

        // GET: CustomerTabGroupsManage/Edit/5
        [Authorize(Roles = SysRole.CustomerTabGroupsManageEdit + "," + SysRole.ECustomerTabGroupsManageEdit)]
        public ActionResult Edit(int? id)
        {
            var tempuser = db.Users.FirstOrDefault(s => s.Id == AccontData.UserID);
            //防止企业用户串号修改
            if ((AccontData.UserType == UserType.Enterprise
                && tempuser.EnterpriseID != AccontData.EnterpriseID) || AccontData.EnterpriseID <= 0)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
            Sidebar();
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
        [Authorize(Roles = SysRole.CustomerTabGroupsManageEdit + "," + SysRole.ECustomerTabGroupsManageEdit)]
        public ActionResult Edit(CustomerTabGroup customerTabGroup)
        {
            var tempuser = db.Users.FirstOrDefault(s => s.Id == AccontData.UserID);
            //防止企业用户串号修改
            if ((AccontData.UserType == UserType.Enterprise
                && tempuser.EnterpriseID != AccontData.EnterpriseID) || AccontData.EnterpriseID <= 0)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
            Sidebar();
            var temp = db.CustomerTabGroups.FirstOrDefault(s=>s.ID== customerTabGroup.ID);
            if (ModelState.IsValid)
            {
                temp.Name = customerTabGroup.Name;
                temp.Sort = customerTabGroup.Sort;
                temp.Style = customerTabGroup.Style;
                temp.EnterpriseID = AccontData.EnterpriseID;
                //db.Entry(customerTabGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customerTabGroup);
        }

        // GET: CustomerTabGroupsManage/Delete/5
        [Authorize(Roles = SysRole.CustomerTabGroupsMangeDelete + "," + SysRole.ECustomerTabGroupsMangeDelete)]
        public ActionResult Delete(int? id)
        {
            var tempuser = db.Users.FirstOrDefault(s => s.Id == AccontData.UserID);
            //防止企业用户串号修改
            if ((AccontData.UserType == UserType.Enterprise
                && tempuser.EnterpriseID != AccontData.EnterpriseID) || AccontData.EnterpriseID <= 0)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
            Sidebar();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var query =(from ct in db.CustomerTabGroups
                        from e in db.Enterprises
                        where ct.EnterpriseID == e.ID&& ct.ID==id
                        select new CustomerTabGroupViewModel
                        {
                            EnterpriseID = ct.EnterpriseID,
                            ID = ct.ID,
                            EnterpriseName = e.Name,
                            Name = ct.Name,
                            Sort = ct.Sort,
                            Style = ct.Style
                        }).FirstOrDefault();
            if (query == null)
            {
                return HttpNotFound();
            }
            return View(query);
        }

        // POST: CustomerTabGroupsManage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SysRole.CustomerTabGroupsMangeDelete + "," + SysRole.ECustomerTabGroupsMangeDelete)]
        public ActionResult DeleteConfirmed(int id)
        {
            var tempuser = db.Users.FirstOrDefault(s => s.Id == AccontData.UserID);
            //防止企业用户串号修改
            if ((AccontData.UserType == UserType.Enterprise
                && tempuser.EnterpriseID != AccontData.EnterpriseID) || AccontData.EnterpriseID <= 0)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
            Sidebar();
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
