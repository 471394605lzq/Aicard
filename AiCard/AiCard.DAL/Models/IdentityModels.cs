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
    }


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

        public DbSet<RoleGroup> RoleGroups { get; set; }

        public DbSet<Enterprise> Enterprises { get; set; }

        public DbSet<Card> Cards { get; set; }

        public DbSet<Article> Articles { get; set; }

        public DbSet<ArticleComment> ArticleComments { get; set; }

        public DbSet<UserLog> UserLogs { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductKind> ProductKinds { get; set; }

        public DbSet<HomePageModular> HomePageModulars { get; set; }

        public DbSet<CardTab> CardTabs { get; set; }


        public DbSet<UserProductTop> UserProductTops { get; set; }

        public DbSet<UserCardTop> UserCardTops { get; set; }

        public DbSet<EnterpriseCustomer> EnterpriseCustomers { get; set; }

        public DbSet<EnterpriseUserCustomer> EnterpriseUserCustomer { get; set; }

        public DbSet<CustomerTab> CustomerTabs { get; set; }

        public DbSet<CustomerTabGroup> CustomerTabGroups { get; set; }

        public DbSet<EnterpriseCustomerTab> EnterpriseCustomerTabs { get; set; }

        public DbSet<UserSpeech> UserSpeechs { get; set; }

        public DbSet<UserSpeechType> UserSpeechTypes { get; set; }


        public DbSet<Order> Orders { get; set; }

        public DbSet<Vip> Vips { get; set; }


        public DbSet<VipRelationship> VipRelationships { get; set; }

        public DbSet<VipAmountLog> VipAmountLogs { get; set; }

        public DbSet<VipForwardOrder> VipForwardOrders { get; set; }


        public SqlConnection Connection { get; internal set; }



        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}