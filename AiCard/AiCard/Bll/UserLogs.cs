﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using AiCard.Models;

namespace AiCard.Bll
{
    public static class UserLogs
    {
        public static IPagedList<UserLogListViewModel> Search(int? relationID = null, string targetUserID = null, int? enterpriseID = null, Enums.UserLogType? type = null, int page = 1, int pageSize = 20)
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
                    case Enums.UserLogType.ArticleLike:
                    case Enums.UserLogType.ArticleComment:
                    case Enums.UserLogType.ArticleRead:
                    case Enums.UserLogType.ArticleShare:
                        {
                            var a = db.Articles.FirstOrDefault(s => s.ID == log.RelationID);
                            if (a == null || a.State != Enums.ArticleState.Released)
                            {
                                throw new Exception("文章不存在");
                            }
                            log.TargetEnterpriseID = a.EnterpriseID;
                            log.TargetUserID = a.UserID;
                        }
                        break;
                    case Enums.UserLogType.ProductRead:
                    case Enums.UserLogType.ProductCon:
                        {
                            var p = db.Products.FirstOrDefault(s => s.ID == log.RelationID);
                            if (p == null)
                            {
                                throw new Exception("商品不存在");
                            }
                            log.TargetEnterpriseID = p.EnterpriseID;
                        }
                        break;
                    case Enums.UserLogType.Communication:
                    case Enums.UserLogType.HomePageRead:
                    case Enums.UserLogType.ShopRead:
                    case Enums.UserLogType.WeChatOpen:
                    case Enums.UserLogType.CardRead:
                    case Enums.UserLogType.CardShare:
                    case Enums.UserLogType.CardSave:
                    case Enums.UserLogType.CardLike:
                    case Enums.UserLogType.PhoneCall:
                    case Enums.UserLogType.EmailSend:
                    case Enums.UserLogType.VoicePlay:
                    case Enums.UserLogType.VideoPlay:
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
                    case Enums.UserLogType.CardTab:
                        {
                            if (db.UserLogs.Any(s => s.Type == Enums.UserLogType.CardTab
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
                    default:
                        break;
                }
                if (log.Type == Enums.UserLogType.ArticleLike || log.Type == Enums.UserLogType.CardLike)
                {
                    var dbLog = db.UserLogs.FirstOrDefault(s => s.RelationID == log.RelationID
                                && s.UserID == log.UserID
                                && s.Type == log.Type);
                    if (dbLog == null)
                    {
                        db.UserLogs.Add(log);
                    }
                    else
                    {
                        db.UserLogs.Remove(dbLog);
                    }
                }
                else
                {
                    db.UserLogs.Add(log);

                }
                db.SaveChanges();//修改log
                switch (log.Type)
                {
                    case Enums.UserLogType.ArticleLike:
                    case Enums.UserLogType.ArticleShare:
                        {
                            var art = db.Articles.FirstOrDefault(s => s.ID == log.RelationID);
                            var count = db.UserLogs.Count(s => s.RelationID == log.RelationID && s.Type == log.Type);
                            if (log.Type == Enums.UserLogType.ArticleShare)
                            {
                                art.Share = count;
                            }
                            if (log.Type == Enums.UserLogType.ArticleLike)
                            {
                                art.Like = count;
                            }

                            db.SaveChanges();//更新点赞数量或分享数
                            break;
                        }
                    case Enums.UserLogType.CardLike:
                    case Enums.UserLogType.CardRead:
                    case Enums.UserLogType.CardShare:
                        {
                            var card = db.Cards.FirstOrDefault(s => s.ID == log.RelationID);
                            if (log.Type == Enums.UserLogType.CardRead)
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
                                if (log.Type == Enums.UserLogType.CardLike)
                                {
                                    card.View = count;
                                }
                            }

                            db.SaveChanges();//更新点赞数量或阅读数量
                        }
                        break;
                    case Enums.UserLogType.CardTab:
                        {
                            var t = db.CardTabs.FirstOrDefault(s => s.ID == log.RelationID);
                            t.Count = db.UserLogs.Count(s => s.RelationID == log.RelationID
                                && s.Type == Enums.UserLogType.CardTab);
                            db.SaveChanges();//更新卡片标签
                            break;
                        }
                    default:
                        break;
                }
            }
        }
    }
}