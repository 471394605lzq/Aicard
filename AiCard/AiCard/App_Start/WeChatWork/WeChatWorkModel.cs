using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.WeChatWork.Models
{
    public class Department
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int ParentID { get; set; }

        public int Order { get; set; }

        public List<Department> Child { get; set; } = new List<Department>();

    }

    public class User
    {
        public string ID { get; set; }

        [Display(Name="姓名")]
        public string Name { get; set; }

        //岗位
        public string Position { get; set; }

        //手机号
        public string Mobile { get; set; }

        //性别
        public Enums.Gender Gender { get; set; }

        //邮箱
        public string Email { get; set; }
        //头像
        public string Avatar { get; set; }
        //座机
        public string Telephone { get; set; }
        //是否启动
        public bool Enable { get; set; }
        //状态
        public UserState State { get; set; }
        //unionid
        public string UnionID { get; set; }

    }

    [Flags]
    public enum UserState
    {
        /// <summary>
        /// 已激活
        /// </summary>
        Active = 1,
        /// <summary>
        /// 已经禁用
        /// </summary>
        Disable = 2,
        /// <summary>
        /// 未激活
        /// </summary>
        NoActive = 4
    }
}