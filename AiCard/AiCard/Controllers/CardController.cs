using AiCard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using AiCard.Enums;
using System.Net;
using AiCard.Models.CommModels;

namespace AiCard.Controllers
{

    public class CardController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [AllowCrossSiteJson]
        public ActionResult Index(int enterpriseID, string userID, string filter, int page = 1, int pageSize = 20)
        {
            var query = from c in db.Cards
                        from e in db.Enterprises
                        join t in db.UserCardTops
                            .Where(s => s.UserID == userID)
                            on c.ID equals t.CardID
                            into ct
                        where e.ID == enterpriseID
                            && c.EnterpriseID == e.ID
                            && c.Enable
                        select new CardListViewModel
                        {
                            Avatar = c.Avatar,
                            Email = c.Email,
                            CardID = c.ID,
                            Logo = e.Logo,
                            Mobile = c.Mobile,
                            Name = c.Name,
                            Position = c.Position,
                            IsTop = ct.Any(),
                            CreateDateTime = ct.FirstOrDefault() == null ? null : (DateTime?)ct.FirstOrDefault().CreateDateTime,
                        };
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(s => s.Name.Contains(filter)
                    || s.Position.Contains(filter)
                    || s.Mobile.Contains(filter)
                    || s.Email.Contains(filter));
            }
            var paged = query
                .OrderByDescending(s => s.CreateDateTime)
                .ThenBy(s => s.CardID)
                .ToPagedList(page, pageSize);
            foreach (var item in paged)
            {
                item.Avatar = item.Avatar.SplitToArray<string>()?[0];
            }
            return Json(Comm.ToJsonResultForPagedList(paged, paged), JsonRequestBehavior.AllowGet);
        }

        [AllowCrossSiteJson]
        public ActionResult Detail(int cardID, string userID)
        {
            var card = (from c in db.Cards
                        from e in db.Enterprises
                        where c.EnterpriseID == e.ID && c.ID == cardID
                        select new
                        {
                            c.ID,
                            e.Address,
                            c.Avatar,
                            c.Email,
                            c.EnterpriseID,
                            c.Gender,
                            c.Images,
                            c.Info,
                            c.Mobile,
                            c.PhoneNumber,
                            c.Position,
                            c.Remark,
                            c.UserID,
                            c.Video,
                            c.Voice,
                            c.WeChatCode,
                            c.Name,
                            EName = e.Name,
                            e.Lat,
                            e.Lng,
                            e.Logo,
                            c.WeChatMiniQrCode,
                            c.Poster
                        }).FirstOrDefault();
            if (card == null)
            {
                return Json(Comm.ToJsonResult("NoFound", "卡片不存在"));
            }
            var likeCount = db.UserLogs.Count(s => s.Type == UserLogType.CardLike && s.RelationID == cardID);
            var viewCount = db.UserLogs
                .Where(s => s.Type == UserLogType.CardRead && s.RelationID == cardID)
                .GroupBy(s => s.UserID)
                .Count();
            var hadLike = db.UserLogs.Any(s => s.RelationID == cardID
                && s.Type == UserLogType.CardLike
                && s.UserID == userID);
            //获取最近访问的6个人头像
            var leastUsers = (from l in db.UserLogs
                              from u in db.Users
                              where l.Type == UserLogType.CardRead
                                && l.RelationID == cardID
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
            var tab = (from t in db.CardTabs
                       join l in db.UserLogs.Where(s => s.Type == UserLogType.CardTab)
                        on t.ID equals l.RelationID into tl
                       join ul in db.UserLogs.Where(s => s.Type == UserLogType.CardTab && s.UserID == userID)
                        on t.ID equals ul.RelationID into tl2
                       where t.CardID == cardID
                       select new
                       {
                           t.ID,
                           t.CardID,
                           t.Count,
                           t.Name,
                           t.Style,
                           HadLike = tl2.Any()
                       }).ToList();

            Func<int, string> stringNum = num =>
            {
                if (num < 1000)
                {
                    return num.ToString();
                }
                else
                {
                    return $"{num / 1000}k".ToString();
                }
            };

            var model = new
            {
                card.Address,
                Avatar = card.Avatar.SplitToArray<string>(),
                CardID = card.ID,
                Email = card.Email,
                EnterpriseID = card.EnterpriseID.Value,
                EnterpriseName = card.EName,
                card.Name,
                HadLike = hadLike,
                Images = card.Images.SplitToArray<string>(),
                card.Info,
                card.Lat,
                card.Lng,
                LikeCount = likeCount.ToString(),
                card.Logo,
                card.Mobile,
                Phone = card.PhoneNumber,
                card.Position,
                card.Video,
                VideoThumbnail = card.Video == null ? null : Comm.ResizeImage(card.Video),
                card.Voice,
                ViewCount = stringNum(viewCount),
                Viewers = leastUsers.Select(s => s.Avatar).ToList(),
                WeChatCode = card.WeChatCode,
                CardTabs = tab.Select(s => new
                {
                    TabID = s.ID,
                    Count = s.Count.ToString(),
                    s.Name,
                    s.Style,
                    s.HadLike
                }),
                WeChatMiniQrCode = card.WeChatMiniQrCode,
                Poster = Comm.ResizeImage(card.Poster)
            };
            try
            {
                Bll.UserLogs.Add(new UserLog
                {
                    UserID = userID,
                    RelationID = cardID,
                    Type = UserLogType.CardRead
                });
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
            }

            return Json(Comm.ToJsonResult("Success", "成功", model), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 生成海报
        /// </summary>
        /// <param name="cardID"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult GetPoster(int cardID)
        {

            try
            {
                var query = (from c in db.Cards
                             from e in db.Enterprises
                             where c.EnterpriseID == e.ID && c.ID == cardID
                             select c).FirstOrDefault();
                if (query == null)
                {
                    return Json(Comm.ToJsonResult("Error", "该名片不存在"), JsonRequestBehavior.AllowGet);
                }
                var qe = (from e in db.Enterprises
                          where e.ID == query.EnterpriseID
                          select e).FirstOrDefault();

                var cardm = (from ct in db.CardTabs
                             where ct.CardID == cardID
                             orderby ct.Count descending
                             select new
                             {
                                 Name = ct.Name,
                                 Count = ct.Count,
                                 Style = ct.Style
                             }).Take(2).ToList();
                List<TagModel> listst = new List<TagModel>();
                if (cardm.Count > 0)
                {
                    for (int i = 0; i < cardm.Count; i++)
                    {
                        if (i == 0)
                        {
                            TagModel tm = new TagModel();
                            tm.TagName = cardm[i].Name + " " + cardm[i].Count.ToStrForm(4, "");
                            tm.TagStyle = cardm[i].Style;
                            listst.Add(tm);
                        }
                    }
                }
                var dm = new DrawingPictureModel
                {
                    AvatarPath = string.IsNullOrWhiteSpace(query.Avatar) ? "" : DrawingPictures.DownloadImg(query.Avatar.SplitToArray<string>(',')[0], $"avatar_{cardID}.png", 834, 834),
                    CompanyName = qe.Name,
                    LogoPath = string.IsNullOrWhiteSpace(qe.Logo) ? "" : DrawingPictures.DownloadImg(qe.Logo, $"logo_{cardID}.png", 96, 96),
                    Position = query.Position,
                    QrPath = string.IsNullOrWhiteSpace(query.WeChatMiniQrCode) ? "" : DrawingPictures.DownloadImg(query.WeChatMiniQrCode, $"qrcode_{cardID}.png", 240, 240),
                    Remark = query.Remark,
                    UserName = query.Name,
                    TagList = listst,
                    PosterImageName = "cardid_" + cardID.ToString()
                };
                //调用生成海报方法
                string returnpath = Comm.MergePosterImage(dm);
                var data = new
                {
                    Posterpath = returnpath
                };
                var card = db.Cards.FirstOrDefault(s => s.ID == cardID);
                card.Poster = returnpath;
                db.SaveChanges();
                return Json(Comm.ToJsonResult("Success", "成功", data), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Ai雷达 我的主页获取名片信息
        /// </summary>
        /// <param name="cardID"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult GetCardInfo(int cardID)
        {
            try
            {
                var query = (from c in db.Cards
                             where c.ID == cardID && c.Enable
                             select c).FirstOrDefault();
                var data = new
                {
                    Name = query.Name,
                    Avatar = query.Avatar.SplitToArray<string>(',')[0],
                    Position = query.Position,
                    PhoneNumber = query.PhoneNumber,
                    Mobile = query.Mobile,
                    Email = query.Email,
                    WeChatCode = query.WeChatCode,
                    Remark = query.Remark,
                    Video = query.Video,
                    Voice = query.Voice,
                    Images = query.Images
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
        public ActionResult EditCardInfo(CardEditViewModel model)
        {
            try
            {
                var t = db.Cards.FirstOrDefault(s => s.ID == model.CardID);
                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    return Json(Comm.ToJsonResult("Error", "姓名不能为空"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    t.Name = model.Name;
                }
                if (model.Email != null)
                {
                    if (string.Empty != model.Email && !Reg.IsEmail(model.Email))
                    {
                        return Json(Comm.ToJsonResult("Error", "邮箱格式不正确"), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        t.Email = model.Email;
                    }
                }
                if (model.Mobile != null)
                {
                    if (string.Empty != model.Mobile && !Reg.IsMobile(model.Mobile))
                    {
                        return Json(Comm.ToJsonResult("Error", "手机号格式不正确"), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        t.Mobile = model.Mobile;
                    }
                }
                if (model.PhoneNumber != null)
                {
                    if (string.Empty != model.PhoneNumber && !Reg.IsMobile(model.PhoneNumber))
                    {
                        return Json(Comm.ToJsonResult("Error", "座机号码格式不正确"), JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        t.PhoneNumber = model.PhoneNumber;
                    }
                }
                if (model.Position != null)
                {
                    t.Position = model.Position;
                }
                if (model.Remark != null)
                {
                    t.Remark = model.Remark;
                }
                if (model.WeChatCode != null)
                {
                    t.WeChatCode = model.WeChatCode;
                }
                if (model.Avatar != null)
                {
                    t.Avatar = model.Avatar;
                }
                if (model.Images != null)
                {
                    t.Images = model.Images;
                }
                if (model.Video != null)
                {
                    t.Video = model.Video;
                }
                if (model.Voice != null)
                {
                    t.Voice = model.Voice;
                }
                if (model.Info != null)
                {
                    t.Info = model.Info;
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
        [AllowCrossSiteJson]
        public ActionResult Top(string userID, int cardID)
        {
            if (!db.Users.Any(s => s.Id == userID))
            {
                return Json(Comm.ToJsonResult("UserNoFound", "用户不存在"));
            }
            if (!db.Cards.Any(s => s.ID == cardID))
            {
                return Json(Comm.ToJsonResult("CardNoFound", "卡片不存在"));
            }
            var top = db.UserCardTops.FirstOrDefault(s => s.UserID == userID && s.CardID == cardID);

            if (top == null)
            {
                db.UserCardTops.Add(new UserCardTop { CardID = cardID, CreateDateTime = DateTime.Now, UserID = userID });
                db.SaveChanges();
                return Json(Comm.ToJsonResult("Success", "置顶成功", true));
            }
            else
            {
                db.UserCardTops.Remove(top);
                db.SaveChanges();
                return Json(Comm.ToJsonResult("Success", "解除置顶", false));
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