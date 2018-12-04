using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System;
using System.Data.SqlClient;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using AiCard.Common.Enums;

namespace AiCard.DAL.Models
{
    // 可以通过向 ApplicationUser 类添加更多属性来为用户添加配置文件数据。若要了解详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=317594。
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在此处添加自定义用户声明
            return userIdentity;
        }

        [Display(Name = "注册时间")]
        public DateTime RegisterDateTime { get; set; }

        [Display(Name = "登录时间")]
        public DateTime LastLoginDateTime { get; set; }


        public int? RoleGroupID { get; set; }

        [Display(Name = "用户类型")]
        public UserType UserType { get; set; }

        [Display(Name = "企业")]
        public int EnterpriseID { get; set; }

        [Display(Name = "微信UnitID")]
        public string WeChatID { get; set; }

        [Display(Name = "昵称")]
        public string NickName { get; set; }

        [Display(Name = "头像")]
        public string Avatar { get; set; }

        /// <summary>
        /// 存一个Json
        /// </summary>
        [Display(Name = "OpenID")]
        public string OpenIDs { get; set; }
    }

    /// <summary>
    /// 权限
    /// </summary>
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }

        public RoleType Type { get; set; }

        public string Group { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public virtual string Description { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {

        }

        /// <summary>
        /// 角色
        /// </summary>
        public DbSet<RoleGroup> RoleGroups { get; set; }

        /// <summary>
        /// 企业
        /// </summary>
        public DbSet<Enterprise> Enterprises { get; set; }

        /// <summary>
        /// 企业名片
        /// </summary>
        public DbSet<Card> Cards { get; set; }

        /// <summary>
        /// 动态
        /// </summary>
        public DbSet<Article> Articles { get; set; }

        /// <summary>
        /// 动态评论
        /// </summary>
        public DbSet<ArticleComment> ArticleComments { get; set; }

        /// <summary>
        /// 用户行为记录
        /// </summary>
        public DbSet<UserLog> UserLogs { get; set; }

        /// <summary>
        /// 企业版商品
        /// </summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// 企业商品分类
        /// </summary>
        public DbSet<ProductKind> ProductKinds { get; set; }

        /// <summary>
        /// 企业版自定义主页模块
        /// </summary>
        public DbSet<HomePageModular> HomePageModulars { get; set; }

        /// <summary>
        /// 名片标签
        /// </summary>
        public DbSet<CardTab> CardTabs { get; set; }

        /// <summary>
        /// 商品置顶
        /// </summary>
        public DbSet<UserProductTop> UserProductTops { get; set; }

        /// <summary>
        /// 用户名片置顶
        /// </summary>
        public DbSet<UserCardTop> UserCardTops { get; set; }

        /// <summary>
        /// 企业客户
        /// </summary>
        public DbSet<EnterpriseCustomer> EnterpriseCustomers { get; set; }

        /// <summary>
        /// 企业客户备注
        /// </summary>
        public DbSet<EnterpriseUserCustomer> EnterpriseUserCustomer { get; set; }

        /// <summary>
        /// 企业预设标签
        /// </summary>
        public DbSet<CustomerTab> CustomerTabs { get; set; }

        /// <summary>
        /// 企业的预设标签分组
        /// </summary>
        public DbSet<CustomerTabGroup> CustomerTabGroups { get; set; }

        /// <summary>
        /// 企业客户的标签
        /// </summary>
        public DbSet<EnterpriseCustomerTab> EnterpriseCustomerTabs { get; set; }

        /// <summary>
        /// 语术
        /// </summary>
        public DbSet<UserSpeech> UserSpeechs { get; set; }

        /// <summary>
        /// 语术类别
        /// </summary>
        public DbSet<UserSpeechType> UserSpeechTypes { get; set; }

        /// <summary>
        /// 交易订单
        /// </summary>
        public DbSet<Order> Orders { get; set; }

        /// <summary>
        /// VIP（个人版用户扩展表）
        /// </summary>
        public DbSet<Vip> Vips { get; set; }

        /// <summary>
        /// VIP的关系记录
        /// </summary>
        public DbSet<VipRelationship> VipRelationships { get; set; }

        /// <summary>
        /// VIP的金额记录
        /// </summary>
        public DbSet<VipAmountLog> VipAmountLogs { get; set; }

        /// <summary>
        /// VIP提现
        /// </summary>
        public DbSet<VipForwardOrder> VipForwardOrders { get; set; }

        /// <summary>
        /// 商品置顶
        /// </summary>
        public DbSet<CardUserProductTop> CardUserProductTops { get; set; }

        /// <summary>
        /// 个人名片
        /// </summary>
        public DbSet<CardPersonal> CardPersonals { get; set; }

        /// <summary>
        /// 个人版VIP提现账号
        /// </summary>
        public DbSet<VipForwardAccount> VipForwardAccounts { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public DbSet<VerificationCode> VerificationCodes { get; set; }

        /// <summary>
        /// 微信小程序推送
        /// </summary>
        public DbSet<WeChatMiniNotifyForm> WeChatMiniNotifyForms { get; set; }

        public SqlConnection Connection { get; internal set; }




        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}