using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
using Newtonsoft.Json;
using AiCard.Common.Enums;
using AiCard.DAL.Models;
using AiCard.Common;
namespace AiCard.Controllers
{
    public class ArticlesManageController : Controller
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
        public void Sidebar(string name = "动态管理")
        {
            ViewBag.Sidebar = name;
        }

        [Authorize(Roles = SysRole.ArticlesManageRead + "," + SysRole.EArticlesManageRead)]
        // GET: ArticlesManage
        public ActionResult Index(string filter, ArticleType type = ArticleType.Html, int? page = 1)
        {
            Sidebar();
            var m = from a in db.Articles
                    from e in db.Enterprises
                    from u in db.Users
                    where a.EnterpriseID == e.ID && a.UserID == u.Id && a.Type == type
                    select new ArticleViewModel
                    {
                        ID = a.ID,
                        Comment = a.Comment,
                        Content = a.Content,
                        EnterpriseID = e.ID,
                        EnterpriseName = e.Name,
                        Images = a.Images,
                        Title = a.Title,
                        UserName = u.UserName,
                        Type = a.Type,
                        State = a.State

                    };
            if (!string.IsNullOrWhiteSpace(filter))
            {
                m = m.Where(s => s.Title.Contains(filter));
            }
            //如果是企业用户则只查询该企业信息
            if (AccontData.UserType == UserType.Enterprise)
            {
                m = m.Where(s => s.EnterpriseID == AccontData.EnterpriseID);
            }
            var paged = m.OrderByDescending(s => s.ID).ToPagedList(page.Value, 15);
            return View(paged);
            //return View(db.Articles.ToList());
        }

        // GET: ArticlesManage/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        [Authorize(Roles = SysRole.ArticlesManageCreate + "," + SysRole.EArticlesManageCreate)]
        // GET: ArticlesManage/Create
        public ActionResult Create()
        {
            Sidebar();
            var model = new ArticleCreateEditViewModel();
            return View(model);
        }

        // POST: ArticlesManage/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [Authorize(Roles = SysRole.ArticlesManageCreate + "," + SysRole.EArticlesManageCreate)]
        public ActionResult Create(ArticleCreateEditViewModel article)
        {
            Sidebar();
            var tempuser = db.Users.FirstOrDefault(s => s.Id == AccontData.UserID);
            //防止企业用户串号修改
            if (AccontData.UserType == UserType.Enterprise
                && tempuser.EnterpriseID != AccontData.EnterpriseID)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
            if (ModelState.IsValid)
            {

                var model = new Article
                {
                    Images = article.Cover.ImageUrl,
                    Like = 0,
                    Share = 0,
                    State = ArticleState.Wait,
                    Comment = 0,
                    Content = article.Content,
                    CreateDateTime = DateTime.Now,
                    EnterpriseID = AccontData.EnterpriseID,
                    Title = article.Title,
                    Type = ArticleType.Html,
                    UpdateDateTime = DateTime.Now,
                    UserID = AccontData.UserID,
                    Video = ""
                };
                db.Articles.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(article);
        }

        [Authorize(Roles = SysRole.ArticlesManageEdit + "," + SysRole.EArticlesManageEdit)]
        // GET: ArticlesManage/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sidebar();
            var temp = db.Articles.FirstOrDefault(s => s.ID == id);
            //防止企业用户串号修改
            if (AccontData.UserType == UserType.Enterprise
                && temp.EnterpriseID != AccontData.EnterpriseID)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
            else if (temp.Type == ArticleType.Text)
            {
                return this.ToError("错误", "用户动态不可编辑", Url.Action("Index"));
            }
            var models = new ArticleCreateEditViewModel
            {
                ID = temp.ID,
                Title = temp.Title,
                Content = temp.Content,
            };
            models.Cover.ImageUrl = temp.Images.SplitToArray<string>(',')?[0];
            if (models == null)
            {
                return HttpNotFound();
            }
            return View(models);
        }

        // POST: ArticlesManage/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [Authorize(Roles = SysRole.ArticlesManageEdit + "," + SysRole.EArticlesManageEdit)]
        public ActionResult Edit(ArticleCreateEditViewModel article)
        {
            var temp = db.Articles.FirstOrDefault(s => s.ID == article.ID);
            //防止企业用户串号修改
            if (AccontData.UserType == UserType.Enterprise
                && temp.EnterpriseID != AccontData.EnterpriseID)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
            if (ModelState.IsValid)
            {
                if (temp.Type == ArticleType.Html)
                {
                    var t = db.Articles.FirstOrDefault(s => s.ID == article.ID);
                    t.ID = article.ID;
                    t.Content = article.Content;
                    t.Title = article.Title;
                    t.Images = article.Cover.ImageUrl;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return this.ToError("错误", "用户动态不可编辑", Url.Action("Index"));
                }
            }
            Sidebar();
            return View(article);
        }

        [Authorize(Roles = SysRole.ArticlesMangeDelete + "," + SysRole.EArticlesMangeDelete)]
        // GET: ArticlesManage/Delete/5
        public ActionResult Delete(int? id)
        {
            var temp = db.Articles.FirstOrDefault(s => s.ID == id);
            //防止企业用户串号修改
            if (AccontData.UserType == UserType.Enterprise
                && temp.EnterpriseID != AccontData.EnterpriseID)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        // POST: ArticlesManage/Delete/5
        [Authorize(Roles = SysRole.ArticlesMangeDelete + "," + SysRole.EArticlesMangeDelete)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var temp = db.Articles.FirstOrDefault(s => s.ID == id);
            //防止企业用户串号修改
            if (AccontData.UserType == UserType.Enterprise
                && temp.EnterpriseID != AccontData.EnterpriseID)
            {
                return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            }
            Article article = db.Articles.Find(id);
            db.Articles.Remove(article);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [AllowCrossSiteJson]
        [ValidateInput(false)]
        [Authorize(Roles = SysRole.ArticlesMangeCheck + "," + SysRole.EArticlesMangeCheck)]
        // GET: ArticlesManage/Edit/5
        public ActionResult Check(string listu)
        {

            var article = JsonConvert.DeserializeObject<List<Article>>(listu);//post过来需要保存数据库的数据
            Sidebar();
            //var temp = db.Articles.FirstOrDefault(s => s.ID == id);
            ////防止企业用户串号修改
            //if (AccontData.UserType == UserType.Enterprise
            //    && temp.EnterpriseID != AccontData.EnterpriseID)
            //{
            //    return this.ToError("错误", "没有该操作权限", Url.Action("Index"));
            //}
            //else if (temp.Type == ArticleType.Text)
            //{
            //    return this.ToError("错误", "用户动态不可编辑", Url.Action("Index"));
            //}

            foreach (var item in article)
            {
                try
                {
                    var t = db.Articles.FirstOrDefault(s => s.ID == item.ID);
                    t.ID = item.ID;
                    t.State = ArticleState.Released;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
                }
            }
            return Json(Comm.ToJsonResult("Success", "同步成功", new { data = article }), JsonRequestBehavior.AllowGet);
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
