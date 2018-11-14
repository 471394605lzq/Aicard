using AiCard.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AiCard.Controllers
{
    public class ChinaPCASController : Controller
    {
        // GET: ChinaPCAS
        public ActionResult Getc(string pname)
        {
            List<string> list= ChinaPCAS.GetC(pname);
            return Json(Comm.ToJsonResult("Success", "获取成功", new { data = list }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Geta(string pname,string cname)
        {
            List<string> list = ChinaPCAS.GetA(pname,cname);
            return Json(Comm.ToJsonResult("Success", "获取成功", new { data = list }), JsonRequestBehavior.AllowGet);
        }
    }
}