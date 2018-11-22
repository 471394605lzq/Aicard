using AiCard.Common;
using AiCard.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static AiCard.Common.Comm;

namespace AiCard.Bll
{
    /// <summary>
    /// author:lym
    /// date:2018-11-17
    /// 个人账户佣金及余额业务处理类
    /// </summary>
    public sealed class VIPAccountBLL
    {

        /// <summary>
        /// 计算会员收益佣金
        /// </summary>
        /// <param name="vUserID">注册或升级的用户ID</param>
        /// <param name="vBussType">业务类型 0 注册/新建名片  1 升级会员</param>
        public RequestResult CalculateVIPAmount(string vUserID,int vBussType) {
            #region
            RequestResult result = new RequestResult() {
                retCode=ReqResultCode.failed,
                retMsg="计算佣金失败"
            };
            if (string.IsNullOrWhiteSpace(vUserID)) {
                result.retMsg = "操作用户ID不能为空";
                return result;
            }
            try
            {
                decimal parentProfitAmount = 0;//上级用户佣金
                decimal GrandfatheredProfitAmount = 0;//上上级用户佣金
                Common.Enums.VipAmountLogType logType = 0;//日志记录类型
                int rows = 0;
                switch (vBussType) {
                    case 0:
                        parentProfitAmount = Comm.RegisterAmount();
                        logType = Common.Enums.VipAmountLogType.NewCard;
                        break;
                    case 1:
                        parentProfitAmount = Comm.UpGradeAmount() * Comm.ParentCommissionRate();
                        GrandfatheredProfitAmount = Comm.UpGradeAmount() * Comm.GrandfatheredCommissionRate();
                        logType = Common.Enums.VipAmountLogType.NewChild2nd;
                        break;
                    default:
                        break;
                }
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    VipRelationship vipship = db.VipRelationships.FirstOrDefault(p=>p.UserID==vUserID);
                    if (vipship != null)
                    {
                        Vip parentUser = db.Vips.FirstOrDefault(p=>p.ID == vipship .ParentID && p.UserID ==vipship .ParentUserID );
                        //上级用户
                        if (parentUser != null)
                        {
                            //佣金记录
                            VipAmountLog logModel = new VipAmountLog()
                            {
                                Amount = +parentProfitAmount,
                                Before = parentUser.Amount,
                                CreateDateTime =DateTime.Now ,
                                SourceUserID =vipship .UserID ,
                                Type = logType,
                                UserID = parentUser .UserID,
                                VipID =parentUser .ID 
                            };
                            db.VipAmountLogs.Add(logModel);
                            parentUser.TotalAmount += parentProfitAmount;
                            parentUser.Amount += parentProfitAmount;
                            if (logType == Common.Enums.VipAmountLogType.NewCard)
                            {
                                parentUser.FreeChildCount  += 1;
                            }
                            else if (logType == Common.Enums.VipAmountLogType.NewChild2nd)
                            {
                                parentUser.VipChild2ndCount += 1;
                            }

                            //上上级用户
                            vipship = db.VipRelationships.FirstOrDefault(p => p.UserID == parentUser.UserID );
                            if (vipship != null && vBussType==1)//升级会员才有
                            {
                                Vip grandfatherUser = db.Vips.FirstOrDefault(p => p.ID == vipship.ParentID && p.UserID == vipship.ParentUserID);
                                if (grandfatherUser != null)
                                {
                                    //佣金记录
                                    VipAmountLog logModel2 = new VipAmountLog()
                                    {
                                        Amount = +GrandfatheredProfitAmount,
                                        Before = grandfatherUser.Amount,
                                        CreateDateTime = DateTime.Now,
                                        SourceUserID = vipship.UserID,
                                        Type = Common.Enums.VipAmountLogType.NewChild3rd ,
                                        UserID = grandfatherUser.UserID,
                                        VipID = grandfatherUser.ID
                                    };
                                    db.VipAmountLogs.Add(logModel2);
                                    grandfatherUser.TotalAmount += GrandfatheredProfitAmount;
                                    grandfatherUser.Amount += GrandfatheredProfitAmount;
                                    grandfatherUser.VipChild3rdCount += 1;
                                }
                            }
                            rows = db.SaveChanges();
                            if (rows>0) {
                                result.retCode = ReqResultCode.success;
                                result.retMsg = "计算佣金成功";
                            }
                        }
                        else {
                            result.retCode = ReqResultCode.success;
                            result.retMsg = "上级用户不存在";
                        }
                    }
                    else {
                        result.retCode = ReqResultCode.success;
                        result.retMsg = "非邀请注册，无需计算佣金";
                    }
                }
            }
            catch (Exception ex)
            {
                result.retCode = ReqResultCode.excetion;
                result.retMsg = $"计算佣金时发生异常：{ex.Message}";
                //调试日志
                Comm.WriteLog("exception", result.retMsg, Common.Enums.DebugLogLevel.Error, "Bll.VIPAccountBLL.CalculateVIPAmount");
            }

            return result;
            #endregion
        }

    }
}