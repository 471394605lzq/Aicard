﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
using ToolGood.Words;

namespace AiCard.Controllers
{
    public class ArticleCommentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [AllowCrossSiteJson]
        public ActionResult Index(int articleID, int page = 1, int pageSize = 20)
        {
            var paged = (from c in db.ArticleComments
                         from u in db.Users
                         where c.ArticleID == articleID && c.UserID == u.Id
                         orderby c.CreateDateTime descending
                         select new { u.Id, u.NickName, u.Avatar, c.Content, c.CreateDateTime })
                         .ToPagedList(page, pageSize);
            var data = paged.Select(s => new
            {
                s.Avatar,
                s.Content,
                DateTime = s.CreateDateTime.ToStrForm(),
                UserName = s.NickName
            });
            return Json(Comm.ToJsonResultForPagedList(paged, data),JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult Create(int articleID, string userID, string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(content))
                {
                    return Json(Comm.ToJsonResult("ContentIsNull", "评论不能为空"));
                }
                var wordPath = HttpContext.Server.MapPath("~/App_Start/IllegalWords/keywords.txt");
                var words = System.IO.File.ReadAllText(wordPath).SplitToArray<string>('|');
                StringSearch iwords = new StringSearch();
                iwords.SetKeywords(words);
                content = iwords.Replace(content, '*');//敏感词过滤
                if (string.IsNullOrWhiteSpace(userID) || !db.Users.Any(s => s.Id == userID))
                {
                    return Json(Comm.ToJsonResult("UserNoFound", "用户不存在"));
                }
                if (!db.Articles.Any(s => s.ID == articleID))
                {
                    return Json(Comm.ToJsonResult("ArticleNoFound", "文章不存在"));
                }
                //var limitSec = 60;
                //var limit = DateTime.Now.AddSeconds(limitSec * -1);
                //if (db.ArticleComments.Any(s => s.UserID == userID && s.CreateDateTime > limit))
                //{
                //    return Json(Comm.ToJsonResult("CreateLimit", $"评论在{limitSec}s内只能评论一次"));
                //}
                var comment = new ArticleComment { ArticleID = articleID, UserID = userID, CreateDateTime = DateTime.Now, Content = content };
                db.ArticleComments.Add(comment);
                db.SaveChanges();
                return Json(Comm.ToJsonResult("Success", "成功"));
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message));
            }


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