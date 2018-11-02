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
        public ActionResult Index(int enterpriseID, string filter, int page = 1, int pageSize = 20)
        {
            var query = from c in db.Cards
                        from e in db.Enterprises
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
                            Position = c.Position
                        };
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(s => s.Name.Contains(filter)
                    || s.Position.Contains(filter)
                    || s.Mobile.Contains(filter)
                    || s.Email.Contains(filter));
            }
            var paged = query.OrderBy(s => s.CardID).ToPagedList(page, pageSize);
            foreach (var item in paged)
            {
                item.Avatar = item.Avatar.SplitToArray<string>()[0];
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
                            c.WeChatMiniQrCode
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
                //WeChatMiniQrCode = card.WeChatMiniQrCode
                WeChatMiniQrCode = "http://image.dtoao.com/WeChatQrCodeDemo.png"
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
        [AllowCrossSiteJson]
        public ActionResult GetPoster(int cardID)
        {
            try
            {
                var query = (from c in db.Cards
                             from e in db.Enterprises
                             where c.EnterpriseID == e.ID && c.ID == cardID
                             select c).FirstOrDefault();
                if (query == null) {
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
                List<tagmodel> listst = new List<tagmodel>();
                if (cardm.Count > 0)
                {
                    for (int i = 0; i < cardm.Count; i++)
                    {
                        if (i == 0)
                        {
                            tagmodel tm = new tagmodel();
                            tm.tagname = cardm[i].Name + " " + cardm[i].Count.ToString();
                            tm.tagstyle = cardm[i].Style.GetDisplayName();
                            listst.Add(tm);
                        }
                    }
                }
                var dm = new DrawingPictureModel
                {
                    AvatarPath = DrawingPictures.DownloadImg(query.Avatar, "avatar.png", 834, 834),
                    CompanyName = qe.Name,
                    LogoPath = DrawingPictures.DownloadImg(qe.Logo, "logo.png", 96, 96),
                    Position = query.Position,
                    QrPath = DrawingPictures.DownloadImg(query.WeChatMiniQrCode, "qrcode.png", 240, 240),
                    Remark = query.Remark,
                    UserName = query.Name,
                    taglist = listst
                };
                //调用生成海报方法
                string returnpath = Comm.MergePosterImage(dm);
                var data = new
                {
                    Posterpath = returnpath
                };
                return Json(Comm.ToJsonResult("Success", "成功", data), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
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