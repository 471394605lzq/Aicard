using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.Models;
namespace AiCard.Controllers
{
    public class HomePageModularsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        [AllowCrossSiteJson]
        public ActionResult Index(string userID, int cardID)
        {
            if (!db.Users.Any(s => s.Id == userID))
            {
                return Json(Comm.ToJsonResult("UserNoFound", "用户不存在"), JsonRequestBehavior.AllowGet);
            }
            var card = db.Cards.FirstOrDefault(s => s.ID == cardID && s.Enable && s.EnterpriseID.HasValue);
            if (card == null)
            {
                return Json(Comm.ToJsonResult("CardNoFound", "卡片不存在或卡片不是企业"), JsonRequestBehavior.AllowGet);
            }
            var ent = db.Enterprises.FirstOrDefault(s => s.ID == card.EnterpriseID);
            db.HomePageModulars.Where(s => s.EnterpriseID == card.EnterpriseID).OrderBy(s => s.Sort);
            var modulars = new object[]
            {
                new
                {
                    Title = "Banner",
                    Content = new string[]{"http://image.dtoao.com/BaiduShurufa_2018-10-27_16-50-9.png","http://image.dtoao.com/QQ%E5%9B%BE%E7%89%8720181027165225.png" },
                    Type= Enums.HomePageModularType.Banner,
                },
                new {
                    Title = "公司简介",
                    Content ="Hello World<br/>你好",
                    Type= Enums.HomePageModularType.Html,
                },
                new {
                    Title = "产品介绍",
                    Content =$"产品1<br/><img src='http://image.dtoao.com/BaiduShurufa_2018-10-27_16-58-18.png' />",
                    Type= Enums.HomePageModularType.Html,
                },
                new {
                    Title = "核心成员",
                    Content = new string[]{ "http://image.dtoao.com/QQ%E5%9B%BE%E7%89%8720181027165945.png"},
                    Type= Enums.HomePageModularType.Images,
                },
                new {
                    Title = "服务客户",
                    Content = new string[]{ "http://image.dtoao.com/123123.png"},
                    Type= Enums.HomePageModularType.Images,
                },
                new
                {
                    Title = "联系方式",
                    Content = new {
                        Phone=ent.PhoneNumber,
                        Email=ent.Email,
                        Address = new {
                            Name=$"{ent.Province}{ent.City}{ent.District}{ent.Address}" ,
                            Lat=ent.Lat,
                            Lng= ent.Lng
                        }
                    },
                    Type= Enums.HomePageModularType.Contact
                }
            };
            return Json(Comm.ToJsonResult("Success", "成功", modulars), JsonRequestBehavior.AllowGet);
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