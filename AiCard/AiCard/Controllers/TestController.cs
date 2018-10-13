﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AiCard.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowCrossSiteJson]
        public ActionResult MiniTest()
        {
            List<MiniModel> list = new List<MiniModel>();

            var model1 = new MiniModel
            {
                ID = "Info",
                Style = Enums.CellStyle.RichText,
                Title = "公司简介",
                Data = "<p>联系方式</p>" + $"<img src='{Url.ResizeImage("~/Upload/1.png")}'/>"
            };
            list.Add(model1);
            var model2 = new MiniModel
            {
                ID = "Phone",
                Style = Enums.CellStyle.RichText,
                Title = "联系方式",
                Data = "<p>22222222</p>"
            };
            list.Add(model2);
            List<ImageListViewModel> alist = new List<ImageListViewModel>();
            alist.Add(new ImageListViewModel { Date = DateTime.Now.ToString("yyyy-MM-dd"), ID = 1, Title = "新闻1", Image = Url.ResizeImage("~/Upload/1.png") });
            alist.Add(new ImageListViewModel { Date = DateTime.Now.ToString("yyyy-MM-dd"), ID = 2, Title = "新闻2", Image = Url.ResizeImage("~/Upload/2.png") });
            var model3 = new MiniModel
            {
                ID = "ArticleList",
                Data = alist,
                Style = Enums.CellStyle.CellList,
                Title = "企业资讯",
            };


            list.Add(model3);

            return Json(Comm.ToJsonResult("Success", "成功", list), JsonRequestBehavior.AllowGet);
        }


    }

    public class MiniModel
    {
        public string ID { get; set; }

        public string Title { get; set; }

        public Enums.CellStyle Style { get; set; }

        public object Data { get; set; }
    }

    public class ImageListViewModel
    {
        public int ID { get; set; }

        public string Image { get; set; }

        public string Title { get; set; }

        public string Date { get; set; }

    }


}