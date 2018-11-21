using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using AiCard.Common.CommModels;
using AiCard.Common.Enums;
using AiCard.DAL.Models;
using AiCard.Common;

namespace AiCard.Controllers
{
    public class TestController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Test
        public ActionResult Index(string userName)
        {
            var vips = db.Vips.ToList();
            int amount = 86;
            foreach (var item in vips)
            {
                var freeChild = Common.Comm.Random.Next(1, 100);
                var randomChild2 = Common.Comm.Random.Next(1, freeChild);
                var randomChild3 = Common.Comm.Random.Next(1, 100);

                item.TotalAmount = freeChild * 3m + randomChild2 * amount * 0.5m + randomChild3 * amount * 0.1m;
                var take = 50 * Common.Comm.Random.Next(0, (int)(item.TotalAmount % 50));
                item.Amount = item.TotalAmount - take;
                item.Code = Common.Comm.Random.Next(100000, 999999).ToString();
                item.VipChild2ndCount = randomChild2;
                item.VipChild3rdCount = randomChild3;
                item.FreeChildCount = freeChild;
                item.Type = VipRank.Vip99;
                
            }
            db.SaveChanges();
            return Json(vips, JsonRequestBehavior.AllowGet);

        }
        #region 腾讯IM

        /// <summary>
        /// 1对1聊天
        /// </summary>
        /// <param name="from">发送人昵称</param>
        /// <param name="to">接收人昵称</param>
        /// <returns></returns>
        public ActionResult TestTxImChat(string from, string to)
        {
            var userNicks = new string[] { from, to };
            var users = db.Users.Where(s => userNicks.Contains(s.NickName))
                .Select(s => new Models.TxImViewModels.User
                {
                    ID = s.Id,
                    Avatar = s.Avatar,
                    NickName = s.NickName,
                    UserName = s.UserName
                }).ToList();
            var fromUser = users.FirstOrDefault(s => s.NickName == from);
            fromUser.SignUser = Common.TxIm.SigCheck.Sign(fromUser.UserName);
            ViewBag.From = users.FirstOrDefault(s => s.NickName == from);
            ViewBag.To = users.FirstOrDefault(s => s.NickName == to);
            return View();
        }

        public ActionResult ImportUser(string userID)
        {
            var user = db.Users.FirstOrDefault(s => s.Id == userID);
            new Common.TxIm.ImApi().ImportUser(user.UserName, user.NickName, user.Avatar);
            return Json(1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TestTxImList(string from)
        {
            var fromUser = db.Users.Select(s => new Models.TxImViewModels.User
            {
                ID = s.Id,
                Avatar = s.Avatar,
                NickName = s.NickName,
                UserName = s.UserName
            }).FirstOrDefault(s => s.NickName == from);
            fromUser.SignUser = Common.TxIm.SigCheck.Sign(fromUser.UserName);
            ViewBag.From = fromUser;
            return View();
        }

        #endregion

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

        //[AllowCrossSiteJson]
        //public ActionResult DeleteUser()
        //{
        //    var user = db.Users.FirstOrDefault(s => s.NickName == "hot pink");

        //    var log = db.UserLogs.Where(s => s.UserID == user.Id).ToList();
        //    db.UserLogs.RemoveRange(log);
        //    db.Users.Remove(user);
        //    db.SaveChanges();
        //    return Json(Comm.ToJsonResult("Success", "消息"), JsonRequestBehavior.AllowGet);
        //}


        [HttpGet]
        public ActionResult TestMap()
        {
            Enterprise e = new Enterprise
            {
                Address = "帝推科技",
                Lat = 23.015228,
                Lng = 113.74574
            };
            var map = new Map(e);
            return View(map);
        }

        [HttpPost]
        public ActionResult TestMap(Map map)
        {
            return View(map);
        }

        public ActionResult TestWeixinWork(int? dep = null)
        {
            string corpid = "wwfbd3847b25072e2b";
            string secret = "x7H_NcJJ0-mxfwH42SSHGqxS9TdGkRvNqtZbPH5-xb8";
            var api = new Common.WeChatWork.WeChatWorkApi();
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


        [AllowCrossSiteJson]
        public ActionResult WirteLog(object data)
        {
            Comm.WriteLog("Debug", JsonConvert.SerializeObject(data), DebugLogLevel.Normal);
            return Json(Comm.ToJsonResult("Success", "成功"));
        }


        public class MiniModel
        {
            public string ID { get; set; }

            public string Title { get; set; }

            public CellStyle Style { get; set; }

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
                Avatar = new FileUpload
                {
                    Max = 3,
                    Name = "Avatar",
                    Sortable = true,
                    Type = Common.CommModels.FileType.Sound,
                    AutoInit = false,
                    Server = UploadServer.QinQiu
                };
            }

            public string NickName { get; set; }

            public FileUpload Avatar { get; set; }

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