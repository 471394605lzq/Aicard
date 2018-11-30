using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.DAL.Models;
using AiCard.Common;
using AiCard.Models;

namespace AiCard.Controllers
{
    public class CardPersonalController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: VipHome
        public ActionResult HomeList(int page = 1, int pageSize = 20)
        {
            var queryCards = (from c in db.CardPersonals
                              from v in db.Vips
                              where c.Enable && c.ID == v.CardID
                              select new
                              {
                                  c.ID,
                                  c.Name,
                                  c.Avatar,
                                  c.Birthday,
                                  c.Gender,
                                  c.Position,
                                  v.Type,
                                  c.Industry,
                                  c.UserID,
                                  c.City,
                              });

            var paged = queryCards
                .OrderByDescending(s => s.ID)
                .ToPagedList(page, pageSize);
            var model = paged.Select(s => new
            {
                s.Name,
                PCardID = s.ID,
                s.UserID,
                s.Avatar,
                s.Gender,
                Age = s.Birthday.GetAgeForBirthday(),
                s.Position,
                s.Industry,
                s.City,
                s.Type
            });

            return Json(Common.Comm.ToJsonResultForPagedList(paged, model), JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// Ai雷达 我的主页获取名片信息
        /// </summary>
        /// <param name="pCardID"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult GetCardInfo(int? pCardID, string userID)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userID))
                {
                    return Json(Comm.ToJsonResult("Error", "UserID为空"), JsonRequestBehavior.AllowGet);
                }
                var query = (from c in db.CardPersonals
                             where c.Enable
                             select c);
                if (pCardID.HasValue)
                {
                    query = query.Where(s => s.ID == pCardID);
                }
                else
                {
                    query = query.Where(s => s.UserID == userID);
                }

                var card = query.FirstOrDefault();
                if (card == null)
                {
                    var user = db.Users.FirstOrDefault(s => s.Id == userID);
                    if (user == null)
                    {
                        return Json(Comm.ToJsonResult("UserNoFound", "用户不出在"), JsonRequestBehavior.AllowGet);
                    }
                    return Json(Comm.ToJsonResult("PCardNoFound", "名片不存在", new
                    {
                        user.Avatar,
                        Name = user.NickName,
                        UserID = user.Id
                    }), JsonRequestBehavior.AllowGet);
                }
                Bll.UserLogs.Add(new UserLog
                {
                    UserID = userID,
                    RelationID = card.ID,
                    TargetUserID = card.UserID,
                    Type = Common.Enums.UserLogType.CardPersonalRead
                });
                var vip = db.Vips.FirstOrDefault(s => s.CardID == card.ID);
                //获取最近访问的6个人头像
                var leastUsers = (from l in db.UserLogs
                                  from u in db.Users
                                  where l.Type == Common.Enums.UserLogType.CardPersonalRead
                                    && l.RelationID == card.ID
                                    && u.Id == l.UserID
                                  select new
                                  {
                                      UserID = u.Id,
                                      u.Avatar,
                                      l.CreateDateTime
                                  }).GroupBy(s => new { s.UserID, s.Avatar })
                                   .Select(s => new
                                   {
                                       s.Key.UserID,
                                       s.Key.Avatar,
                                       CreateDateTime = s.Max(x => x.CreateDateTime)
                                   })
                                   .OrderByDescending(s => s.CreateDateTime)
                                   .Take(6)
                                   .ToList();


                var data = new
                {
                    card.Name,
                    card.Avatar,
                    card.Position,
                    Phone = card.PhoneNumber,
                    card.Mobile,
                    card.Email,
                    card.WeChatCode,
                    card.Remark,
                    card.Video,
                    card.Voice,
                    card.Info,
                    card.Industry,
                    Images = card.Images.SplitToArray<string>() ?? new List<string>(),
                    PCardID = card.ID,
                    EnterpriseName = card.Enterprise,
                    card.Address,
                    card.City,
                    card.District,
                    card.Province,
                    FullAddress = $"{card.Province}{card.City}{card.District}{card.Address}",
                    Lat = card.Lat,
                    Lng = card.Lng,
                    Poster = Comm.ResizeImage(card.Poster),
                    WeChatMiniQrCode = Comm.ResizeImage(card.WeChatMiniQrCode),
                    Age = card.Birthday.GetAgeForBirthday(),
                    card.Gender,
                    Birthday = card.Birthday?.ToString("yyyy-MM-dd"),
                    Code = vip.Code,
                    Type = vip.Type,
                    Viewers = leastUsers.Select(s => s.Avatar).ToList()
                };
                return Json(Comm.ToJsonResult("Success", "成功", data), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Ai雷达 编辑名片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult EditCardInfo(PersonalCardViewModels model)
        {
            try
            {
                var pCard = db.CardPersonals.FirstOrDefault(s => s.ID == model.PCardID && s.UserID == model.UserID);
                if (pCard == null)
                {
                    return Json(Comm.ToJsonResult("Error", "名片不存在"));
                }
                //if (!ModelState.IsValid)
                //{
                //    return Json(Comm.ToJsonResult("Error", ModelState.FirstErrorMessage()));
                //}
                if (model.Name != null)
                {
                    if (!string.IsNullOrWhiteSpace(model.Name.Trim()))
                    {
                        pCard.Name = model.Name;
                    }
                    else
                    {
                        return Json(Comm.ToJsonResult("Error", "名字不能空"));
                    }

                }
                if (model.Email != null)
                {
                    if (string.IsNullOrWhiteSpace(model.Email))
                    {
                        pCard.Email = null;
                    }
                    else if (Reg.IsEmail(model.Email))
                    {
                        pCard.Email = model.Email;
                    }
                    else
                    {
                        return Json(Comm.ToJsonResult("Error", "邮箱格式不正确"));
                    }


                }
                if (model.Mobile != null)
                {
                    if (string.IsNullOrWhiteSpace(model.Mobile))
                    {
                        pCard.Mobile = null;
                    }
                    else if (Reg.IsMobile(model.Mobile))
                    {
                        pCard.Mobile = model.Mobile;
                    }
                    else
                    {
                        return Json(Comm.ToJsonResult("Error", "手机号格式不正确"));
                    }
                }
                if (model.Phone != null)
                {
                    if (string.IsNullOrWhiteSpace(model.Phone))
                    {
                        pCard.PhoneNumber = null;
                    }
                    else if (Reg.IsPhone(model.Phone))
                    {
                        pCard.PhoneNumber = model.Phone;
                    }
                    else
                    {
                        return Json(Comm.ToJsonResult("Error", "座机号格式不正确"));
                    }

                }
                if (model.Position != null)
                {
                    pCard.Position = model.Position.Trim();
                }
                if (model.Remark != null)
                {
                    pCard.Remark = model.Remark.Trim();
                }
                if (model.WeChatCode != null)
                {
                    pCard.WeChatCode = model.WeChatCode.Trim();
                }
                if (model.Avatar != null)
                {
                    pCard.Avatar = model.Avatar.Trim();
                }
                if (model.Images != null)
                {
                    pCard.Images = string.Join(",", model.Images);
                }
                if (model.Video != null)
                {
                    pCard.Video = model.Video.Trim();
                }
                if (model.Voice != null)
                {
                    pCard.Voice = model.Voice.Trim();
                }
                if (model.Info != null)
                {
                    pCard.Info = model.Info.Trim();
                }
                if (model.Industry != null)
                {
                    pCard.Industry = model.Industry.Trim();
                }
                if (model.Birthday != null)
                {
                    if (string.IsNullOrWhiteSpace(model.Birthday))
                    {
                        pCard.Birthday = null;
                    }
                    else
                    {
                        DateTime birthday;
                        if (DateTime.TryParse(model.Birthday, out birthday))
                        {
                            pCard.Birthday = birthday;
                        }
                        else
                        {
                            Json(Comm.ToJsonResult("Error", "生日的格式不正确"));
                        }
                    }
                }
                if (model.Gender.HasValue)
                {
                    pCard.Gender = model.Gender.Value;
                }
                if (model.Address != null)
                {
                    pCard.Address = model.Address.Trim();
                }
                if (model.City != null)
                {
                    pCard.City = model.City.Trim();
                }
                if (model.Province != null)
                {
                    pCard.Province = model.Province.Trim();
                }
                if (model.District != null)
                {
                    pCard.District = model.District.Trim();
                }
                if (model.EnterpriseName != null)
                {
                    pCard.Enterprise = model.EnterpriseName.Trim();
                }
                if (model.Lat != null)
                {
                    if (string.IsNullOrWhiteSpace(model.Lat))
                    {
                        pCard.Lat = null;
                    }
                    else
                    {
                        double temp;
                        if (double.TryParse(model.Lat, out temp))
                        {
                            pCard.Lat = temp;
                        }
                        else
                        {
                            Json(Comm.ToJsonResult("Error", "纬度格式不正确"));
                        }
                    }
                }
                if (model.Lng != null)
                {
                    if (string.IsNullOrWhiteSpace(model.Lng))
                    {
                        pCard.Lng = null;
                    }
                    else
                    {
                        double temp;
                        if (double.TryParse(model.Lng, out temp))
                        {
                            pCard.Lng = temp;
                        }
                        else
                        {
                            Json(Comm.ToJsonResult("Error", "纬度格式不正确"));
                        }
                    }
                }
                db.SaveChanges();
                return Json(Comm.ToJsonResult("Success", "成功"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error500", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GeneratePosters(int pCardID, string img)
        {
            var card = db.CardPersonals.FirstOrDefault(s => s.ID == pCardID);
            if (card == null)
            {
                return Json(Comm.ToJsonResult("CardNoFound", "名片不存在"));
            }
            if (!string.IsNullOrWhiteSpace(card.WeChatMiniQrCode))
            {
                var path = Comm.MergePosterPersonalImage(new Common.CommModels.DrawingPictureProsonal
                {
                    Avatar = card.Avatar,
                    BgImage = img,
                    Name = card.Name,
                    QrCode = card.WeChatMiniQrCode,
                    FileName = $"{pCardID}_{new System.IO.FileInfo(img.Replace("http://", "~").Replace("http://", "~")).Name}"
                });
                return Json(Comm.ToJsonResult("Success", "成功", Url.ContentFull(path)));
            }
            return Json(Comm.ToJsonResult("QrCodeNoFound", "分享二维码不存在"));
        }

        /// <summary>
        /// 刷新所有
        /// </summary>
        /// <returns></returns>
        [Authorize(Users = "admin")]
        public ActionResult ReflashWeCharQrCode()
        {
            Common.WeChat.IConfig config = new Common.WeChat.ConfigMiniPersonal();
            var api = new Common.WeChat.WeChatMinApi(config);
            Func<int, string> getQrCode = pCardID =>
            {
                var p = new Dictionary<string, string>();
                p.Add("PCardID", pCardID.ToString());
                return api.GetWXACodeUnlimit(Common.WeChat.WeChatPagePersonal.CardDetail, p);
            };
            var cards = db.CardPersonals.Where(s => s.WeChatMiniQrCode == null).ToList();
            foreach (var item in cards)
            {
                item.WeChatMiniQrCode = getQrCode(item.ID);
            }
            db.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult GetDefaultPosters()
        {
            var imgs = new string[] { "~/Content/Images/Poster/img_poster1.png",
                "~/Content/Images/Poster/img_poster2.png",
                "~/Content/Images/Poster/img_poster3.png",
                "~/Content/Images/Poster/img_poster4.png",
                "~/Content/Images/Poster/img_poster5.png"};
            imgs = imgs.Select(s => Url.ContentFull(s)).ToArray();
            return Json(Comm.ToJsonResult("Success", "成功", imgs));
        }

        [HttpGet]
        public ActionResult GetPostersAfterGenerate(int pCardID)
        {
            var imgs = new string[] { "~/Content/Images/Poster/img_poster1.png",
                "~/Content/Images/Poster/img_poster2.png",
                "~/Content/Images/Poster/img_poster3.png",
                "~/Content/Images/Poster/img_poster4.png",
                "~/Content/Images/Poster/img_poster5.png"};
            imgs = imgs.Select(s => Url.ContentFull(s)).ToArray();
            var card = db.CardPersonals.FirstOrDefault(s => s.ID == pCardID);
            if (card == null)
            {
                return Json(Comm.ToJsonResult("CardNoFound", "名片不存在"), JsonRequestBehavior.AllowGet);
            }
            if (!string.IsNullOrWhiteSpace(card.WeChatMiniQrCode))
            {
                try
                {
                    for (int i = 0; i < imgs.Length; i++)
                    {
                        imgs[i] = Comm.MergePosterPersonalImage(new Common.CommModels.DrawingPictureProsonal
                        {
                            Avatar = card.Avatar,
                            BgImage = imgs[i],
                            Name = card.Name,
                            QrCode = card.WeChatMiniQrCode,
                            FileName = $"{pCardID}_p{i}"
                        });
                    }

                    return Json(Comm.ToJsonResult("Success", "成功", imgs.Select(s => Url.ContentFull(s))), JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
                }

            }
            return Json(Comm.ToJsonResult("QrCodeNoFound", "分享二维码不存在"), JsonRequestBehavior.AllowGet);

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