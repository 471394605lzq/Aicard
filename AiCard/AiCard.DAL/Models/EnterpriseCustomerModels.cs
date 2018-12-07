using AiCard.Common;
using AiCard.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard.DAL.Models
{
    /// <summary>
    /// 公司客户表
    /// </summary>
    public class EnterpriseCustomer : IDistrict, IGprs
    {
        public int ID { get; set; }

        /// <summary>
        /// 企业ID
        /// </summary>
        public int EnterpriseID { get; set; }




        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; } = null;

        /// <summary>
        /// 邮件
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 县镇
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 用户标签（存放CustomerTab集合的Json）
        /// </summary>
        public string Tabs { get; set; }

        /// <summary>
        /// 维度
        /// </summary>
        public double? Lat { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double? Lng { get; set; }


        /// <summary>
        /// 记录
        /// </summary>
        public string Log { get; set; }


        public List<EnterpriseUserCustomer> CustomerRemarks { get; set; }

        public List<EnterpriseCustomerTab> CustomerTabs { get; set; }
    }

    /// <summary>
    /// 员工的公司客户备注表
    /// </summary>
    public class EnterpriseUserCustomer
    {
        public int ID { get; set; }


        public EnterpriseCustomer Customer { get; set; }

        /// <summary>
        /// 关联的客户
        /// </summary>
        public int CustomerID { get; set; }

        /// <summary>
        /// 关联的员工
        /// </summary>
        public string OwnerID { get; set; }

        ///// <summary>
        ///// 来源
        ///// </summary>
        public Common.Enums.EnterpriseUserCustomerSource Source { get; set; }

        ///// <summary>
        ///// 状态
        ///// </summary>
        public Common.Enums.EnterpriseUserCustomerState State { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDateTime { get; set; }

    }

    /// <summary>
    /// 企业客户标签表
    /// </summary>
    public class EnterpriseCustomerTab
    {

        public int ID { get; set; }

        public int CustomerID { get; set; }

        public EnterpriseCustomer Customer { get; set; }

        public string OwnerID { get; set; }

        public string Name { get; set; }

        public CardTabStyle Style { get; set; }

        public DateTime CreateDateTime { get; set; }
    }


    /// <summary>
    /// 给Customer的Log字段做Json解析用
    /// </summary>
    public class HelpEnterpriseCustomerLog
    {
        public string UserID { get; set; }

        /// <summary>
        /// 存档的是一个KeyValue的Json[{"Key":"RealName","Value":"张三"},{"Key":"Gender","Value":1}]　
        /// </summary>
        public string Content { get; set; }

        public DateTime CreateDateTime { get; set; }

    }

    public class AddEnterpriseCustomer
    {
        public int ID { get; set; }

        /// <summary>
        /// 企业ID
        /// </summary>
        public int EnterpriseID { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }


        ///// <summary>
        ///// 来源
        ///// </summary>
        public Common.Enums.EnterpriseUserCustomerSource Source { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string HeadImgUrl { get; set; }

        /// <summary>
        /// 统一用户ID
        /// </summary>
        public string UnionID { get; set; }

        /// <summary>
        /// 加密的用户信息
        /// </summary>
        public string EncryptedData { get; set; }


        public string IV { get; set; }

        public string OpenID { get; set; }

        /// <summary>
        /// 是否已经关注
        /// </summary>
        public bool IsSubscribe { get; set; }

        public WeChatAccount Type { get; set; } = WeChatAccount.AiCardMini;

    }

}