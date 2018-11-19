using AiCard.Common;
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
        public ActionResult GetYesterdayEpitomeForRadar(int enterpriseid, string userid)
        {
            try
            {
                if (!db.Cards.Any(s => s.EnterpriseID == enterpriseid && s.UserID == userid))
                {
                    return Json(Comm.ToJsonResult("CardNoFound", "名片不存在"), JsonRequestBehavior.AllowGet);
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
        public ActionResult TrendAnalysis(int enterpriseid, string userid, int timenumber)
        {
            try
            {
                if (!db.Cards.Any(s => s.EnterpriseID == enterpriseid && s.UserID == userid))
                {
                    return Json(Comm.ToJsonResult("CardNoFound", "名片不存在"));
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
        /// <summary>
        /// 获取客户来源数据
        /// </summary>
        /// <param name="userid">名片所属用户ID</param>
        /// <param name="timenumber">时间天数(0表示查询所有)</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult GetCustomerSource(string userid, int timenumber)
        {
            try
            {
                if (!db.EnterpriseUserCustomer.Any(s => s.OwnerID == userid))
                {
                    return Json(Comm.ToJsonResult("CardNoFound", "客户归属用户不存在"));
                }
                //拼接参数
                SqlParameter[] parameters = {
                        new SqlParameter("@userid", SqlDbType.NVarChar),
                        new SqlParameter("@timenumber", SqlDbType.Int),
                    };
                parameters[0].Value = userid;
                parameters[1].Value = timenumber;

                string sqlstr = string.Format(@"GetCustomerSource @userid,@timenumber");
                List<CustomerActionModel> data = db.Database.SqlQuery<CustomerActionModel>(sqlstr, parameters).ToList();
                var resultdata = data.Select(s => new
                {
                    counts = s.counts,
                    allcounts = s.allcounts,
                    source = s.action,
                    sourcename = GetEnumsName(s.action),
                    ratio = s.ratio
                });

                return Json(Comm.ToJsonResult("Success", "成功", resultdata), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 分享路径
        /// </summary>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult SharePath(int enterpriseid, string userid, int timenumber)
        {
            try
            {
                if (!db.Cards.Any(s => s.EnterpriseID == enterpriseid && s.UserID == userid))
                {
                    return Json(Comm.ToJsonResult("CardNoFound", "名片不存在"));
                }

                //int[] actionstr = {
                //   Common.Enums.UserLogType.ShareWeChatFriend.GetHashCode(),
                //   Common.Enums.UserLogType.ShareWeChatGroup.GetHashCode()

                //};
                //拼接参数
                SqlParameter[] parameters = {
                        new SqlParameter("@enterpriseid", SqlDbType.Int),
                        new SqlParameter("@userid", SqlDbType.NVarChar),
                        new SqlParameter("@timenumber", SqlDbType.Int),
                        new SqlParameter("@actionstr1", SqlDbType.Int),
                        new SqlParameter("@actionstr2", SqlDbType.Int)
                    };
                parameters[0].Value = enterpriseid;
                parameters[1].Value = userid;
                parameters[2].Value = timenumber;
                parameters[3].Value = Common.Enums.UserLogType.ShareWeChatFriend;
                parameters[4].Value = Common.Enums.UserLogType.ShareWeChatGroup;

                string sqlstr = string.Format(@"GetSharePath @enterpriseid,@userid,@timenumber,@actionstr1,@actionstr2");
                List<CustomerActionModel> data = db.Database.SqlQuery<CustomerActionModel>(sqlstr, parameters).ToList();
                var resultdata = data.Select(s => new
                {
                    counts = s.counts,
                    allcounts = s.allcounts,
                    sharepath = s.action,
                    sharepathname = GetEnumsName(s.action),
                    ratio = s.ratio
                });

                return Json(Comm.ToJsonResult("Success", "成功", resultdata), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 获取分享页面统计
        /// </summary>
        /// <param name="enterpriseid"></param>
        /// <param name="userid"></param>
        /// <param name="timenumber"></param>
        /// <returns></returns>
        public ActionResult GetSharePage(int enterpriseid, string userid, int timenumber)
        {
            try
            {
                if (!db.Cards.Any(s => s.EnterpriseID == enterpriseid && s.UserID == userid))
                {
                    return Json(Comm.ToJsonResult("CardNoFound", "名片不存在"));
                }
                //拼接参数
                SqlParameter[] parameters = {
                        new SqlParameter("@enterpriseid", SqlDbType.Int),
                        new SqlParameter("@userid", SqlDbType.NVarChar),
                        new SqlParameter("@timenumber", SqlDbType.Int),
                        new SqlParameter("@actionstr1", SqlDbType.Int),
                        new SqlParameter("@actionstr2", SqlDbType.Int),
                        new SqlParameter("@actionstr3", SqlDbType.Int)
                    };
                parameters[0].Value = enterpriseid;
                parameters[1].Value = userid;
                parameters[2].Value = timenumber;
                parameters[3].Value = Common.Enums.UserLogType.ArticleShare;
                parameters[4].Value = Common.Enums.UserLogType.CardShare;
                parameters[5].Value = Common.Enums.UserLogType.HomePageShare;
                string sql = @" SELECT COUNT(ID) counts,[Type] AS [action] FROM dbo.UserLogs 
                            WHERE [Type] IN(@actionstr1,@actionstr2,@actionstr3)  
                            AND TargetUserID=@userid AND TargetEnterpriseID=@enterpriseid AND 
                            CreateDateTime BETWEEN dateadd(day, -@timenumber, dateadd(ms, 0, DATEADD(dd, DATEDIFF(dd, 0, getdate()), 0)))
                            AND dateadd(day, -@timenumber, DATEADD(ms, -3, DATEADD(dd, DATEDIFF(dd, -1, getdate()), 0)))
                            GROUP BY [Type]";
                string sqlstr = string.Format(sql, @"@enterpriseid,@userid,@timenumber,@actionstr1,@actionstr2,@actionstr3");
                List<CustomerActionModel> data = db.Database.SqlQuery<CustomerActionModel>(sqlstr, parameters).ToList();
                var resultdata = data.Select(s => new
                {
                    counts = s.counts,
                    sharepage = s.action,
                    sharepaagename = GetEnumsName(s.action),
                });
                return Json(Comm.ToJsonResult("Success", "成功", resultdata), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 分享次数趋势分析
        /// </summary>
        /// <param name="enterpriseid"></param>
        /// <param name="userid"></param>
        /// <param name="timenumber"></param>
        /// <returns></returns>
        public ActionResult GetShareNumberAnalysis(int enterpriseid, string userid, int timenumber) {
            try
            {
                if (!db.Cards.Any(s => s.EnterpriseID == enterpriseid && s.UserID == userid))
                {
                    return Json(Comm.ToJsonResult("CardNoFound", "名片不存在"));
                }
                //拼接参数
                SqlParameter[] parameters = {
                        new SqlParameter("@enterpriseid", SqlDbType.Int),
                        new SqlParameter("@userid", SqlDbType.NVarChar),
                        new SqlParameter("@timenumber", SqlDbType.Int),
                        new SqlParameter("@actionstr1", SqlDbType.Int),
                        new SqlParameter("@actionstr2", SqlDbType.Int)
                    };
                parameters[0].Value = enterpriseid;
                parameters[1].Value = userid;
                parameters[2].Value = timenumber;
                parameters[3].Value = Common.Enums.UserLogType.ShareWeChatFriend;
                parameters[4].Value = Common.Enums.UserLogType.ShareWeChatGroup;
                string sql = @" SELECT DATENAME(HOUR,CreateDateTime) AS hourstr,COUNT(ID) counts FROM dbo.UserLogs WHERE [Type] IN(@actionstr1,@actionstr2) 
                                AND TargetUserID=@userid AND TargetEnterpriseID=@enterpriseid AND 
                                CreateDateTime BETWEEN dateadd(day, -@timenumber, dateadd(ms, 0, DATEADD(dd, DATEDIFF(dd, 0, getdate()), 0)))AND dateadd(day, -@timenumber, DATEADD(ms, -3, DATEADD(dd, DATEDIFF(dd, -1, getdate()), 0)))
                                GROUP BY DATENAME(HOUR,CreateDateTime)";
                string sqlstr = string.Format(sql, @"@enterpriseid,@userid,@timenumber,@actionstr1,@actionstr2");
                List<TrendAnalysisModel> data = db.Database.SqlQuery<TrendAnalysisModel>(sqlstr, parameters).ToList();
                var resultdata = data.Select(s => new
                {
                    counts = s.counts,
                    hourstr = s.hourstr
                    //typenumber=s.typenumber,
                    //typestr = GetEnumsName(s.typenumber)
                });
                return Json(Comm.ToJsonResult("Success", "成功", resultdata), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 性别占比
        /// </summary>
        /// <param name="enterpriseid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult GetCustomerGenderProportion(int enterpriseid, string userid) {
            try
            {
                if (!db.Cards.Any(s => s.EnterpriseID == enterpriseid && s.UserID == userid))
                {
                    return Json(Comm.ToJsonResult("CardNoFound", "名片不存在"));
                }
                //拼接参数
                SqlParameter[] parameters = {
                        new SqlParameter("@enterpriseid", SqlDbType.Int),
                        new SqlParameter("@userid", SqlDbType.NVarChar)
                };
                parameters[0].Value = enterpriseid;
                parameters[1].Value = userid;
                string sqlstr = string.Format(@"GetCustomerGenderProportion @enterpriseid,@userid");
                List<CustomerActionModel> data = db.Database.SqlQuery<CustomerActionModel>(sqlstr, parameters).ToList();
                var resultdata = data.Select(s => new
                {
                    counts = s.counts,
                    allcounts = s.allcounts,
                    gender = s.action,
                    gendername = GetEnumsName(s.action),
                    ratio = s.ratio
                });

                return Json(Comm.ToJsonResult("Success", "成功", resultdata), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
        //获取客户来源枚举名称
        private string GetEnumsName(int val)
        {
            string returnmane = "";
            try
            {
                returnmane = ((Common.Enums.EnterpriseUserCustomerSource)val).GetDisplayName();
            }
            finally
            {
                returnmane = "未知";
            }
            return returnmane;
        }
        private class CustomerActionModel
        {
            /// <summary>
            /// 数量
            /// </summary>
            public int counts { get; set; }
            //总数
            public int allcounts { get; set; }
            //行为值
            public int action { get; set; }
            //行为名称
            public string actionname { get; set; }
            //百分比
            public double ratio { get; set; }
        }
        private class TrendAnalysisModel
        {
            //小时
            public string hourstr { get; set; }
            //数量
            public int counts { get; set; }
            //类型值
            public int typenumber { get; set; }
            //类型
            public string typestr { get; set; }
        }
        private class GetYesterdayEpitomeForRadarModel
        {

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