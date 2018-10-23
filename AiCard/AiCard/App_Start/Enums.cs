using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AiCard.Enums
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
        [Display(Name = "在线沟通")]
        Communication = 20,
        [Display(Name = "商品查看")]
        ProductRead = 30,
        [Display(Name = "商品咨询")]
        ProductCon = 31,
        [Display(Name = "主页查看")]
        HomePageRead = 40,
        [Display(Name = "微信查看")]
        WeChatOpen = 50,
        [Display(Name = "名片查看")]
        CardRead = 60,
        [Display(Name = "转发名片")]
        CardForward = 61,
        [Display(Name = "名片保存")]
        CardSave = 62,
        [Display(Name = "名片点赞数")]
        CardLike = 63,
        [Display(Name = "商城查看")]
        ShopRead = 70,
        [Display(Name = "手机拨打")]
        PhoneCall = 80,
        [Display(Name = "邮件发送")]
        EmailSend = 81,
        [Display(Name = "语音播放")]
        VoicePlay = 82,
        [Display(Name = "视频播放")]
        VideoPlay = 83
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
}