using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using AiCard.Models;
using AiCard.Common.Enums;
using AiCard.DAL.Models;

namespace AiCard.Bll
{
    public static class UserLogs
    {
        public static IPagedList<UserLogListViewModel> Search(int? relationID = null, string targetUserID = null, int? enterpriseID = null, UserLogType? type = null, int page = 1, int pageSize = 20)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (string.IsNullOrWhiteSpace(targetUserID) && !enterpriseID.HasValue && !relationID.HasValue)
                {
                    throw new Exception("RelationID、TargetUserID、EnterpriseID不能同时为空");
                }
                else if (relationID.HasValue && !type.HasValue)
                {
                    throw new Exception("使用RelationID必须同时使用Type");
                }
                var logs = from l in db.UserLogs
                           from u in db.Users
                           where l.UserID == u.Id
                           select new UserLogListViewModel
                           {
                               UserAvatar = u.Avatar,
                               UserNickName = u.NickName,
                               UserID = l.UserID,
                               CreateDateTime = l.CreateDateTime,
                               Type = l.Type,
                               Remark = l.Remark,
                               TargetEnterpriseID = l.TargetEnterpriseID,
                               TargetUserID = l.TargetUserID,
                               RelationID = l.RelationID,
                               ID = l.ID,
                           };
                if (relationID.HasValue)
                {
                    logs = logs.Where(s => s.RelationID == relationID.Value);
                }
                if (!string.IsNullOrWhiteSpace(targetUserID))
                {
                    logs = logs.Where(s => s.TargetUserID == targetUserID);
                }
                if (enterpriseID.HasValue)
                {
                    logs = logs.Where(s => s.TargetEnterpriseID == enterpriseID.Value);
                }
                if (type.HasValue)
                {
                    logs = logs.Where(s => s.Type == type.Value);
                }
                var paged = logs.OrderByDescending(s => s.CreateDateTime).ToPagedList(page, pageSize);

