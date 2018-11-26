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
                                  VipType = v.Type,
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
                s.VipType
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
                var vip = db.Vips.FirstOrDefault(s => s.CardID == card.ID);
                var data = new
                {
                    card.Name,
                    card.Avatar,
                    card.Position,
                    card.PhoneNumber,
                    card.Mobile,
                    card.Email,
                    card.WeChatCode,
                    card.Remark,
                    card.Video,
                    card.Voice,
                    card.Info,
                    Images = card.Images.SplitToArray<string>() ?? new List<string>(),
                    PCardID = card.ID,
                    Enterprise = card.Enterprise,
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
                if (!ModelState.IsValid)
                {
                    return Json(Comm.ToJsonResult("Error", ModelState.FirstErrorMessage()));
                }
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
                    if (Reg.IsEmail(model.Email.Trim()))
                    {
                        pCard.Email = model.Email.Trim();
                    }
                    else
                    {
                        return Json(Comm.ToJsonResult("Error", "邮箱格式不正确"));
                    }

                }
                if (model.Mobile != null)
                {
                    pCard.Mobile = model.Mobile.Trim();
                }
                if (model.PhoneNumber != null)
                {
                    pCard.PhoneNumber = model.PhoneNumber.Trim();
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
                    pCard.City = model.City;
                }
                if (model.Province != null)
                {
                    pCard.Province = model.Province;
                }
                if (model.District != null)
                {
                    pCard.District = model.District;
                }


                db.SaveChanges();
                return Json(Comm.ToJsonResult("Success", "成功"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error500", ex.Message), JsonRequestBehavior.AllowGet);
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