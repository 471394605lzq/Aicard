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
                        where e.ID == enterpriseID && c.EnterpriseID == e.ID
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
                        }).FirstOrDefault();
            if (card == null)
            {
                return Json(Comm.ToJsonResult("NoFound", "卡片不存在"));
            }
            var likeCount = db.UserLogs.Count(s => s.Type == UserLogType.CardLike && s.RelationID == cardID);
            var viewCount = db.UserLogs.Count(s => s.Type == UserLogType.CardRead && s.RelationID == cardID);
            var hadLike = db.UserLogs.Any(s => s.Type == UserLogType.CardLike && s.UserID == userID);
            var leastUsers = (from u in db.Users
                              from l in db.UserLogs
                              where u.Id == l.UserID
                                  && l.Type == UserLogType.CardRead
                                  && l.RelationID == cardID
                              orderby l.CreateDateTime descending
                              select new { u.Avatar, u.Id }).Take(6);

            var tab = new List<CardTab>();
            tab.Add(new CardTab { Count = "1000", Name = "服务贴心", Style = CardTabStyle.Green });
            tab.Add(new CardTab { Count = "40", Name = "诚实", Style = CardTabStyle.Blue });
            tab.Add(new CardTab { Count = "10", Name = "能说", Style = CardTabStyle.Orange });
            tab.Add(new CardTab { Count = "165451", Name = "能唱", Style = CardTabStyle.Purple });
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
                likeCount,
                card.Logo,
                card.Mobile,
                NoReadCount = 0,
                Phone = card.PhoneNumber,
                card.Position,
                card.Video,
                card.Voice,
                ViewCount = stringNum(viewCount),
                Viewers = leastUsers.Select(s => s.Avatar).ToList(),
                WeChatCode = card.WeChatCode,
                CardTab = tab,
                QrCode = ""
            };
            return Json(Comm.ToJsonResult("Success", "成功", model), JsonRequestBehavior.AllowGet);
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