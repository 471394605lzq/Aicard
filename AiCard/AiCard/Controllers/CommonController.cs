using AiCard.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AiCard.Controllers
{
    public class CommonController : Controller
    {
        /// <summary>
        /// 获取区域信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult GetArea(int type,string province,string city)
        {
            try
            {
                if (type == 0)
                {
                    var datap = new
                    {
                        data = ChinaPCAS.GetP()//省份
                    };
                    return Json(Comm.ToJsonResult("Success", "获取成功", datap), JsonRequestBehavior.AllowGet);
                }
                else if (type == 1)
                {
                    var datac = new
                    {
                        data = ChinaPCAS.GetC(province)//城市
                    };
                    return Json(Comm.ToJsonResult("Success", "获取成功", datac), JsonRequestBehavior.AllowGet);
                }
                else if (type == 2)
                {
                    var datad = new
                    {
                        data = ChinaPCAS.GetA(province, city)//街道
                    };
                    return Json(Comm.ToJsonResult("Success", "获取成功", datad), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(Comm.ToJsonResult("Error", "类型不正确"), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
    }
}