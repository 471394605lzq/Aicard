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
        Proxy,
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
}