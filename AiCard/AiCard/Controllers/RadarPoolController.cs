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
                    memo = GetEnumsName(s.action),
                    ratio = s.ratio,
                    consts = "const"
                });
                var temp = resultdata.FirstOrDefault();
                var rdata = new
                {
                    allcounts = temp == null ? 0 : temp.allcounts,
                    data = resultdata
                };
                return Json(Comm.ToJsonResult("Success", "成功", rdata), JsonRequestBehavior.AllowGet);
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
            catch (Exception ex)
            {
                returnmane = "未知";
            }
            return returnmane;
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
                    sharepathname = GetCustLogEnumsName(s.action),
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
                            AND dateadd(day, -1, DATEADD(ms, -3, DATEADD(dd, DATEDIFF(dd, -1, getdate()), 0)))
                            GROUP BY [Type]";
                string sqlstr = string.Format(sql, @"@enterpriseid,@userid,@timenumber,@actionstr1,@actionstr2,@actionstr3");
                List<CustomerActionModel> data = db.Database.SqlQuery<CustomerActionModel>(sqlstr, parameters).ToList();
                var resultdata = data.Select(s => new
                {
                    counts = s.counts,
                    sharepage = s.action,
                    sharepaagename = GetCustLogEnumsName(s.action),
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
        public ActionResult GetShareNumberAnalysis(int enterpriseid, string userid, int timenumber)
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
                        new SqlParameter("@actionstr2", SqlDbType.Int)
                    };
                parameters[0].Value = enterpriseid;
                parameters[1].Value = userid;
                parameters[2].Value = timenumber;
                parameters[3].Value = Common.Enums.UserLogType.ShareWeChatFriend;
                parameters[4].Value = Common.Enums.UserLogType.ShareWeChatGroup;
                string sql = @" SELECT DATENAME(HOUR,CreateDateTime) AS hourstr,COUNT(ID) counts FROM dbo.UserLogs WHERE [Type] IN(@actionstr1,@actionstr2) 
                                AND TargetUserID=@userid AND TargetEnterpriseID=@enterpriseid AND 
                                CreateDateTime BETWEEN dateadd(day, -@timenumber, dateadd(ms, 0, DATEADD(dd, DATEDIFF(dd, 0, getdate()), 0)))AND dateadd(day, -1, DATEADD(ms, -3, DATEADD(dd, DATEDIFF(dd, -1, getdate()), 0)))
                                GROUP BY DATENAME(HOUR,CreateDateTime)";
                string sqlstr = string.Format(sql, @"@enterpriseid,@userid,@timenumber,@actionstr1,@actionstr2");
                List<TrendAnalysisModel> data = db.Database.SqlQuery<TrendAnalysisModel>(sqlstr, parameters).ToList();
                //var resultdata = data.Select(s => new
                //{
                //    counts = s.counts,
                //    hourstr = s.hourstr
                //    //typenumber=s.typenumber,
                //    //typestr = GetEnumsName(s.typenumber)
                //});
                return Json(Comm.ToJsonResult("Success", "成功", data), JsonRequestBehavior.AllowGet);
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
        public ActionResult GetCustomerGenderProportion(int enterpriseid, string userid)
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
                    gendername = GetCustGenderEnumsName(s.action),
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
        /// 年龄占比
        /// </summary>
        /// <param name="enterpriseid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult GetCustomerAgeProportion(int enterpriseid, string userid)
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
                        new SqlParameter("@userid", SqlDbType.NVarChar)
                };
                parameters[0].Value = enterpriseid;
                parameters[1].Value = userid;
                string sqlstr = string.Format(@"GetCustomerAgeProportion @enterpriseid,@userid");
                List<CustomerActionModel> data = db.Database.SqlQuery<CustomerActionModel>(sqlstr, parameters).ToList();
                var resultdata = data.Select(s => new
                {
                    counts = s.counts,
                    allcounts = s.allcounts,
                    agename = s.actionname,
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
        /// 客户区域占比
        /// </summary>
        /// <param name="enterpriseid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult GetCustomerAreaProportion(int enterpriseid, string userid)
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
                        new SqlParameter("@userid", SqlDbType.NVarChar)
                };
                parameters[0].Value = enterpriseid;
                parameters[1].Value = userid;
                string sqlstr = string.Format(@"GetCustomerAreaProportion @enterpriseid,@userid");
                List<CustomerActionModel> data = db.Database.SqlQuery<CustomerActionModel>(sqlstr, parameters).ToList();
                var resultdata = data.Select(s => new
                {
                    counts = s.counts,
                    allcounts = s.allcounts,
                    areaname = s.actionname,
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
        /// 客户兴趣分析
        /// </summary>
        /// <param name="enterpriseid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult GetCustomerAvocation(int custid)
        {
            try
            {
                if (!db.EnterpriseCustomers.Any(s => s.ID == custid))
                {
                    return Json(Comm.ToJsonResult("NoFound", "客户不存在"));
                }
                //拼接参数
                SqlParameter[] parameters = {
                        new SqlParameter("@custid", SqlDbType.Int),
                        new SqlParameter("@articleread", SqlDbType.Int),
                        new SqlParameter("@cardread", SqlDbType.Int),
                        new SqlParameter("@shopread", SqlDbType.Int),
                        new SqlParameter("@homepageread", SqlDbType.Int),
                };
                parameters[0].Value = custid;
                parameters[1].Value = Common.Enums.UserLogType.ArticleRead;
                parameters[2].Value = Common.Enums.UserLogType.CardRead;
                parameters[3].Value = Common.Enums.UserLogType.ShopRead;
                parameters[4].Value = Common.Enums.UserLogType.HomePageRead;
                string sqlstr = string.Format(@"GetCustomerAvocation @custid,@articleread,@cardread,@shopread,@homepageread");
                List<CustomerActionModel> data = db.Database.SqlQuery<CustomerActionModel>(sqlstr, parameters).ToList();
                var resultdata = data.Select(s => new
                {
                    counts = s.counts,
                    allcounts = s.allcounts,
                    avocation = s.action,
                    avocationname = GetCustLogEnumsName(s.action),
                    ratio = s.ratio
                });

                return Json(Comm.ToJsonResult("Success", "成功", resultdata), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        //获取客户行为枚举名称
        private string GetCustLogEnumsName(int val)
        {
            string returnmane = "";
            try
            {
                returnmane = ((Common.Enums.UserLogType)val).GetDisplayName();
            }
            catch (Exception ex)
            {
                returnmane = "未知";
            }
            return returnmane;
        }

        //获取客户性别枚举名称
        private string GetCustGenderEnumsName(int val)
        {
            string returnmane = "";
            try
            {
                returnmane = ((Common.Enums.Gender)val).GetDisplayName();
            }
            catch (Exception ex)
            {
                returnmane = "未知";
            }
            return returnmane;
        }



        /// <summary>
        /// 客户详情-客户活跃度
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult GetCustomerActivity(int custID, int timenumber)
        {
            try
            {
                if (!db.EnterpriseCustomers.Any(s => s.ID == custID))
                {
                    return Json(Comm.ToJsonResult("NoFound", "客户不存在"));
                }
                //拼接参数
                SqlParameter[] parameters = {
                        new SqlParameter("@custID", SqlDbType.Int),
                        new SqlParameter("@timenumber", SqlDbType.Int),
                    };
                parameters[0].Value = custID;
                parameters[1].Value = timenumber;

                string sqlstr = string.Format(@"SELECT DATENAME(HOUR,ul.CreateDateTime) AS hourstr,COUNT(ul.ID) counts FROM dbo.UserLogs ul
                                                    INNER JOIN dbo.EnterpriseCustomers c ON c.UserID = ul.UserID
                                                    WHERE c.ID=@custID AND ul.CreateDateTime BETWEEN dateadd(day, -@timenumber, dateadd(ms, 0, DATEADD(dd, DATEDIFF(dd, 0, getdate()), 0)))
                                                    AND dateadd(day, -1, DATEADD(ms, -3, DATEADD(dd, DATEDIFF(dd, -1, getdate()), 0)))
                                                    GROUP BY DATENAME(HOUR,CreateDateTime)");
                List<TrendAnalysisModel> data = db.Database.SqlQuery<TrendAnalysisModel>(sqlstr, parameters).ToList();
                return Json(Comm.ToJsonResult("Success", "成功", data), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 客户活跃度排行榜
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult GetCustomerActivityTop(int type, string userID)
        {
            try
            {
                if (!db.Users.Any(s => s.Id == userID))
                {
                    return Json(Comm.ToJsonResult("NoFound", "该用户不存在"));
                }
                //拼接参数
                SqlParameter[] parameters = {
                        new SqlParameter("@userid", SqlDbType.NVarChar),
                        new SqlParameter("@timetype", SqlDbType.Int),
                    };
                parameters[0].Value = userID;
                parameters[1].Value = type;
                string sqlstr = string.Format(@"GetCustomerActivityTop @userid,@timetype");
                List<CustActivityTopModel> data = db.Database.SqlQuery<CustActivityTopModel>(sqlstr, parameters).ToList();
                return Json(Comm.ToJsonResult("Success", "成功", data), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }



        /// <summary>
        /// Ai雷达 智能追踪
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult GetNoopsycheFollowList(Common.Enums.RankingsType? type, int enterpriseID, string userID, int? page = 1, int? pageSize = 2)
        {
            try
            {
                if (!db.Cards.Any(s => s.EnterpriseID == enterpriseID && s.UserID == userID))
                {
                    return Json(Comm.ToJsonResult("CardNoFound", "名片不存在"));
                }
                int starpagesize = page.Value * pageSize.Value - pageSize.Value;
                int endpagesize = page.Value * pageSize.Value;
                //拼接参数
                SqlParameter[] parameters = {
                        new SqlParameter("@TargetUserID", SqlDbType.NVarChar),
                        new SqlParameter("@TargetEnterpriseID", SqlDbType.Int),
                        new SqlParameter("@starpagesize", SqlDbType.Int),
                        new SqlParameter("@endpagesize", SqlDbType.Int)
                    };
                parameters[0].Value = userID;
                parameters[1].Value = enterpriseID;
                parameters[2].Value = starpagesize;
                parameters[3].Value = endpagesize;

                List< List <NoopsycheFollowShowModel> > returndatalist = new List<List<NoopsycheFollowShowModel>>();
                //全部
                if (type == Common.Enums.RankingsType.All)
                {
                    string sqlstr = string.Format(@"SELECT * FROM (SELECT CAST(ROW_NUMBER() over(order by CONVERT(CHAR(10),CreateDateTime,120) DESC) AS INTEGER) AS rownumber,
                     CASE WHEN CONVERT(CHAR(10),CreateDateTime,120)=CONVERT(CHAR(10),GETDATE(),120) THEN '今天' WHEN CONVERT(CHAR(10),CreateDateTime,120)=CONVERT(CHAR(10),dateadd(day,-1,getdate()),120) 
                     THEN '昨天' ELSE CONVERT(CHAR(10),CreateDateTime,120) END AS timestr,CONVERT(CHAR(10),CreateDateTime,120) AS datestr
                     FROM dbo.UserLogs WHERE Type IN(12,40,60,70) AND TargetUserID=@TargetUserID AND TargetEnterpriseID=@TargetEnterpriseID
                     GROUP BY CONVERT(CHAR(10),CreateDateTime,120)) t WHERE t.rownumber > @starpagesize AND t.rownumber<=@endpagesize");
                    List<NoopsycheFollowModel> data = db.Database.SqlQuery<NoopsycheFollowModel>(sqlstr, parameters).ToList();
                    for (int i = 0; i < data.Count; i++)
                    {
                        List<NoopsycheFollowShowModel> list = new List<NoopsycheFollowShowModel>();
                        string tempdatestr = data[i].datestr;
                        SqlParameter[] myparameters = {
                        new SqlParameter("@time", SqlDbType.NVarChar),
                        new SqlParameter("@TargetUserID", SqlDbType.NVarChar),
                        new SqlParameter("@TargetEnterpriseID", SqlDbType.Int)
                         };
                        myparameters[0].Value = tempdatestr;
                        myparameters[1].Value = userID;
                        myparameters[2].Value = enterpriseID;
                        string listsqlstr = string.Format(@"SELECT DISTINCT CASE WHEN euc.CreateDateTime BETWEEN dateadd(day, -0, dateadd(ms, 0, DATEADD(dd, DATEDIFF(dd, 0, getdate()), 0))) 
                        AND dateadd(day, -0, DATEADD(ms, -3, DATEADD(dd, DATEDIFF(dd, -1, getdate()), 0))) THEN '是' ELSE '否' END AS isnewcust,Type,Total,ec.RealName,us.Avatar,
                        CONVERT(NVARCHAR(50),DATEPART(hh,ul.CreateDateTime))+':'+CONVERT(NVARCHAR(50),DATEPART(mi,ul.CreateDateTime)) as createtimestr,ec.ID,us.UserName as custid 
                        FROM dbo.UserLogs ul
                        INNER JOIN dbo.EnterpriseCustomers ec ON ec.UserID=ul.UserID
                        INNER JOIN dbo.AspNetUsers us ON us.Id=ec.UserID
                        INNER JOIN dbo.EnterpriseUserCustomers euc ON euc.CustomerID=ec.ID
                        WHERE Type IN(12,40,60,70) AND TargetUserID=@TargetUserID AND  TargetEnterpriseID=@TargetEnterpriseID AND
                        ul.CreateDateTime BETWEEN dateadd(day, -0, dateadd(ms, 0, DATEADD(dd, DATEDIFF(dd, 0, @time), 0))) 
                        AND dateadd(day, -0, DATEADD(ms, -3, DATEADD(dd, DATEDIFF(dd, -1, @time), 0))) AND ul.Total=(SELECT MAX(Total) FROM dbo.UserLogs WHERE UserID=ec.UserID AND 
						Type IN(12,40,60,70) AND TargetUserID=@TargetUserID AND  TargetEnterpriseID=@TargetEnterpriseID AND
                        ul.CreateDateTime BETWEEN dateadd(day, -0, dateadd(ms, 0, DATEADD(dd, DATEDIFF(dd, 0, @time), 0))) 
                        AND dateadd(day, -0, DATEADD(ms, -3, DATEADD(dd, DATEDIFF(dd, -1, @time), 0))) )");

                        List<NoopsycheFollowShowModel> mydata = db.Database.SqlQuery<NoopsycheFollowShowModel>(listsqlstr, myparameters).ToList();
                        for (int j = 0; j < mydata.Count; j++)
                        {
                            NoopsycheFollowShowModel model = new NoopsycheFollowShowModel();
                            model.Avatar = mydata[j].Avatar;
                            model.createtimestr = mydata[j].createtimestr;
                            model.isnewcust = mydata[j].isnewcust;
                            model.RealName = mydata[j].RealName;
                            model.Total = mydata[j].Total;
                            model.showstr = ((Common.Enums.NoopsycheFollowType)mydata[j].Type).GetDisplayName();
                            model.showremarkstr = ((Common.Enums.NoopsycheFollowType)mydata[j].Type + 1).GetDisplayName();
                            model.TitleTime = data[i].timestr;
                            model.ID = mydata[j].ID;
                            model.custid = mydata[j].custid;
                            list.Add(model);
                        }
                        returndatalist.Add(list);
                    }
                    var resultdata = new
                    {
                        listdata = returndatalist
                    };
                    return Json(Comm.ToJsonResult("Success", "成功", resultdata), JsonRequestBehavior.AllowGet);
                }
                //新客户
                else
                {
                    List<NoopsycheFollowShowModel> returnlist = new List<NoopsycheFollowShowModel>();
                    string sqlstr = string.Format(@"SELECT  '是' AS isnewcust,Type,	Total,ec.RealName,us.Avatar,CONVERT(NVARCHAR(50),DATEPART(hh,ul.CreateDateTime))+':'+CONVERT(NVARCHAR(50),DATEPART(mi,ul.CreateDateTime)) as createtimestr,ec.ID ,us.UserName as custid 
                        FROM dbo.UserLogs ul
                        INNER JOIN dbo.EnterpriseCustomers ec ON ec.UserID=ul.UserID
                        INNER JOIN dbo.AspNetUsers us ON us.Id=ec.UserID
                        INNER JOIN dbo.EnterpriseUserCustomers euc ON euc.CustomerID=ec.ID
                        WHERE Type IN(12,40,60,70) AND TargetUserID=@TargetUserID AND  TargetEnterpriseID=@TargetEnterpriseID 
						 AND ul.Total=(SELECT MAX(Total) FROM dbo.UserLogs WHERE UserID=ec.UserID AND 
						Type IN(12,40,60,70) AND TargetUserID=@TargetUserID AND  TargetEnterpriseID=@TargetEnterpriseID ) 
						AND euc.CreateDateTime BETWEEN dateadd(day, -0, dateadd(ms, 0, DATEADD(dd, DATEDIFF(dd, 0, getdate()), 0))) 
                        AND dateadd(day, -0, DATEADD(ms, -3, DATEADD(dd, DATEDIFF(dd, -1, getdate()), 0)))");
                    List<NoopsycheFollowShowModel> data = db.Database.SqlQuery<NoopsycheFollowShowModel>(sqlstr, parameters).ToList();
                    for (int j = 0; j < data.Count; j++)
                    {
                        NoopsycheFollowShowModel model = new NoopsycheFollowShowModel();
                        model.Avatar = data[j].Avatar;
                        model.createtimestr = data[j].createtimestr;
                        model.isnewcust = data[j].isnewcust;
                        model.RealName = data[j].RealName;
                        model.Total = data[j].Total;
                        model.showstr =  ((Common.Enums.NoopsycheFollowType)data[j].Type).GetDisplayName();
                        model.showremarkstr = ((Common.Enums.NoopsycheFollowType)data[j].Type + 1).GetDisplayName();
                        model.TitleTime = "无";
                        model.ID = data[j].ID;
                        model.custid = data[j].custid;
                        returnlist.Add(model);
                    }
                    var resultdata = new
                    {
                        listdata = returnlist
                    };
                    return Json(Comm.ToJsonResult("Success", "成功", resultdata), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Ai雷达 客户详情智能追踪
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult GetCustomerNoopsycheFollowList(int enterpriseID, int custID, string owerID, int? page = 1, int? pageSize = 2)
        {
            try
            {
                if (!db.EnterpriseUserCustomer.Any(s => s.CustomerID == custID && s.OwnerID == owerID))
                {
                    return Json(Comm.ToJsonResult("NoFound", "客户不存在"));
                }
                int starpagesize = page.Value * pageSize.Value - pageSize.Value;
                int endpagesize = page.Value * pageSize.Value;
                //拼接参数
                SqlParameter[] parameters = {
                        new SqlParameter("@CustomerID", SqlDbType.Int),
                        new SqlParameter("@TargetUserID", SqlDbType.NVarChar),
                        new SqlParameter("@TargetEnterpriseID", SqlDbType.Int),
                        new SqlParameter("@starpagesize", SqlDbType.Int),
                        new SqlParameter("@endpagesize", SqlDbType.Int)
                    };
                parameters[0].Value = custID;
                parameters[1].Value = owerID;
                parameters[2].Value = enterpriseID;
                parameters[3].Value = starpagesize;
                parameters[4].Value = endpagesize;

                List<List<NoopsycheFollowShowModel>> returndatalist = new List<List<NoopsycheFollowShowModel>>();
                    string sqlstr = string.Format(@"SELECT * FROM (SELECT CAST(ROW_NUMBER() over(order by CONVERT(CHAR(10),CreateDateTime,120) DESC) AS INTEGER) AS rownumber,
                     CASE WHEN CONVERT(CHAR(10),CreateDateTime,120)=CONVERT(CHAR(10),GETDATE(),120) THEN '今天' WHEN CONVERT(CHAR(10),CreateDateTime,120)=CONVERT(CHAR(10),dateadd(day,-1,getdate()),120) 
                     THEN '昨天' ELSE CONVERT(CHAR(10),CreateDateTime,120) END AS timestr,CONVERT(CHAR(10),CreateDateTime,120) AS datestr
                     FROM dbo.UserLogs INNER JOIN dbo.EnterpriseCustomers ec ON ec.UserID=UserLogs.UserID WHERE Type IN(12,40,60,70) AND TargetUserID=@TargetUserID AND ec.ID=@CustomerID AND  TargetEnterpriseID=@TargetEnterpriseID
                     GROUP BY CONVERT(CHAR(10),CreateDateTime,120)) t WHERE t.rownumber > @starpagesize AND t.rownumber<=@endpagesize");
                    List<NoopsycheFollowModel> data = db.Database.SqlQuery<NoopsycheFollowModel>(sqlstr, parameters).ToList();
                    for (int i = 0; i < data.Count; i++)
                    {
                        List<NoopsycheFollowShowModel> list = new List<NoopsycheFollowShowModel>();
                        string tempdatestr = data[i].datestr;
                        SqlParameter[] myparameters = {
                        new SqlParameter("@time", SqlDbType.NVarChar),
                        new SqlParameter("@TargetUserID", SqlDbType.NVarChar),
                        new SqlParameter("@TargetEnterpriseID", SqlDbType.Int),
                        new SqlParameter("@CustomerID", SqlDbType.Int)
                         };
                        myparameters[0].Value = tempdatestr;
                        myparameters[1].Value = owerID;
                        myparameters[2].Value = enterpriseID;
                        myparameters[3].Value = custID;
                    string listsqlstr = string.Format(@"SELECT Type,Total,
                        CONVERT(NVARCHAR(50),DATEPART(hh,ul.CreateDateTime))+':'+CONVERT(NVARCHAR(50),DATEPART(mi,ul.CreateDateTime)) as createtimestr,ec.ID 
                        FROM dbo.UserLogs ul
                        INNER JOIN dbo.EnterpriseCustomers ec ON ec.UserID=ul.UserID
                        WHERE Type IN(12,40,60,70) AND TargetUserID=@TargetUserID AND  TargetEnterpriseID=@TargetEnterpriseID AND ec.ID=@CustomerID AND
                        ul.CreateDateTime BETWEEN dateadd(day, -0, dateadd(ms, 0, DATEADD(dd, DATEDIFF(dd, 0, @time), 0))) 
                        AND dateadd(day, -0, DATEADD(ms, -3, DATEADD(dd, DATEDIFF(dd, -1, @time), 0))) AND ul.Total=(SELECT MAX(Total) FROM dbo.UserLogs WHERE UserID=ec.UserID AND 
						Type IN(12,40,60,70) AND TargetUserID=@TargetUserID AND  TargetEnterpriseID=@TargetEnterpriseID AND ec.ID=@CustomerID AND
                        ul.CreateDateTime BETWEEN dateadd(day, -0, dateadd(ms, 0, DATEADD(dd, DATEDIFF(dd, 0, @time), 0))) 
                        AND dateadd(day, -0, DATEADD(ms, -3, DATEADD(dd, DATEDIFF(dd, -1, @time), 0))) )");
                        List<NoopsycheFollowShowModel> mydata = db.Database.SqlQuery<NoopsycheFollowShowModel>(listsqlstr, myparameters).ToList();
                        for (int j = 0; j < mydata.Count; j++)
                        {
                            NoopsycheFollowShowModel model = new NoopsycheFollowShowModel();
                            model.createtimestr = mydata[j].createtimestr;
                            model.Total = mydata[j].Total;
                            model.showstr = ((Common.Enums.NoopsycheFollowType)mydata[j].Type).GetDisplayName();
                            model.showremarkstr = ((Common.Enums.NoopsycheFollowType)mydata[j].Type + 1).GetDisplayName();
                            model.TitleTime = data[i].timestr;
                            model.ID = mydata[j].ID;
                            list.Add(model);
                        }
                    returndatalist.Add(list);
                }
                    var resultdata = new
                    {
                        listdata = returndatalist
                    };
                    return Json(Comm.ToJsonResult("Success", "成功", resultdata), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Ai雷达 客户详情跟进记录
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult GetCustomerFollowRecordList(int enterpriseID, int custID, string owerID, int? page = 1, int? pageSize = 2)
        {
            try
            {
                if (!db.EnterpriseUserCustomer.Any(s => s.CustomerID == custID && s.OwnerID == owerID))
                {
                    return Json(Comm.ToJsonResult("NoFound", "客户不存在"));
                }
                int starpagesize = page.Value * pageSize.Value - pageSize.Value;
                int endpagesize = page.Value * pageSize.Value;
                //拼接参数
                SqlParameter[] parameters = {
                        new SqlParameter("@CustomerID", SqlDbType.Int),
                        new SqlParameter("@TargetUserID", SqlDbType.NVarChar),
                        new SqlParameter("@TargetEnterpriseID", SqlDbType.Int),
                        new SqlParameter("@starpagesize", SqlDbType.Int),
                        new SqlParameter("@endpagesize", SqlDbType.Int)
                    };
                parameters[0].Value = custID;
                parameters[1].Value = owerID;
                parameters[2].Value = enterpriseID;
                parameters[3].Value = starpagesize;
                parameters[4].Value = endpagesize;

                List<List<NoopsycheFollowShowModel>> returndatalist = new List<List<NoopsycheFollowShowModel>>();
                string sqlstr = string.Format(@"SELECT * FROM (SELECT CAST(ROW_NUMBER() over(order by CONVERT(CHAR(10),CreateDateTime,120) DESC) AS INTEGER) AS rownumber,
                     CASE WHEN CONVERT(CHAR(10),CreateDateTime,120)=CONVERT(CHAR(10),GETDATE(),120) THEN '今天' WHEN CONVERT(CHAR(10),CreateDateTime,120)=CONVERT(CHAR(10),dateadd(day,-1,getdate()),120) 
                     THEN '昨天' ELSE CONVERT(CHAR(10),CreateDateTime,120) END AS timestr,CONVERT(CHAR(10),CreateDateTime,120) AS datestr
                     FROM dbo.UserLogs INNER JOIN dbo.EnterpriseCustomers ec ON ec.ID=UserLogs.RelationID  WHERE Type IN(101,102,20) AND UserLogs.UserID=@TargetUserID AND ec.ID=@CustomerID AND  TargetEnterpriseID=@TargetEnterpriseID
                     GROUP BY CONVERT(CHAR(10),CreateDateTime,120)) t WHERE t.rownumber > @starpagesize AND t.rownumber<=@endpagesize");
                List<NoopsycheFollowModel> data = db.Database.SqlQuery<NoopsycheFollowModel>(sqlstr, parameters).ToList();
                for (int i = 0; i < data.Count; i++)
                {
                    List<NoopsycheFollowShowModel> list = new List<NoopsycheFollowShowModel>();
                    string tempdatestr = data[i].datestr;
                    SqlParameter[] myparameters = {
                        new SqlParameter("@time", SqlDbType.NVarChar),
                        new SqlParameter("@TargetUserID", SqlDbType.NVarChar),
                        new SqlParameter("@TargetEnterpriseID", SqlDbType.Int),
                        new SqlParameter("@CustomerID", SqlDbType.Int)
                         };
                    myparameters[0].Value = tempdatestr;
                    myparameters[1].Value = owerID;
                    myparameters[2].Value = enterpriseID;
                    myparameters[3].Value = custID;
                    string listsqlstr = string.Format(@"SELECT Type,0 as Total,ul.Remark,
                        CONVERT(NVARCHAR(50),DATEPART(hh,ul.CreateDateTime))+':'+CONVERT(NVARCHAR(50),DATEPART(mi,ul.CreateDateTime)) as createtimestr,ec.ID 
                        FROM dbo.UserLogs ul
                        INNER JOIN dbo.EnterpriseCustomers ec ON ec.ID=ul.RelationID
                        WHERE Type IN(101,102) AND ul.UserID=@TargetUserID AND  TargetEnterpriseID=@TargetEnterpriseID AND ec.ID=@CustomerID AND
                        ul.CreateDateTime BETWEEN dateadd(day, -0, dateadd(ms, 0, DATEADD(dd, DATEDIFF(dd, 0, @time), 0))) 
                        AND dateadd(day, -0, DATEADD(ms, -3, DATEADD(dd, DATEDIFF(dd, -1, @time), 0)))
                        UNION ALL 
						SELECT Type,Total,ul.Remark,
                        CONVERT(NVARCHAR(50),DATEPART(hh,ul.CreateDateTime))+':'+CONVERT(NVARCHAR(50),DATEPART(mi,ul.CreateDateTime)) as createtimestr,ec.ID 
                        FROM dbo.UserLogs ul
                        INNER JOIN dbo.EnterpriseCustomers ec ON ec.ID=ul.RelationID
                        WHERE Type IN(20) AND ul.UserID=@TargetUserID AND  TargetEnterpriseID=@TargetEnterpriseID AND ec.ID=@CustomerID AND
                        ul.CreateDateTime BETWEEN dateadd(day, -0, dateadd(ms, 0, DATEADD(dd, DATEDIFF(dd, 0, @time), 0))) 
                        AND dateadd(day, -0, DATEADD(ms, -3, DATEADD(dd, DATEDIFF(dd, -1, @time), 0)))
						AND ul.Total=(SELECT MAX(Total) FROM dbo.UserLogs WHERE UserID=ec.UserID AND 
						Type IN(20) AND ul.UserID=@TargetUserID AND  TargetEnterpriseID=@TargetEnterpriseID AND ec.ID=@CustomerID AND
                        ul.CreateDateTime BETWEEN dateadd(day, -0, dateadd(ms, 0, DATEADD(dd, DATEDIFF(dd, 0, @time), 0))) 
                        AND dateadd(day, -0, DATEADD(ms, -3, DATEADD(dd, DATEDIFF(dd, -1, @time), 0))))");
                    List<NoopsycheFollowShowModel> mydata = db.Database.SqlQuery<NoopsycheFollowShowModel>(listsqlstr, myparameters).ToList();
                    for (int j = 0; j < mydata.Count; j++)
                    {
                        NoopsycheFollowShowModel model = new NoopsycheFollowShowModel();
                        model.Type = mydata[j].Type == Common.Enums.UserLogType.AddCustTab.GetHashCode() ? 102 : mydata[j].Type == Common.Enums.UserLogType.FollowUp.GetHashCode() ? 101 : 20;
                        model.createtimestr = mydata[j].createtimestr;
                        model.Total = mydata[j].Total;
                        model.showstr = mydata[j].Type == Common.Enums.UserLogType.Communication.GetHashCode() ? ((Common.Enums.NoopsycheFollowType)mydata[j].Type).GetDisplayName() : "";
                        model.showremarkstr =mydata[j].Remark;
                        model.TitleTime = data[i].timestr;
                        model.ID = mydata[j].ID;
                        list.Add(model);
                    }
                    returndatalist.Add(list);
                }
                var resultdata = new
                {
                    listdata = returndatalist
                };
                return Json(Comm.ToJsonResult("Success", "成功", resultdata), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", ex.Message), JsonRequestBehavior.AllowGet);
            }
        }


        private class NoopsycheFollowModel
        {
            public int rownumber { get; set; }
            public string timestr { get; set; }
            public string datestr { get; set; }
        }
        private class NoopsycheFollowShowModel
        {
            //是否是新客户
            public string isnewcust { get; set; }
            //类型
            public int Type { get; set; }
            //总数
            public int Total { get; set; }
            //客户姓名
            public string RealName { get; set; }
            //头像
            public string Avatar { get; set; }
            //时间(小时:分钟)
            public string createtimestr { get; set; }
            //显示第几次行为说明
            public string showstr { get; set; }
            //提示说明
            public string showremarkstr { get; set; }
            //时间标题
            public string TitleTime { get; set; }
            public int ID { get; set; }
            public string Remark { get; set; }
            public string custid{get;set;}
        }
        private class CustActivityTopModel
        {
            public int Ornumber { get; set; }
            public string Name { get; set; }
            public string Avatar { get; set; }
            public int Counts { get; set; }
            public int ID { get; set; }
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