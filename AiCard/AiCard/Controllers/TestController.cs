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
using Microsoft.AspNet.Identity.Owin;

namespace AiCard.Controllers
{
    public class TestController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();



        public ActionResult Index()
        {
            var vips = db.CardPersonals.ToList();
            foreach (var item in vips)
            {
                item.View = db.UserLogs.Where(s => s.RelationID == item.ID && s.Type == UserLogType.CardPersonalRead)
                    .GroupBy(s => s.UserID)
                    .Count();
            }
            db.SaveChanges();
            return Json("1", JsonRequestBehavior.AllowGet);
        }


        public string GetWeChatQrCode(int pCardID)
        {
            Common.WeChat.IConfig config = new Common.WeChat.ConfigMiniPersonal();
            var api = new Common.WeChat.WeChatMinApi(config);
            var p = new Dictionary<string, string>();
            p.Add("PCardID", pCardID.ToString());
            return api.GetWXACodeUnlimit(Common.WeChat.WeChatPagePersonal.CardDetail, p);
        }

        public ActionResult CreateUser()
        {
            throw new Exception();
            var txtUsers = System.IO.File.ReadAllText(Request.MapPath("~/Upload/1231.txt"));
            var userList = txtUsers.Replace("\r\n", ",").SplitToArray<string>(',');
            var users = userList.Select(s =>
            {
                var item = s.SplitToArray<string>(' ');
                return new Common.WeChat.UserInfoResult
                {
                    NickName = item[0].Trim(),
                    HeadImgUrl = item[1].Trim(),
                };
            });

            var userManange = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            foreach (var item in users)
            {
                var user = CreateRandom(item);
                if (!db.Users.Any(s => s.NickName == user.NickName))
                {
                    var result = userManange.CreateAsync(user);
                }

            }
            return Json("1", JsonRequestBehavior.AllowGet);
        }

        // 随机给VIP用户加入收益记录
        public ActionResult AutoCreateVip()
        {
            var vips = db.Vips.Where(s => s.Type != VipRank.Default && s.Amount == 0).ToList();
            //int amount = 86;
            foreach (var item in vips)
            {
                //var freeChild = Common.Comm.Random.Next(1, 100);
                //var randomChild2 = Common.Comm.Random.Next(1, freeChild);
                //var randomChild3 = Common.Comm.Random.Next(1, 100);

                //item.TotalAmount = freeChild * 3m + randomChild2 * amount * 0.5m + randomChild3 * amount * 0.1m;
                //var take = 50 * Common.Comm.Random.Next(0, (int)(item.TotalAmount % 50));
                //item.Amount = item.TotalAmount - take;
                //item.Code = Common.Comm.Random.Next(100000, 999999).ToString();
                //item.VipChild2ndCount = randomChild2;
                //item.VipChild3rdCount = randomChild3;
                //item.FreeChildCount = freeChild;
                //item.Type = VipRank.Vip99;
                AutoCreateVipAmountLog(item.UserID);
            }
            db.SaveChanges();
            return Json(vips, JsonRequestBehavior.AllowGet);

        }

        public ActionResult AutoCreateVipAmountLog(string userID)
        {
            int amount = 86;
            var vip = db.Vips.FirstOrDefault(s => s.UserID == userID);
            var freeChild = Common.Comm.Random.Next(1, 100);
            var randomChild2 = Common.Comm.Random.Next(1, freeChild);
            var randomChild3 = Common.Comm.Random.Next(1, 100);

            vip.TotalAmount = freeChild * 3m + randomChild2 * amount * 0.5m + randomChild3 * amount * 0.1m;
            var take = 50 * Common.Comm.Random.Next(0, (int)(vip.TotalAmount % 50));
            vip.Amount = vip.TotalAmount - take;
            vip.VipChild2ndCount = randomChild2;
            vip.VipChild3rdCount = randomChild3;
            vip.FreeChildCount = freeChild;
            db.SaveChanges();
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
            DateTime end = DateTime.Now.Date;
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
            if (takes.Count > 0)
            {
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
            }
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

        private ApplicationUser CreateRandom(Common.WeChat.UserInfoResult model)
        {

            string username, nickname, avart, unionId = model.UnionID;
            var user = db.Users.FirstOrDefault(s => s.WeChatID == unionId);
            nickname = model.NickName;

            avart = model.HeadImgUrl;
            if (!string.IsNullOrWhiteSpace(avart))
            {
                try
                {
                    avart = this.Download(avart);
                }
                catch (Exception)
                {
                    avart = "~/Content/Images/404/avatar.png";
                }
            }
            unionId = model.UnionID;

            #region 把图片传到七牛
            var path = Server.MapPath(avart);
            avart = new Common.Qiniu.QinQiuApi().UploadFile(path, true);
            #endregion

            do
            {
                username = $"rm{DateTime.Now:yyyyMMddHHmmss}{Comm.Random.Next(1000, 9999)}";
            } while (db.Users.Any(s => s.UserName == username));
            if (string.IsNullOrWhiteSpace(nickname))
            {
                nickname = username;
            }
            user = new ApplicationUser
            {
                WeChatID = unionId,
                UserName = username,
                NickName = nickname,
                Avatar = avart,
                RegisterDateTime = DateTime.Now,
                LastLoginDateTime = DateTime.Now,
                UserType = UserType.Personal
            };

            return user;
        }
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
            //string mp3SavePth = System.Web.HttpContext.Current.Request.MapPath($"~/Upload/{DateTime.Now:yyyyMMddHHmmss}{Comm.Random.Next(1000, 9999)}.mp3");
            //Comm.ConvertToMp3($"E:/201811271125152499.amr", mp3SavePth);
            SendMsg s = new SendMsg();
            var resultstr = s.SendSMS("18820716886", "402477", "你好");
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