                return paged;
            }


        }

        /// <summary>
        /// 添加记录
        /// </summary>
        /// <param name="log"></param>
        public static void Add(UserLog log)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                if (!db.Users.Any(s => s.Id == log.UserID))
                {
                    throw new Exception("用户不存在");
                }
                log.CreateDateTime = DateTime.Now;
                ///验证
                switch (log.Type)
                {
                    case UserLogType.ArticleLike:
                    case UserLogType.ArticleComment:
                    case UserLogType.ArticleRead:
                    case UserLogType.ArticleShare:
                        {
                            var a = db.Articles.FirstOrDefault(s => s.ID == log.RelationID);
                            if (a == null || a.State != ArticleState.Released)
                            {
                                throw new Exception("文章不存在");
                            }
                            log.TargetEnterpriseID = a.EnterpriseID;
                            log.TargetUserID = a.UserID;
                        }
                        break;
                    case UserLogType.ProductRead:
                    case UserLogType.ProductCon:
                        {
                            var p = db.Products.FirstOrDefault(s => s.ID == log.RelationID);
                            if (p == null)
                            {
                                throw new Exception("商品不存在");
                            }
                            log.TargetEnterpriseID = p.EnterpriseID;
                        }
                        break;
                    case UserLogType.HomePageRead:
                    case UserLogType.ShopRead:
                    case UserLogType.CardRead:
                    case UserLogType.Communication:
                    case UserLogType.WeChatOpen:
                    case UserLogType.CardShare:
                    case UserLogType.CardSave:
                    case UserLogType.CardLike:
                    case UserLogType.PhoneCall:
                    case UserLogType.EmailSend:
                    case UserLogType.VoicePlay:
                    case UserLogType.VideoPlay:
                        {

                            var c = db.Cards.FirstOrDefault(s => s.ID == log.RelationID);

                            if (c == null || !c.Enable)
                            {
                                throw new Exception("卡片不存在");
                            }
                            log.TargetEnterpriseID = c.EnterpriseID;
                            log.TargetUserID = c.UserID;
                        }
                        break;
                    case UserLogType.CardTab:
                        {
                            if (db.UserLogs.Any(s => s.Type == UserLogType.CardTab
                               && s.RelationID == log.RelationID
                               && s.UserID == log.UserID))
                            {
                                throw new Exception("该用户已经点击过此名片标签");
                            }
                            var t = db.CardTabs.FirstOrDefault(s => s.ID == log.RelationID);
                            if (t == null)
                            {
                                throw new Exception("卡片标签不存在");
                            }
                            var c = db.Cards.FirstOrDefault(s => s.ID == t.CardID);
                            log.TargetEnterpriseID = c.EnterpriseID;
                            log.TargetUserID = c.UserID;
                            break;
                        }
                    case UserLogType.CardPersonalAddressNav:
                    case UserLogType.CardPersonalEmailCopy:
                    case UserLogType.CardPersonalEnterpriseCopy:
                    case UserLogType.CardPersonalLike:
                    case UserLogType.CardPersonalMobileCall:
                    case UserLogType.CardPersonalPhoneCall:
                    case UserLogType.CardPersonalRead:
                    case UserLogType.CardPersonalSave:
                    case UserLogType.CardPersonalShare:
                    case UserLogType.CardPersonalWechat:
                        {
                            var card = db.CardPersonals.FirstOrDefault(s => s.ID == log.RelationID && s.Enable);
                            if (card == null)
                            {
                                throw new Exception("个人卡片不存在");
                            }
                            log.TargetUserID = card.UserID;
                            break;
                        }
                    default:
                        break;
                }
                //点赞处理
                if (log.Type == UserLogType.ArticleLike || log.Type == UserLogType.CardLike || log.Type == UserLogType.CardPersonalLike)
                {
                    var dbLog = db.UserLogs.FirstOrDefault(s => s.RelationID == log.RelationID
                                && s.UserID == log.UserID
                                && s.Type == log.Type);
                    if (dbLog == null)
                    {
                        log.Total = db.UserLogs.Count(s => s.RelationID == log.RelationID && s.Type == log.Type && s.UserID == log.UserID);
                        db.UserLogs.Add(log);
                    }
                    else
                    {
                        db.UserLogs.Remove(dbLog);
                    }
                }
                else
                {
                    int tempcount = db.UserLogs.Count(s => s.RelationID == log.RelationID && s.Type == log.Type && s.UserID == log.UserID);
                    log.Total = tempcount == 0 ? 1 : tempcount + 1;
                    db.UserLogs.Add(log);
                }
                db.SaveChanges();//修改log
                switch (log.Type)
                {
                    case UserLogType.ArticleLike:
                    case UserLogType.ArticleShare:
                        {
                            var art = db.Articles.FirstOrDefault(s => s.ID == log.RelationID);
                            var count = db.UserLogs.Count(s => s.RelationID == log.RelationID && s.Type == log.Type);
                            if (log.Type == UserLogType.ArticleShare)
                            {
                                art.Share = count;
                            }
                            if (log.Type == UserLogType.ArticleLike)
                            {
                                art.Like = count;
                            }
                            db.SaveChanges();//更新点赞数量或分享数
                            break;
                        }
                    case UserLogType.CardLike:
                    case UserLogType.CardRead:
                    case UserLogType.CardShare:
                        {
                            var card = db.Cards.FirstOrDefault(s => s.ID == log.RelationID);
                            if (log.Type == UserLogType.CardRead)
                            {
                                //算人头
                                card.Like = db.UserLogs
                                   .Where(s => s.Type == log.Type && s.RelationID == log.RelationID)
                                   .GroupBy(s => s.UserID)
                                   .Count();

                            }
                            else
                            {
                                var count = db.UserLogs
                                    .Count(s => s.RelationID == log.RelationID
                                            && s.Type == log.Type);
                                if (log.Type == UserLogType.CardLike)
                                {
                                    card.View = count;
                                }
                            }

                            db.SaveChanges();//更新点赞数量或阅读数量
                        }
                        break;
                    case UserLogType.CardTab:
                        {
                            var t = db.CardTabs.FirstOrDefault(s => s.ID == log.RelationID);
                            t.Count = db.UserLogs.Count(s => s.RelationID == log.RelationID
                                && s.Type == UserLogType.CardTab);
                            db.SaveChanges();//更新卡片标签
                            break;
                        }
                    case UserLogType.CardPersonalLike:
                    case UserLogType.CardPersonalRead:
                        {
                            var c = db.CardPersonals.FirstOrDefault(s => s.ID == log.RelationID);
                            var count = db.UserLogs.Count(s => s.RelationID == log.RelationID
                                    && s.Type == UserLogType.CardPersonalLike);
                            if (log.Type == UserLogType.CardPersonalLike)
                            {
                                c.Like = count;
                            }
                            else if (log.Type == UserLogType.CardPersonalRead)
                            {
                                c.View = count;
                            }
                            db.SaveChanges();
                            break;
                        }
                    default:
                        break;
                }


            }
            Common.WeChat.IConfig config = null;
            if ((int)log.Type < 200)
            {
                config = new Common.WeChat.ConfigMini();
            }
            else if ((int)log.Type >= 200)
            {
                config = new Common.WeChat.ConfigMiniPersonal();
            }
            if (config != null)
            {
                NotifyByLog(log, config);
            }
         
        }
        ///消息推送
        public static void NotifyByLog(UserLog log, Common.WeChat.IConfig config)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var targetUser = db.Users.FirstOrDefault(s => s.Id == log.TargetUserID);
                var user = db.Users.FirstOrDefault(s => s.Id == log.UserID);
                var wechat = new Common.WeChat.WeChatMinApi(config);
                if (targetUser == null)
                {
                    throw new Exception("推送用户不存在");
                }
                var userOpenID = new Bll.Users.UserOpenID(targetUser);
                string openID = userOpenID.SearchOpenID(config.AppID);
                var form = db.WeChatMiniNotifyForms
                    .FirstOrDefault(s => s.UserID == log.TargetUserID
                        && s.EndDateTime > DateTime.Now);
                var fromUser = db.Users.FirstOrDefault(s => s.Id == log.UserID);
                if (form == null)
                {
                    return;
                }
                Common.WeChat.WeChatMessageTemp.IWeChatMessageTemp iTempMessage;
                try
                {
                    switch (log.Type)
                    {
                        case UserLogType.CardPersonalRead:
                            {
                                iTempMessage = new Common.WeChat.WeChatMessageTemp.NewUserNotifyWeChatMessage(fromUser.NickName, log.CreateDateTime);
                                wechat.SendMessage(openID, form.FormID, null, iTempMessage);
                            }
                            break;
                        case UserLogType.CardPersonalAddressNav:
                        case UserLogType.CardPersonalEmailCopy:
                        case UserLogType.CardPersonalEnterpriseCopy:
                        case UserLogType.CardPersonalMobileCall:
                        case UserLogType.CardPersonalLike:
                        case UserLogType.CardPersonalPhoneCall:
                        case UserLogType.CardPersonalSave:
                        case UserLogType.CardPersonalShare:
                        case UserLogType.CardPersonalWechat:
                            {
                                iTempMessage = new Common.WeChat.WeChatMessageTemp.DefaultNotifyWeChatMessage(fromUser.NickName, $"{fromUser.NickName}{log.Type.GetDisplayName()}", log.CreateDateTime);
                                wechat.SendMessage(openID, form.FormID, null, iTempMessage);
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    db.WeChatMiniNotifyForms.Remove(form);
                    db.SaveChanges();
                }

            }

        }
    }
}