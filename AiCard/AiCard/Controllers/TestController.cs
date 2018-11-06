using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Drawing;
using System.Net;
using AiCard.Models.CommModels;

namespace AiCard.Controllers
{
    public class TestController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Test
        public ActionResult Index(int eid)
        {
            var max = db.Cards
                .Where(s => s.EnterpriseID == eid)
                .Max(s => (int?)s.Sort) ?? 0;
            var sort = max / 10 * 10 + 10;
            return Json(sort, JsonRequestBehavior.AllowGet);
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

        public ActionResult UploadTest()
        {
            var model = new TestImages();
            return View(model);
        }

        [HttpPost]
        public ActionResult UploadTest(TestImages model)
        {
            return View(model);
        }

        public ActionResult PcasTest()
        {
            return View();
        }

        [AllowCrossSiteJson]
        public ActionResult DeleteUser()
        {
            var user = db.Users.FirstOrDefault(s => s.NickName == "hot pink");

            var log = db.UserLogs.Where(s => s.UserID == user.Id).ToList();
            db.UserLogs.RemoveRange(log);
            db.Users.Remove(user);
            db.SaveChanges();
            return Json(Comm.ToJsonResult("Success", "消息"), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult TestMap()
        {
            Models.Enterprise e = new Enterprise
            {
                Address = "帝推科技",
                Lat = 23.015228,
                Lng = 113.74574
            };
            var map = new Models.CommModels.Map(e);
            return View(map);
        }

        [HttpPost]
        public ActionResult TestMap(Models.CommModels.Map map)
        {
            return View(map);
        }

        public ActionResult TestWeixinWork(int? dep = null)
        {
            string corpid = "wwfbd3847b25072e2b";
            string secret = "x7H_NcJJ0-mxfwH42SSHGqxS9TdGkRvNqtZbPH5-xb8";
            var api = new WeChatWork.WeChatWorkApi();
            try
            {
                api.GetAccessToken(corpid, secret);
                var deps = api.GetDepartment();
                ViewBag.Deps = deps;
                if (!dep.HasValue)
                {
                    dep = deps.FirstOrDefault().ID;
                }
                var model = api.GetUsesByDepID(dep.Value);
                return View(model);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        //public ActionResult DrawingPicture(string id)
        //{


        //    FileStream fs2 = new FileStream(System.Web.HttpContext.Current.Request.MapPath("~\\Content\\Images\\qrcode.png"), FileMode.Open, FileAccess.Read);
        //    Image image2 = Image.FromStream(fs2);
        //    fs2.Close();
        //    System.Drawing.Image img2 = DrawingPictures.ResizeImage(image2, new Size(240, 240));
        //    string qrcodePath = System.Web.HttpContext.Current.Server.MapPath("~\\Content\\Images\\temofile\\") + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + "qrcode.png";
        //    img2.Save(qrcodePath);
        //    img2.Dispose();

        //    FileStream logofs = new FileStream(System.Web.HttpContext.Current.Request.MapPath("~\\Content\\Images\\logo.png"), FileMode.Open, FileAccess.Read);
        //    Image logoimage = Image.FromStream(logofs);
        //    logofs.Close();
        //    System.Drawing.Image logoimages = DrawingPictures.ResizeImage(logoimage, new Size(96, 96));
        //    string logoPath = System.Web.HttpContext.Current.Server.MapPath("~\\Content\\Images\\temofile\\") + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + "logo.png";
        //    logoimages.Save(logoPath);
        //    logoimages.Dispose();

        //    string avatarPath = DrawingPictures.DownloadImg("http://image.dtoao.com/201810220954316919.jpg", "avatar.png", 834, 834);
        //    DrawingPictureModel m = new DrawingPictureModel();
        //    m.AvatarPath = avatarPath;
        //    m.CompanyName = "广东帝推网络股份科技有限公司啊啊啊啊啊";
        //    m.LogoPath = logoPath;
        //    m.Position = "CEO";
        //    m.QrPath = qrcodePath;
        //    m.Remark = "既然选择了远方，便只顾风雨兼程";
        //    m.UserName = "吴江";
        //    m.PosterImageName = "cardid_" + id;
        //    List<TagModel> listst = new List<TagModel>();
        //    TagModel tm = new TagModel();
        //    tm.TagName = "这就是神器啊 155885888";
        //    tm.TagName = "牛逼的人物啊 155885888";
        //    tm.TagStyle = "橙色";
        //    tm.TagStyle = "绿色";
        //    listst.Add(tm);

        //    string returnpath = Comm.MergePosterImage(m);
        //    return View();
        //}
        public ActionResult TestQiniu()
        {
            return View();

        }

        public ActionResult TestCkEdit()
        {

            return View();
        }
        public ActionResult pac()
        {
            var model = new Test();
            return View(model);
        }

        [AllowCrossSiteJson]
        public ActionResult WirteLog(object data)
        {
            Comm.WriteLog("Debug", JsonConvert.SerializeObject(data), Enums.DebugLogLevel.Normal);
            return Json(Comm.ToJsonResult("Success", "成功"));
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

        public class TestImages
        {
            public TestImages()
            {
                Avatar = new AiCard.Models.CommModels.FileUpload
                {
                    Max = 3,
                    Name = "Avatar",
                    Sortable = true,
                    Type = AiCard.Models.CommModels.FileType.Sound,
                    AutoInit = false,
                    Server = AiCard.Models.CommModels.UploadServer.QinQiu
                };
            }

            public string NickName { get; set; }

            public AiCard.Models.CommModels.FileUpload Avatar { get; set; }

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