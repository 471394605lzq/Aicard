using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AiCard.DAL.Models
{
    /// <summary>
    /// 语术
    /// </summary>
    public class UserSpeech
    {
        public int ID { get; set; }

        [Display(Name = "内容")]
        public string Content { get; set; }

        [Display(Name = "用户")]
        public string UserID { get; set; }

        [Display(Name = "排序")]
        public int Sort { get; set; }


        public UserSpeechType Type { get; set; }

        [Display(Name = "类别")]
        public int TypeID { get; set; }
    }

    /// <summary>
    /// 用户的语术分类
    /// </summary>
    public class UserSpeechType
    {
        public int ID { get; set; }

        [Display(Name = "分类名字")]
        public string Name { get; set; }

        [Display(Name = "排序")]
        public int Sort { get; set; }

        [Display(Name = "用户")]
        public string UserID { get; set; }

        public List<UserSpeech> Speechs { get; set; }

    }

}