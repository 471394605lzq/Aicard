using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AiCard.Common.CommModels;
using AiCard.Common.Enums;
using AiCard.DAL.Models;
using AiCard.Common;
using AiCard.Commom.SendMsg;

namespace AiCard.Controllers
{
    public class TestController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {

            return Json(Bll.VipBLL.RandomCode(), JsonRequestBehavior.AllowGet);
        }


        // 随机给VIP用户加入收益记录
        public ActionResult AutoCreateVip()
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

        public ActionResult AutoCreateVipAmountLog(string userID)
        {
            var vip = db.Vips.FirstOrDefault(s => s.UserID == userID);
            var users = db.Users
                .Where(s => s.Id != userID && s.UserType == UserType.Personal && s.Avatar != null)
                .Select(s => s.Id)
                .ToList();
            int n = users.Count;
            while (n > 1)
            {
                n--;
                int k = Comm.Random.Next(n + 1);
                var value = users[k];
                users[k] = users[n];
                users[n] = value;
            }
            var vip1 = users.Take(vip.FreeChildCount).ToList();
            DateTime start = new DateTime(2018, 10, 1);
            DateTime end = new DateTime(2018, 11, 22);
            var sec = ((int)((end - start).TotalSeconds)) / vip1.Count;

            var logs = vip1.Select(s =>
            {
                int index = vip1.IndexOf(s);
                return new VipAmountLog
                {
                    UserID = userID,
                    CreateDateTime = start.AddSeconds(index * sec),
                    Amount = 3,
                    Before = 0,
                    SourceUserID = s,
                    Type = VipAmountLogType.NewCard,
                    VipID = vip.ID,
                };
            }).ToList();

            n = 0;
            List<string> vip2 = new List<string>();
            while (n < vip.VipChild2ndCount)
            {
                var newUser = vip1.Skip(Comm.Random.Next(0, vip1.Count)).FirstOrDefault();
                if (!vip2.Any(s => s == newUser))
                {
                    vip2.Add(newUser);
                    var log = logs.FirstOrDefault(s => s.SourceUserID == newUser);
                    logs.Add(new VipAmountLog
                    {
                        Amount = (86 * 0.5m),
                        CreateDateTime = log.CreateDateTime.AddMinutes(3),
                        SourceUserID = log.SourceUserID,
                        Type = VipAmountLogType.NewChild2nd,
                        UserID = userID,
                        VipID = vip.ID,
                    });
                    n++;
                }
            }

            var vip3 = users.Where(s => !vip1.Contains(s)).Take(vip.VipChild3rdCount);
            logs.AddRange(vip3.Select(s => new VipAmountLog
            {
                Amount = 86 * 0.1m,
                CreateDateTime = start.AddSeconds(sec * Comm.Random.Next(0, vip.FreeChildCount)),
                SourceUserID = s,
                Type = VipAmountLogType.NewChild3rd,
                UserID = userID,
                VipID = vip.ID
            }));
            vip.TotalAmount = logs.Sum(s => s.Amount);
            List<decimal> takes = new List<decimal>();
            for (int i = 0; i < (int)(vip.TotalAmount / 50); i++)
            {
                takes.Add(50);
            }
            for (int i = 0; i < 3; i++)
            {
                var t = takes.Take(Comm.Random.Next(1, takes.Count()));
                if (t.Count() > 0)
                {
                    logs.Add(new VipAmountLog
                    {
                        Amount = -t.Take(Comm.Random.Next(1, takes.Count())).Sum(),
                        CreateDateTime = logs.Min(s => s.CreateDateTime).AddDays(Comm.Random.Next(1, (int)(end - start).TotalDays)),
                        Type = VipAmountLogType.Forward,
                        UserID = userID,
                        VipID = vip.ID,
                    });
                    takes.RemoveRange(0, t.Count());
                }

            }
            vip.Amount = vip.TotalAmount - logs.Where(s => s.Type == VipAmountLogType.Forward).Sum(s => s.Amount);
            logs = logs.OrderBy(s => s.CreateDateTime).ToList();
            db.VipAmountLogs.AddRange(logs);
            db.SaveChanges();
            return Json(Comm.ToJsonResult("Success", "成功"), JsonRequestBehavior.AllowGet);
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
        public ActionResult TestSendMsg()
        {
            return View();
        }
        public ActionResult SendMsgTo()
        {
            SendMsg s = new SendMsg();
            string resultstr = s.SendSMS("18820716886", "347836", "你好");
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