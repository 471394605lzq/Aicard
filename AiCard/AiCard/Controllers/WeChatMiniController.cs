using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AiCard.DAL.Models;
namespace AiCard.Controllers
{
    public class WeChatMiniController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        public ActionResult AddForm(string userID, string[] formIDs)
        {
            if (formIDs.Any(s => s == "the formId is a mock one"))
            {
                return Json(Common.Comm.ToJsonResult("FormIDInvalid", "FormID无效"));
            }
            if (!db.Vips.Any(s => s.UserID == userID))
            {
                return Json(Common.Comm.ToJsonResult("VipNoFound", "未注册"));
            }
            var config = new Common.WeChat.ConfigMiniPersonal();
            var user = db.Users.FirstOrDefault(s => s.Id == userID);
            var openID = new Bll.Users.UserOpenID(user).SearchOpenID(config.AppID);
            var list = new List<WeChatMiniNotifyForm>();
            foreach (var item in formIDs)
            {
                list.Add(new WeChatMiniNotifyForm
                {
                    AppID = config.AppID,
                    CreateDateTime = DateTime.Now,
                    EndDateTime = DateTime.Now.AddDays(7).AddMinutes(-1),
                    FormID = item,
                    OpenID = openID,
                    UserID = userID
                });

            }
            db.WeChatMiniNotifyForms.AddRange(list);
            db.SaveChanges();
            var del = db.WeChatMiniNotifyForms
                .Where(s => s.EndDateTime < DateTime.Now);
            db.WeChatMiniNotifyForms.RemoveRange(del);
            db.SaveChanges();
            var count = db.WeChatMiniNotifyForms
                .Where(s => s.UserID == userID)
                .Count();

            return Json(Common.Comm.ToJsonResult("Success", "成功", new { Count = count }));
        }
    }
}