using AiCard.DAL.Models;
using AiCard.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AiCard.Controllers
{
    /// <summary>
    /// 雷达统计(专门用于Ai雷达中的雷达模块)
    /// </summary>
    public class RadarPoolController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// 获取昨日概括
        /// </summary>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult GetYesterdayEpitomeForRadar(int enterpriseid,string userid) {
            try
            {
                if (!db.Cards.Any(s => s.EnterpriseID == enterpriseid && s.UserID == userid))
                {
                    return Json(Comm.ToJsonResult("CardNoFound", "卡片不存在"));
                }
                //拼接参数
                SqlParameter[] parameters = {
                        new SqlParameter("@enterpriseid", SqlDbType.Int),
                        new SqlParameter("@userid", SqlDbType.NVarChar),
                        new SqlParameter("@browsetype", SqlDbType.Int),
                        new SqlParameter("@consultcusttype", SqlDbType.Int)
                    };
                parameters[0].Value = enterpriseid;
                parameters[1].Value = userid;
                parameters[2].Value = Common.Enums.UserLogType.CardRead;
                parameters[3].Value = Common.Enums.UserLogType.CardCon;
                //
                string sqlstr = string.Format(@"GetYesterday_EpitomeForRadar @enterpriseid,@userid,@browsetype,@consultcusttype");
                List<GetYesterdayEpitomeForRadarModel> data = db.Database.SqlQuery<GetYesterdayEpitomeForRadarModel>(sqlstr, parameters).ToList();
                return Json(Comm.ToJsonResult("Success", "成功", data), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 趋势分析
        /// </summary>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult TrendAnalysis(int enterpriseid, string userid,int timenumber)
        {
            try
            {
                if (!db.Cards.Any(s => s.EnterpriseID == enterpriseid && s.UserID == userid))
                {
                    return Json(Comm.ToJsonResult("CardNoFound", "卡片不存在"));
                }
                //拼接参数
                SqlParameter[] parameters = {
                        new SqlParameter("@enterpriseid", SqlDbType.Int),
                        new SqlParameter("@userid", SqlDbType.NVarChar),
                        new SqlParameter("@timenumber", SqlDbType.Int),
                        new SqlParameter("@browsetype", SqlDbType.Int),
                        new SqlParameter("@consultcusttype", SqlDbType.Int)
                    };
                parameters[0].Value = enterpriseid;
                parameters[1].Value = userid;
                parameters[2].Value = timenumber;
                parameters[3].Value = Common.Enums.UserLogType.CardRead;
                parameters[4].Value = Common.Enums.UserLogType.CardCon;

                string sqlstr = string.Format(@"TrendAnalysis @enterpriseid,@userid,@timenumber,@browsetype,@consultcusttype");
                List<TrendAnalysisModel> data = db.Database.SqlQuery<TrendAnalysisModel>(sqlstr, parameters).ToList();
                return Json(Comm.ToJsonResult("Success", "成功", data), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        private class TrendAnalysisModel {
            //小时
            public string hourstr { get; set; }
            //数量
            public int counts { get; set; }
            //类型
            public string typestr { get; set; }
        }
        private class GetYesterdayEpitomeForRadarModel {

            //统计标题
            public string pooltypestr { get; set; }
            //统计数量
            public int poolcont { get; set; }
            //较前日相差数量
            public int differcount { get; set; }
            //是升或者降或者持平
            public string riseorfallstr { get; set; }
        }
    }
}