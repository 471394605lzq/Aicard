using AiCard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AiCard.Controllers
{
    [Authorize]
    public class EnterpriseController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public void Sidebar(string name = "企业信息")
        {
            ViewBag.Sidebar = name;

        }
        /// <summary>
        /// 获取企业信息
        /// </summary>
        /// <param name="id">企业id</param>
        /// <returns>企业Enterprise model</returns>
        public ActionResult Index()
        {
            Sidebar();
            string cookieuserid = this.GetAccountData().UserID;//从cookie中读取userid
            string decryptuserid = Comm.Decrypt(cookieuserid);
            var user = db.Users.FirstOrDefault(s => s.Id == decryptuserid);
            if (user.Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = (from e in db.Enterprises
                         from u in db.Users
                         where e.AdminID == u.Id && e.ID == user.EnterpriseID
                         select e).FirstOrDefault();
            var models = new Enterprise
            {
                ID = model.ID,
                Code = model.Code,
                Province = model.Province,
                City = model.City,
                Address = model.Address,
                Email = model.Email,
                HomePage = model.HomePage,
                Logo = model.Logo,
                Info = model.Info,
                District = model.District,
                Enable = model.Enable,
                CardCount = model.CardCount,
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
                AdminID = model.AdminID,
                RegisterDateTime = model.RegisterDateTime,
                AppID = model.AppID,
                Lat = model.Lat,
                Lng = model.Lng
            };
            if (models == null)
            {
                return HttpNotFound();
            }
            return View(models);
        }
    }
}