﻿using AiCard.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AiCard.Common.CommModels
{
    public class DrawingPictureModel
    {
        /// <summary>
        /// 海报图片名称
        /// </summary>
        public string PosterImageName { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 头像图片地址
        /// </summary>
        public string AvatarPath { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 公司logo图片地址
        /// </summary>
        public string LogoPath { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 二维码图片地址
        /// </summary>
        public string QrPath { get; set; }

        /// <summary>
        /// 标签内容信息
        /// </summary>
        public List<TagModel> TagList { get; set; }
        ///// <summary>
        ///// 标签1内容
        ///// </summary>
        //public string tag1 { get; set; }
        ///// <summary>
        ///// 标签2内容
        ///// </summary>
        //public string tag2 { get; set; }
        ///// <summary>
        ///// 标签1的颜色样式
        ///// </summary>
        //public string tagstyle1 { get; set; }
        ///// <summary>
        ///// 标签2的颜色样式
        ///// </summary>
        //public string tagstyle2 { get; set; }
    }
    public class TagModel
    {
        public string TagName { get; set; }
        public CardTabStyle TagStyle { get; set; }
    }

    public class DrawingPictureProsonal
    {
        public string BgImage { get; set; }

        public string QrCode { get; set; }

        public string Avatar { get; set; }

        public string Name { get; set; }

        public string FileName { get; set; }
    }
}