using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
using AiCard.Enums;

namespace AiCard.Controllers
{
    [Authorize]
    public class ProductKindsManageController : Controller
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
        public void Sidebar(string name = "商品分类管理")
        {
            ViewBag.Sidebar = name;

        }
        // GET: ProductKinds
        [Authorize(Roles = SysRole.ProductKindManageRead + "," + SysRole.EProductKindManageRead)]
        public ActionResult Index(string filter, int page = 1)
        {
            Sidebar();
            var m = from ps in db.ProductKinds
                    from e in db.Enterprises
                    where ps.EnterpriseID==e.ID
                    select new ProductKindsViewModels
                    {
                        ID = ps.ID,
                        EnterpriseID = ps.EnterpriseID,
                        EnterpriseName = e.Name,
                        Name = ps.Name,
                        Sort = ps.Sort
                    };
            if (!string.IsNullOrWhiteSpace(filter))
            {
                m = m.Where(s => s.Name.Contains(filter));
            }
            //如果是企业用户则只查询该企业信息
            if (AccontData.UserType == Enums.UserType.Enterprise)
            {
                m = m.Where(s => s.EnterpriseID == AccontData.EnterpriseID);
            }
            var paged = m.OrderByDescending(s => s.ID).ToPagedList(page);
            return View(paged);
        }

        // GET: ProductKinds/Details/5
        public ActionResult Details(int? id)
        {
            Sidebar();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductKind productKind = db.ProductKinds.Find(id);
            if (productKind == null)
            {
                return HttpNotFound();
            }
            return View(productKind);
        }

        // GET: ProductKinds/Create
        [Authorize(Roles = SysRole.ProductKindManageCreate + "," + SysRole.EProductKindManageCreate)]
        public ActionResult Create()
        {
            Sidebar();
            return View();
        }

        // POST: ProductKinds/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SysRole.ProductKindManageCreate + "," + SysRole.EProductKindManageCreate)]
        public ActionResult Create(ProductKind productKind)
        {
            var tempuser = db.Users.FirstOrDefault(s => s.Id == AccontData.UserID);
            //防止企业用户串号修改
            if (AccontData.UserType == Enums.UserType.Enterprise
                && tempuser.EnterpriseID != AccontData.EnterpriseID)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
            Sidebar();
            if (ModelState.IsValid)
            {
                productKind.EnterpriseID = AccontData.EnterpriseID;
                db.ProductKinds.Add(productKind);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productKind);
        }

        // GET: ProductKinds/Edit/5
        [Authorize(Roles = SysRole.ProductKindManageEdit + "," + SysRole.EProductKindManageEdit)]
        public ActionResult Edit(int? id)
        {
            Sidebar();
            var temp = db.ProductKinds.FirstOrDefault(s => s.ID == id.Value);
            //防止企业用户串号修改
            if (AccontData.UserType == Enums.UserType.Enterprise
                && temp.EnterpriseID != AccontData.EnterpriseID)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var m = (from ps in db.ProductKinds
                     from e in db.Enterprises
                     where ps.EnterpriseID == e.ID && ps.ID == id.Value
                     select ps).FirstOrDefault();
            if (m == null)
            {
                return HttpNotFound();
            }
            return View(m);
        }

        // POST: ProductKinds/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SysRole.ProductKindManageEdit + "," + SysRole.EProductKindManageEdit)]
        public ActionResult Edit(ProductKind productKind)
        {
            var temp = db.ProductKinds.FirstOrDefault(s => s.ID == productKind.ID);
            //防止企业用户串号修改
            if (AccontData.UserType == Enums.UserType.Enterprise
                && temp.EnterpriseID != AccontData.EnterpriseID)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
            if (ModelState.IsValid)
            {
                //productKind.EnterpriseID = AccontData.EnterpriseID;
                //db.Entry(productKind).State = EntityState.Modified;
                temp.EnterpriseID = AccontData.EnterpriseID;
                temp.Name = productKind.Name;
                temp.Sort = productKind.Sort;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productKind);
        }

        // GET: ProductKinds/Delete/5
        [Authorize(Roles = SysRole.ProductKindMangeDelete + "," + SysRole.EProductKindMangeDelete)]
        public ActionResult Delete(int? id)
        {
            var temp = db.ProductKinds.FirstOrDefault(s => s.ID == id.Value);
            //防止企业用户串号修改
            if (AccontData.UserType == Enums.UserType.Enterprise
                && temp.EnterpriseID != AccontData.EnterpriseID)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
            Sidebar();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var m = (from ps in db.ProductKinds
                     from e in db.Enterprises
                     where ps.EnterpriseID == e.ID && ps.ID == id.Value
                     select ps).FirstOrDefault();
            if (m == null)
            {
                return HttpNotFound();
            }
            return View(m);
        }

        // POST: ProductKinds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SysRole.ProductKindMangeDelete + "," + SysRole.EProductKindMangeDelete)]
        public ActionResult DeleteConfirmed(int id)
        {
            var temp = db.ProductKinds.FirstOrDefault(s => s.ID == id);
            //防止企业用户串号修改
            if (AccontData.UserType == Enums.UserType.Enterprise
                && temp.EnterpriseID != AccontData.EnterpriseID)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
            ProductKind productKind = db.ProductKinds.Find(id);
            db.ProductKinds.Remove(productKind);
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
