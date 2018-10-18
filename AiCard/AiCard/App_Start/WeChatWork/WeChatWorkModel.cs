using System;
using System.Collections.Generic;
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

        public string Name { get; set; }

        public string Position { get; set; }


        public string Mobile { get; set; }


        public Enums.Gender Gender { get; set; }


        public string Email { get; set; }

        public string Avatar { get; set; }

        public string Telephone { get; set; }

        public bool Enable { get; set; }

        public UserState State { get; set; }

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