using AiCard.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static AiCard.Common.Comm;

namespace AiCard.Bll
{
    /// <summary>
    /// author：lym
    /// date:2018-11-21
    /// vip会员业务层
    /// </summary>
    public sealed class VipBLL
    {
        VIPAccountBLL bll = new VIPAccountBLL();

        /// <summary>
        /// 创建vip用户及上下级关系
        /// </summary>
        /// <param name="vUserID"></param>
        /// <param name="inviteCode"></param>
        /// <returns></returns>
        public RequestResult CreateVipRelation(string vUserID, string inviteCode)
        {
            #region
            RequestResult result = new RequestResult()
            {
                retCode = ReqResultCode.failed,
                retMsg = "计算佣金失败"
            };
            #region 数据校验
            if (string.IsNullOrWhiteSpace(vUserID))
            {
                result.retMsg = "操作用户ID不能为空";
                return result;
            }
            #endregion
            int rows = 0;
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Vip uVip = db.Vips.FirstOrDefault(p => p.UserID == vUserID);
                    if (uVip == null)
                    {
                        CardPersonal card = db.CardPersonals.FirstOrDefault(p => p.UserID == vUserID);
                        if (card != null)
                        {
                            uVip = new Vip()
                            {
                                Amount = 0,
                                CardID = card.ID,
                                CreateDateTime = DateTime.Now,
                                FreeChildCount = 0,
                                State = Common.Enums.VipState.Enable,
                                TotalAmount = 0,
                                TotalMonthAmountRank = 0,
                                TotalAmountRank = 0,
                                TotalWeekAmountRank = 0,
                                UserID = vUserID,
                                Type = Common.Enums.VipRank.Default,
                                VipChild2ndCount = 0,
                                VipChild3rdCount = 0
                            };
                            db.Vips.Add(uVip);
                            rows = db.SaveChanges();
                            if (rows <= 0)
                            {
                                result.retMsg = "创建vip用户失败";
                                return result;
                            }
                        }
                        else
                        {
                            result.retMsg = "尚未创建该用户的名片";
                            return result;
                        }

                    }
                    //有邀请码则建立关系
                    if (!string.IsNullOrEmpty(inviteCode))
                    {
                        Vip inviteVip = db.Vips.FirstOrDefault(p => p.Code == inviteCode);
                        if (inviteVip != null)
                        {
                            VipRelationship relation = db.VipRelationships.FirstOrDefault(p => p.UserID == vUserID && p.ParentUserID == inviteVip.UserID);
                            if (relation == null)
                            {

                                relation = new VipRelationship()
                                {
                                    CreateDateTime = DateTime.Now,
                                    ParentID = inviteVip.ID,
                                    ParentUserID = inviteVip.UserID,
                                    VipID = uVip.ID,
                                    UserID = vUserID
                                };
                                db.VipRelationships.Add(relation);
                                rows = db.SaveChanges();
                                if (rows <= 0)
                                {
                                    result.retMsg = "创建vip用户与上级关系时失败";
                                    return result;
                                }

                            }
                        }
                    }

                }
                //计算佣金
                RequestResult ret = bll.CalculateVIPAmount(vUserID, 0);
                if (ret.retCode != ReqResultCode.success)
                {
                    result.retMsg = "vip用户关系建立成功，佣金计算失败";
                    return result;
                }
            }
            catch (Exception ex)
            {

                result.retMsg = $"创建vip用户关系时发生异常：{ex.Message}";
                return result;
            }

            result.retCode = ReqResultCode.success;
            result.retMsg = "操作成功";
            return result;
            #endregion
        }

        /// <summary>
        /// 随机生成6位邀请码（0~9 A~Z）
        /// </summary>
        /// <returns></returns>
        public static string RandomCode()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                string key = "0123456789ABCDEFGHIJKLNMOPQRSTUVWXYZ";
                string code = "";
                do
                {
                    for (int i = 0; i < 6; i++)
                    {
                        code += key[Common.Comm.Random.Next(0, key.Length - 1)];
                    }
                } while (db.Vips.Any(s => s.Code == code));//判断是否有重复
                return code;
            }
        }
    }
}