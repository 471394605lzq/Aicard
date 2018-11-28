using AiCard.Common;
using Newtonsoft.Json.Linq;
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
        public ActionResult GetArea(int type, string province, string city)
        {
            try
            {
                //List<Pcas> listpcas = ChinaPCAS.GetALL();
                //var data = new
                //{
                //    data = listpcas
                //};
                //return Json(Comm.ToJsonResult("Success", "获取成功", data), JsonRequestBehavior.AllowGet);
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

        /// <summary>
        /// 生成jssdk签名字符串
        /// </summary>
        /// <param name="url"></param>
        /// <param name="noncestr"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult CreateJsSign(string url, string noncestr, string timestamp)
        {
            try
            {
                Common.WeChat.IConfig config = new Common.WeChat.ConfigWeChatWork();
                var wechat = new Common.WeChat.WeChatApi(config);
                string signstr = wechat.JsSign(url, noncestr, timestamp);
                var returndata = new
                {
                    data = signstr
                };
                return Json(Comm.ToJsonResult("Success", "获取成功", returndata), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Success", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UploadWeChatMedia(string mediaID)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(mediaID))
                {
                    return Json(Comm.ToJsonResult("Error", "mediaID不能为null"));
                }
                var server = Common.CommModels.UploadServer.QinQiu;
                Common.WeChat.IConfig config = new Common.WeChat.ConfigWeChatWork();
                var wechat = new Common.WeChat.WeChatApi(config);
                string filePath = wechat.GetTempMedia(mediaID, server, ".amr");
                return Json(Comm.ToJsonResult("Success", "成功", new
                {
                    FileUrl = filePath,
                    FileFullUrl = server == Common.CommModels.UploadServer.Local ? Url.ContentFull(filePath) : filePath
                }));
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message));
            }
        }

        [HttpPost]
        public ActionResult WriteLog(string name, string message, Common.Enums.DebugLogLevel level = Common.Enums.DebugLogLevel.Normal)
        {
            if (Comm.GetConfig<bool>("DebugLogApi"))
            {
                try
                {
                    Comm.WriteLog(name, message, level);
                }
                catch (Exception ex)
                {
                    return Json(Comm.ToJsonResult("Error", ex.Message));
                }
                return Json(Comm.ToJsonResult("Success", "成功"));
            }
            else
            {
                return Json(Comm.ToJsonResult("WriteLogDisable", "已关闭"));
            }
        }
    }
}