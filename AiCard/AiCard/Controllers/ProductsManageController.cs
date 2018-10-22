﻿using System;
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
    public class ProductsManageController : Controller
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
        public void Sidebar(string name = "商品管理")
        {
            ViewBag.Sidebar = name;

        }

        // GET: ProductsManage
        public ActionResult Index(string filter, int page = 1)
        {
            Sidebar();
            var m = from p in db.Products
                    from e in db.Enterprises
                    from ps in db.ProductKinds
                    where p.EnterpriseID == e.ID && p.KindID == ps.ID
                    select new ProductIndexViewModels
                    {
                        ID = p.ID,
                        EnterpriseID = p.EnterpriseID,
                        EnterpriseName = e.Name,
                        Name = p.Name,
                        Sort = p.Sort,
                        Count = p.Count,
                        Info = p.Info,
                        KindID = p.KindID,
                        Price = p.Price,
                        Release = p.Release,
                        TotalSales = p.TotalSales,
                        Type = p.Type,
                        Kind = p.Kind,
                        Images=p.Images
                    };
            //var m = db.Products.Include(ps => ps.Kind);
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

        // GET: ProductsManage/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: ProductsManage/Create
        public ActionResult Create()
        {
            ViewBag.KindID = new SelectList(db.ProductKinds, "ID", "Name");
            var model = new ProductViewModels();
            return View(model);
        }

        // POST: ProductsManage/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductViewModels product)
        {
            if (ModelState.IsValid)
            {
                var model = new Product
                {
                    Name = product.Name,
                    Count = product.Count,
                    EnterpriseID = AccontData.EnterpriseID,
                    Images = string.Join(",", product.Images.Images),
                    Info = product.Info,
                    KindID = product.KindID,
                    Price = product.Price,
                    Release = product.Release,
                    Sort = product.Sort,
                    TotalSales = product.TotalSales,
                    Type = product.Type
                };
                db.Products.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.KindID = new SelectList(db.ProductKinds, "ID", "Name", product.KindID);
            return View(product);
        }

        // GET: ProductsManage/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Product product = db.Products.Find(id);
            var model = (from p in db.Products
                         from ps in db.ProductKinds
                         where p.KindID == ps.ID && p.ID == id.Value
                         select p).FirstOrDefault();
            var models = new ProductViewModels
            {
                ID = model.ID,
                Name = model.Name,
                Count = model.Count,
                EnterpriseID = model.EnterpriseID,
                Info = model.Info,
                KindID = model.KindID,
                Price = model.Price,
                Release = model.Release,
                Sort = model.Sort,
                TotalSales = model.TotalSales,
                Type = model.Type
            };
            models.Images.Images = model.Images?.Split(',') ?? new string[0];
            if (models == null)
            {
                return HttpNotFound();
            }
            ViewBag.KindID = new SelectList(db.ProductKinds, "ID", "Name", models.KindID);
            return View(models);
        }

        // POST: ProductsManage/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductViewModels product)
        {
            if (ModelState.IsValid)
            {
                var t = db.Products.FirstOrDefault(s => s.ID == product.ID);
                t.ID = product.ID;
                t.Name = product.Name;
                t.Count = product.Count;
                t.EnterpriseID = product.EnterpriseID;
                t.Info = product.Info;
                t.KindID = product.KindID;
                t.Price = product.Price;
                t.Release = product.Release;
                t.Sort = product.Sort;
                t.TotalSales = product.TotalSales;
                t.Type = product.Type;
                t.Images = string.Join(",", product.Images.Images);
                db.SaveChanges();

                //db.Entry(product).State = EntityState.Modified;
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.KindID = new SelectList(db.ProductKinds, "ID", "Name", product.KindID);
            return View(product);
        }

        // GET: ProductsManage/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: ProductsManage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
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
