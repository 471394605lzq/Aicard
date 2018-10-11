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

    public enum Gender
    {
        NoSet,
        Male,
        Female
    }
}