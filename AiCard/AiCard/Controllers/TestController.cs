using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace AiCard.Controllers
{
    public class TestController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Test
        public ActionResult Index()
        {
            //var _roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(db));
            //var _userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            //var roles = _roleManager.Roles
            //    .Where(s => s.Type == Enums.RoleType.Enterprise)
            //    .Select(s => s.Name).ToArray();
            //var admins = from u in db.Users
            //             from e in db.Enterprises
            //             where u.Id == e.AdminID
            //             select u.Id;
            //foreach (var item in admins)
            //{
            //    _userManager.AddToRoles(item, roles);
            //}

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

        public ActionResult UploadTest()
        {
            var s = ChinaPCAS.GetP();
            var a = ChinaPCAS.GetC("广东省");
            var c = ChinaPCAS.GetA("广东省", "东莞市");
            var model = new TestImages();
            return View(model);
        }

        public ActionResult PcasTest()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadTest(TestImages model)
        {
            return View(model);
        }

        [HttpGet]
        public ActionResult TestMap()
        {
            Models.Enterprise e = new Enterprise
            {
                Address = "帝推科技",
                //Lat = 23.015228,
                //Lng = 113.74574
            };
            var map = new Models.CommModels.Map(e);
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

        public ActionResult TestQiniu()
        {
            return View();

        }

        public ActionResult TestCkEdit()
        {

            return View();
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
                    Type = AiCard.Models.CommModels.FileType.Image,
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