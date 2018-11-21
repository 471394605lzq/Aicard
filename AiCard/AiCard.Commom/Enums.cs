using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Common.Enums
{
    #region 公共枚举
    public enum DebugLog
    {
        /// <summary>
        /// 所有
        /// </summary>
        All,
        /// <summary>
        /// 不输出
        /// </summary>
        No,
        /// <summary>
        /// 警告以上
        /// </summary>
        Warning,
        /// <summary>
        /// 错误以上
        /// </summary>
        Error
    }

    public enum DebugLogLevel
    {
        /// <summary>
        /// 普通记录
        /// </summary>
        Normal,
        /// <summary>
        /// 警告级别
        /// </summary>
        Warning,
        /// <summary>
        /// 错误级别
        /// </summary>
        Error
    }

    /// <summary>
    /// 占位图
    /// </summary>
    public enum DummyImage
    {
        [Display(Name = "默认")]
        Default,
        [Display(Name = "头像")]
        Avatar
    }

    public enum ResizerMode
    {
        Pad,
        Crop,
        Max,
    }

    public enum ReszieScale
    {
        Down,
        Both,
        Canvas
    }

    /// <summary>
    /// 设备类型
    /// </summary>
    [Flags]
    public enum DriveType
    {
        Windows = 1,
        IPhone = 2,
        IPad = 4,
        Android = 8,
        WindowsPhone = 16,
    }

    public enum RoleType
    {
        System,
        Enterprise,
    }

    public enum FileType
    {
        /// <summary>
        /// 图片
        /// </summary>
        Image,
        /// <summary>
        /// 视频
        /// </summary>
        Video,
        /// <summary>
        /// 文本
        /// </summary>
        Text,
        /// <summary>
        /// 音频
        /// </summary>
        Audio,
        /// <summary>
        /// 其他
        /// </summary>
        Other
    }

    /// <summary>
    /// 错误页面的Layout类别，给错误页面使用的一个枚举
    /// </summary>
    public enum Layout
    {
        Manage,
        WebSide,
        MoblieWebSide
    }

    public enum CellStyle
    {
        RichText,
        CellList,

    }

    public enum ActionType
    {
        None,
        Browser,
        CardHome,
        ShopHome,
        EterpriseHome,
        NewsHome,
        ArticleDetail
    }
    #endregion

    public enum ArticleState
    {
        [Display(Name = "未审核")]
        Wait,
        [Display(Name = "已发布")]
        Released,
        [Display(Name = "已删除")]
        Deleted

    }

    public enum ArticleType
    {
        [Display(Name = "用户动态")]
        Text,
        [Display(Name = "后台发布")]
        Html,
    }

    #region 微信
    public enum WeChatAccount
    {
        /// <summary>
        /// 公众号
        /// </summary>
        PC,
        /// <summary>
        /// 小程序
        /// </summary>
        AiCardMini,

    }

    #endregion
    public enum UserType
    {
        [Display(Name = "管理员")]
        Admin,
        [Display(Name = "企业")]
        Enterprise,
        [Display(Name = "个人")]
        Personal
    }

    /// <summary>
    /// 性别
    /// </summary>
    public enum Gender
    {
        [Display(Name = "未设置")]
        NoSet,
        [Display(Name = "男")]
        Male,
        [Display(Name = "女")]
        Female
    }

    public enum UserLogType
    {
        [Display(Name = "文章点赞")]
        ArticleLike = 10,
        [Display(Name = "文章评论")]
        ArticleComment = 11,
        [Display(Name = "文章查看")]
        ArticleRead = 12,
        [Display(Name = "文章分享")]//动态分享
        ArticleShare = 13,
        [Display(Name = "在线沟通")]
        Communication = 20,
        [Display(Name = "商品查看")]
        ProductRead = 30,
        [Display(Name = "商品咨询")]
        ProductCon = 31,
        [Display(Name = "主页查看")]
        HomePageRead = 40,
        [Display(Name = "主页座机")]
        HomePagePhone = 41,
        [Display(Name = "主页Email")]
        HomePageEmail = 42,
        [Display(Name = "主页导航")]
        HomePageAddress = 43,
        [Display(Name = "主页分享")]//官网分享
        HomePageShare = 44,
        [Display(Name = "微信查看")]
        WeChatOpen = 50,
        [Display(Name = "名片查看")]
        CardRead = 60,
        [Display(Name = "转发名片")]//名片分享
        CardShare = 61,
        [Display(Name = "名片保存")]
        CardSave = 62,
        [Display(Name = "名片点赞数")]
        CardLike = 63,

        [Display(Name = "商城查看")]
        ShopRead = 70,
        [Display(Name = "手机拨打")]
        MobileCall = 80,
        [Display(Name = "邮件发送")]
        EmailSend = 81,
        [Display(Name = "语音播放")]
        VoicePlay = 82,
        [Display(Name = "视频播放")]
        VideoPlay = 83,
        [Display(Name = "手机拨打")]
        PhoneCall = 84,
        [Display(Name = "名片公司名称复制")]
        CardCompany = 85,
        [Display(Name = "名片公司导航")]
        CardAddress = 86,
        [Display(Name = "名片标签点击")]
        CardTab = 90,
        [Display(Name = "咨询")]
        CardCon = 100,

        [Display(Name = "客户跟进")]
        FollowUp = 101,
        [Display(Name = "新增标签")]
        AddCustTab = 102,


        [Display(Name = "微信好友分享")]
        ShareWeChatFriend = 103,
        [Display(Name = "微信群分享")]
        ShareWeChatGroup = 104,


        [Display(Name = "个人名片阅览")]
        CardPersonalRead = 200,
        [Display(Name = "个人名片点赞")]
        CardPersonalLike = 201,
        [Display(Name = "个人名片转发")]
        CardPersonalShare = 202,
        [Display(Name = "个人名片存到手机")]
        CardPersonalSave = 203,


        [Display(Name = "个人名片手机呼叫")]
        CardPersonalMobileCall = 210,
        [Display(Name = "个人名片邮件复制")]
        CardPersonalEmailCopy = 211,
        [Display(Name = "个人名片微信号复制")]
        CardPersonalWechat = 212,
        [Display(Name = "个人名片座机呼叫")]
        CardPersonalPhoneCall = 213,
        [Display(Name = "个人名片企业复制")]
        CardPersonalEnterpriseCopy = 214,
        [Display(Name = "个人名片地址导航")]
        CardPersonalAddressNav = 215,
    }

    public enum ProductType
    {
        [Display(Name = "实物")]
        Physical,
        [Display(Name = "虚拟")]
        Virtual
    }

    public enum HomePageModularType
    {
        [Display(Name = "富文本")]
        Html,
        [Display(Name = "图片")]
        Images,
        [Display(Name = "联系方式")]
        Contact,
        [Display(Name = "头部轮播")]
        Banner
    }

    public enum CardTabStyle
    {
        [Display(Name = "橙色")]
        Orange,
        [Display(Name = "绿色")]
        Green,
        [Display(Name = "蓝色")]
        Blue,
        [Display(Name = "紫色")]
        Purple
    }

    public enum PcasType
    {
        [Display(Name = "省份")]
        Province,
        [Display(Name = "城市")]
        City,
        [Display(Name = "城镇")]
        County
    }

    /// <summary>
    /// 排行类型
    /// </summary>
    public enum RankingsType
    {
        [Display(Name = "活跃度")]
        Activity,
        [Display(Name = "客户数")]
        CustNumber
    }
    /// <summary>
    /// 客户活跃排行榜
    /// </summary>
    public enum CustomerActiveType
    {
        [Display(Name = "全部")]
        All,
        [Display(Name = "周榜")]
        Week,
        [Display(Name = "月榜")]
        Month
    }

    /// <summary>
    /// 支付类型
    /// </summary>
    public enum PayChannel
    {
        [Display(Name = "微信支付")]
        WxPay,
        [Display(Name = "阿里支付")]
        AliPay
    }


    /// <summary>
    /// 订单状态
    /// </summary>
    public enum OrderState
    {
        [Display(Name = "待处理")]
        UnHandle = 0,
        [Display(Name = "成功")]
        Success = 1,
        [Display(Name = "失败")]
        Failed = 2,
        [Display(Name = "已取消")]
        Canceled = 3,
    }

    ///// <summary>
    ///// 订单类别
    ///// </summary>
    //public enum OrderType
    //{
    //    [Display(Name = "99购买VIP")]
    //    BuyVip99
    //}

    public enum VipRank
    {
        /// <summary>
        /// 用户
        /// </summary>
        [Display(Name = "免费")]
        Default,
        /// <summary>
        /// 99元VIP
        /// </summary>
        [Display(Name = "掌柜")]
        Vip99
    }

    public enum VipState
    {
        [Display(Name = "已禁用")]
        Disable,
        [Display(Name = "已启用")]
        Enable,

    }

    public enum EnterpriseUserCustomerSource
    {
        [Display(Name = "二维码")]
        QrCode,
        [Display(Name = "微信群")]
        Group,
        [Display(Name = "微信好友")]
        Friend,
        [Display(Name = "卡片列表")]
        CardList
    }

    public enum EnterpriseUserCustomerState
    {
        [Display(Name = "未跟进")]
        NoFllow,
        [Display(Name = "在跟进")]
        Follow,

    }

    public enum VipAmountLogType
    {
        [Display(Name = "新名片")]
        NewCard,
        [Display(Name = "新2级用户")]
        NewChild2nd,
        [Display(Name = "新3级用户")]
        NewChild3rd,
        [Display(Name = "提现")]
        Forward
    }

    public enum VipForwardState
    {
        [Display(Name = "待审核")]
        Waiting,
        [Display(Name = "审核不通过")]
        NoPass,
        [Display(Name = "审核通过")]
        Passed,
        [Display(Name = "转账失败")]
        Fail,
        [Display(Name = "转账成功")]
        Completed,
    }

    public enum VipForwardType
    {
        [Display(Name = "银行卡")]
        BankCard,
        [Display(Name = "红包")]
        WxHongBao
    }

    public enum OrderType
    {
        /// <summary>
        /// 收款
        /// </summary>
        Receivable,
        /// <summary>
        /// 退款
        /// </summary>
        Refund
    }

    public enum VipTotalAmountRankType
    {
        [Display(Name = "总榜")]
        All,
        [Display(Name = "周榜")]
        Week,
        [Display(Name = "月榜")]
        Month
    }
}