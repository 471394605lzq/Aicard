using System;
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
    }
}