using AiCard.Commom.Aliyun.BankCard;
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
    /// author：lym
    /// date:2018-11-29
    /// vip会员提现账户业务层
    /// </summary>
    public sealed class VipForwardAccountBLL
    {
        /// <summary>
        /// 绑定提现银行卡
        /// </summary>
        /// <param name="model"></param>
        /// <param name="vUserID"></param>
        /// <returns></returns>
        public RequestResult CreateBankAccount(BankCardModel model,string vUserID) {
            #region
            RequestResult result = new RequestResult()
            {
                retCode = ReqResultCode.failed,
                retMsg = "绑定银行卡失败"
            };
            
            int rows = 0;
            try
            {
                #region 认证银行卡信息是否正确
                RequestResult ret =BankCardAuth.Bank4Authenticate(model);
                if (ret.retCode != ReqResultCode.success) {
                    result = ret;
                    return result;
                }
                #endregion
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    Vip vip = db.Vips.FirstOrDefault(p=>p.UserID==vUserID);
                    if (vip != null)
                    {
                        BankCardAuthResult data = ret.objectData;
                        VipForwardAccount account = new VipForwardAccount() {
                            Bank = data?.bankName,
                            BankCode=data?.bankCode,
                            CerCode = data?.idNo,
                            CreateDateTime=DateTime.Now,
                            ForwardAccount = data?.cardNo,
                            ForwardName = data?.name,
                            ForwardType =  Common.Enums.VipForwardType.BankCard,
                            PhoneNumber=data?.phoneNo,
                            UserID = vUserID,
                            VipID = vip.ID
                        };
                        db.VipForwardAccounts.Add(account);
                        rows = db.SaveChanges();
                        
                    }
                    else {
                        result.retMsg = "vip用户不存在";
                        return result;
                    }
                }
                if (rows > 0)
                {
                    result.retCode = ReqResultCode.success;
                    result.retMsg = "绑定成功";
                }
                else
                {
                    result.retMsg = "绑定失败";
                }

            }
            catch (Exception ex)
            {
                result.retCode = ReqResultCode.excetion;
                result.retMsg = $"绑定提现银行卡时发生异常：{ex.Message}";
                //调试日志
                Comm.WriteLog("exception", result.retMsg, Common.Enums.DebugLogLevel.Error, "Bll.VipForwardAccountBLL.CreateBankAccount");
            }
            
            return result;
            #endregion
        }


        /// <summary>
        /// 删除提现银行卡
        /// </summary>
        /// <param name="vBankAccountID"></param>
        /// <param name="vUserID"></param>
        /// <returns></returns>
        public RequestResult DeleteBankAccount(int vBankAccountID,string vUserID) {
            #region
            RequestResult result = new RequestResult()
            {
                retCode = ReqResultCode.failed,
                retMsg = "删除失败"
            };
            #region 校验数据
            if (string.IsNullOrWhiteSpace(vUserID))
            {
                result.retMsg = "操作用户ID不能为空";
                return result;
            }
            if (vBankAccountID <=0)
            {
                result.retMsg = "银行卡账户ID不能为空";
                return result;
            }
            
            #endregion
            int rows = 0;
            try
            {
                
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    VipForwardAccount account = db.VipForwardAccounts.FirstOrDefault(p => p.ID == vBankAccountID);
                    if (account != null)
                    {
                        if (account.UserID.Equals(vUserID))
                        {
                            db.VipForwardAccounts.Remove(account);
                            rows = db.SaveChanges();
                        }
                        else {
                            result.retMsg = "当前银行卡账户与当前账户不匹配";
                            return result;
                        }

                    }
                    else
                    {
                        result.retMsg = "当前银行卡账户不存在";
                        return result;
                    }
                }
                if (rows > 0)
                {
                    result.retCode = ReqResultCode.success;
                    result.retMsg = "删除成功";
                }
                else
                {
                    result.retMsg = "删除失败";
                }

            }
            catch (Exception ex)
            {
                result.retCode = ReqResultCode.excetion;
                result.retMsg = $"绑定提现银行卡时发生异常：{ex.Message}";
                //调试日志
                Comm.WriteLog("exception", result.retMsg, Common.Enums.DebugLogLevel.Error, "Bll.VipForwardAccountBLL.DeleteBankAccount");
            }

            return result;
            #endregion
        }
    }
}