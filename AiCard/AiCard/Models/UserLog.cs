using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    /// <summary>
    /// 用户行为记录
    /// </summary>
    public class UserLog
    {
        public int ID { get; set; }

        /// <summary>
        /// 行为用户ID
        /// </summary>
        public string UserID { get; set; }

        [Display(Name = "关联ID")]
        public int RelationID { get; set; }

        [Display(Name = "关联类别")]
        public Enums.UserLogType Type { get; set; }

        /// <summary>
        /// 存放Json
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 被操作用户ID
        /// </summary>
        [Display(Name = "对象用户")]
        public string TargetUserID { get; set; }

        [Display(Name = "对象企业")]
        public int? TargetEnterpriseID { get; set; }

        [Display(Name = "时间")]
        public DateTime CreateDateTime { get; set; }

        [Display(Name = "总次数")]
        public int Total { get; set; }
    }

    /// <summary>
    /// UserLog Remark内容
    /// </summary>
    public class UserLogRemarkComm
    {
        public int Count { get; set; }
    }
}