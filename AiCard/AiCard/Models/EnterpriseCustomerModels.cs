using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard.Models
{
    /// <summary>
    /// 公司客户表
    /// </summary>
    public class EnterpriseCustomer : AiCard.IDistrict, AiCard.IGprs
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
        public Enums.Gender Gender { get; set; }

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
        public DateTime? Birthday { get; set; }

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


        public List<EnterpriseCustomerRemark> CustomerRemarks { get; set; }
    }

    /// <summary>
    /// 员工的公司客户备注表
    /// </summary>
    public class EnterpriseCustomerRemark
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

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 标签存放CustomerTabForCustomer集合的JSON
        /// </summary>
        public string Tabs { get; set; }

    }

    /// <summary>
    /// 给CustomerRemark的Tabs字段做Json解析用
    /// </summary>
    public class HelpEnterpriseCustomerRemarkTabs
    {
        public string Name { get; set; }

        public Enums.CardTabStyle Style { get; set; }
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
}