using AiCard.Commom.Aliyun.BankCard;
using AiCard.Common;
using AiCard.DAL.Models;
using AiCard.Models.VipForward;
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
        private const string singRand = "dtkj2018.com";
        /// <summary>
        /// 银行卡信息认证
        /// </summary>
        /// <param name="model"></param>
        /// <param name="vUserID"></param>
        /// <returns></returns>
        public RequestResult CheckBankInfo(BankCardModel model, string vUserID)
        {
            #region
            RequestResult result = new RequestResult()
            {
                retCode = ReqResultCode.failed,
                retMsg = "绑定银行卡失败"
            };

            string randStr = singRand;
            try
            {
                #region 认证银行卡信息是否正确
                RequestResult ret = BankCardAuth.Bank4Authenticate(model);
                if (ret.retCode != ReqResultCode.success)
                {
                    result = ret;
                    return result;
                }
                #endregion
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    //查看此银行卡是否已经绑定
                    VipForwardAccount account = db.VipForwardAccounts.FirstOrDefault(p => p.UserID == vUserID && p.CerCode == model.idNo && p.ForwardName == model.name && p.PhoneNumber == model.phoneNo && p.ForwardAccount == model.cardNo);
                    if (account != null)
                    {
                        result.retMsg = "该银行卡已经绑定，请重新填入";
                        return result;
                    }
                    Vip vip = db.Vips.FirstOrDefault(p => p.UserID == vUserID);
                    if (vip != null && !string.IsNullOrWhiteSpace(vip.Code))
                    {
                        randStr = vip.Code;
                    }
                }

                result.retCode = ReqResultCode.success;
                result.retMsg = "认证成功";
                result.objectData = new MReqBankAccount
                {
                    bankCode = ret.objectData.bankCode,
                    bankName = ret.objectData.bankName,
                    cardNo = model.cardNo,
                    idNo = model.idNo,
                    name = model.name,
                    phoneNo = model.phoneNo,
                    sign = Comm.GetMd5Hash($"{ret.objectData.bankCode}{ret.objectData.bankName}{model.cardNo}{model.idNo}{model.name}{model.phoneNo}{randStr}")
                };

            }
            catch (Exception ex)
            {
                result.retCode = ReqResultCode.excetion;
                result.retMsg = $"银行卡信息认证时发生异常：{ex.Message}";
                //调试日志
                Comm.WriteLog("exception", result.retMsg, Common.Enums.DebugLogLevel.Error, "Bll.VipForwardAccountBLL.CheckBankInfo");
            }

            return result;
            #endregion
        }


        /// <summary>
        /// 绑定提现银行卡
        /// </summary>
        /// <param name="model"></param>
        /// <param name="vUserID"></param>
        /// <returns></returns>
        public RequestResult CreateBankAccount(MReqBankAccount model, string vUserID, string phoneNumber, string validCode)
        {
            #region
            RequestResult result = new RequestResult()
            {
                retCode = ReqResultCode.failed,
                retMsg = "绑定银行卡失败"
            };

            int rows = 0;
            try
            {
                string randStr = singRand;

                using (ApplicationDbContext db = new ApplicationDbContext())
                {

                    #region 验证校验码是否正确
                    VerificationCode vCode = db.VerificationCodes.FirstOrDefault(p => p.Code == validCode && p.To == phoneNumber.Replace(" ", ""));
                    if (vCode == null)
                    {
                        result.retMsg = "手机验证码不正确";
                        return result;
                    }
                    else if (DateTime.Now > vCode.EndDateTime)
                    {
                        result.retMsg = "验证码已过期";
                        return result;
                    }
                    #endregion
                    Vip vip = db.Vips.FirstOrDefault(p => p.UserID == vUserID);
                    if (vip != null)
                    {
                        #region 验证签名是否正确
                        if (!string.IsNullOrWhiteSpace(vip.Code)) randStr = vip.Code;
                        string sign = Comm.GetMd5Hash($"{model.bankCode}{model.bankName}{model.cardNo}{model.idNo}{model.name}{model.phoneNo}{randStr}");
                        if (!sign.Equals(model.sign)) {
                            result.retMsg = "签名不正确，您的信息可能已被更改";
                            return result;
                        }
                        #endregion
                        int count= db.VipForwardAccounts.Where(p => p.UserID == vUserID).Count();
                        if (count>=5) {
                            result.retMsg = "提现银行卡最多只能绑定5张，如需添加新卡，请删除已绑定的银行卡";
                            return result;
                        }
                        VipForwardAccount account = new VipForwardAccount()
                        {
                            Bank = model.bankName,
                            BankCode = model.bankCode,
                            CerCode = model.idNo,
                            CreateDateTime = DateTime.Now,
                            ForwardAccount = model.cardNo,
                            ForwardName = model.name,
                            ForwardType = Common.Enums.VipForwardType.BankCard,
                            PhoneNumber = model.phoneNo,
                            UserID = vUserID,
                            VipID = vip.ID
                        };
                        db.VipForwardAccounts.Add(account);
                        rows = db.SaveChanges();

                    }
                    else
                    {
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
        public RequestResult DeleteBankAccount(int vBankAccountID, string vUserID)
        {
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
            if (vBankAccountID <= 0)
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
                        else
                        {
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