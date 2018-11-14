using AiCard.Common.Enums;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AiCard
{
    [Flags]
    public enum PageStyle
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default = 1,
        /// <summary>
        /// 显示总数
        /// </summary>
        ShowTotal = 2,
    }

    public static class PagedListExt
    {
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int index, int pageSize)
        {
            return new PagedList<T>(source, index, pageSize);
        }

        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int index)
        {
            return new PagedList<T>(source, index, 15);
        }
    }

    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString PageList(this HtmlHelper source, IPagedList list, string templateUrl)
        {
            return PageList(source, list, templateUrl, 10, PageStyle.Default);
        }



        public static MvcHtmlString PageList(this HtmlHelper source, IPagedList list, string templateUrl, int totalGroups)
        {
            return PageList(source, list, templateUrl, totalGroups, PageStyle.Default);
        }

        public static MvcHtmlString PageList(this HtmlHelper source, IPagedList list, string templateUrl, int totalGroups, PageStyle style)
        {
            if (list.TotalItemCount == 0)
            {
                return new MvcHtmlString("");
            }
            StringBuilder sb = new StringBuilder();
            if (list.HasPreviousPage)
            {
                sb.Append($"<li><a href=\"{Url(templateUrl, 1)}\">&laquo;</a></li>");
                sb.Append($"<li><a href=\"{Url(templateUrl, list.PageNumber - 1)}\">&lsaquo;</a></li>");
            }
            else
            {
                sb.AppendFormat("<li class=\"disabled\"><a>&laquo;</a></li>");
                sb.AppendFormat("<li class=\"disabled\"><a>&lsaquo;</a></li>");
            }

            int start = 0, end = list.PageCount;
            if (list.PageCount >= totalGroups)
            {
                if (totalGroups % 2 == 0)
                {
                    if (list.PageNumber < totalGroups / 2)
                    {
                        start = 0;
                        end = totalGroups;
                    }
                    else if (list.PageNumber + totalGroups / 2 >= list.PageCount)
                    {
                        start = list.PageCount - totalGroups;
                    }
                    else
                    {
                        start = list.PageNumber - totalGroups / 2 + 1;
                        end = list.PageNumber + totalGroups / 2 + 1;
                    }
                }
                else
                {
                    if (list.PageNumber <= totalGroups / 2)
                    {
                        start = 0;
                        end = totalGroups;
                    }
                    else if (list.PageNumber + totalGroups / 2 >= list.PageCount)
                    {
                        start = list.PageCount - totalGroups;
                    }
                    else
                    {
                        start = list.PageNumber - totalGroups / 2;
                        end = list.PageNumber + totalGroups / 2 + 1;
                    }
                }
            }


            for (int i = start + 1; i < end + 1; i++)
            {
                if (i == list.PageNumber)
                {
                    sb.Append($"<li class=\"active\"><a>{i}<span class=\"sr-only\">(current)</span></a></li>");
                }
                else
                {
                    sb.Append($"<li><a href=\"{Url(templateUrl, i)}\">{i}</a></li>");
                }
            }

            if (list.HasNextPage)
            {
                sb.Append($"<li><a href=\"{Url(templateUrl, list.PageNumber + 1)}\">&rsaquo;</a></li>");
                sb.Append($"<li><a href=\"{Url(templateUrl, list.PageCount)}\">&raquo;</a></li>");
            }
            else
            {
                sb.AppendFormat("<li class=\"disabled\"><a>&rsaquo;</a></li>");
                sb.AppendFormat("<li class=\"disabled\"><a>&raquo;</a></li>");
            }
            if ((style & PageStyle.ShowTotal) == PageStyle.ShowTotal)
            {
                return new MvcHtmlString($"<nav class='pagenav'><ul class=\"pagination\">{sb.ToString()}</ul><div class='total'>共{ list.PageCount}页{list.TotalItemCount}行</div></nav>");
            }
            return new MvcHtmlString($"<nav class='pagenav'><ul class=\"pagination\">{sb.ToString()}</ul></nav>");
        }

        private static string Url(string templateUrl, int page)
        {

            if (!templateUrl.Contains("{PageIndex}"))
            {
                throw new Exception("必须包含{PageIndex}");
            }
            return templateUrl.Replace("{PageIndex}", page.ToString());
        }

        /// <summary>
        /// 扩展方法，Content的调用空字符串的时候会报错，这个方法为了绕过报错
        /// </summary>
        /// <param name="source"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ContentNullEmpty(this UrlHelper source, string url)
        {
            return string.IsNullOrEmpty(url) ? "" : source.Content(url);
        }

        public static string ContentFull(this UrlHelper source, string url)
        {
            return string.IsNullOrEmpty(url) ? "" : new Uri(HttpContext.Current.Request.Url, source.Content(url)).AbsoluteUri;
        }

        /// <summary>
        /// ResizeImage图片地址生成
        /// </summary>
        /// <param name="source"></param>
        /// <param name="url">图片地址</param>
        /// <param name="w">最大宽度</param>
        /// <param name="h">最大高度</param>
        /// <param name="quality">质量0~100</param>
        /// <param name="img">占位图类别</param>
        /// <returns>地址为空返回null</returns>
        public static string ResizeImage(this UrlHelper source, string url, int? w = null, int? h = null,
            int? quality = null,
            DummyImage? img = DummyImage.Default,
            ResizerMode? mode = null,
            ReszieScale? scale = ReszieScale.Both)
        {
            return Comm.ResizeImage(url, w, h, quality, img, mode, scale);
        }

        /// <summary>
        /// 扩展方法，判断连接是否合法，考虑到后台使用者陪链接可能会填错导致页面报错
        /// </summary>
        /// <param name="source"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool ContentCheck(this UrlHelper source, string url)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(url))
                {
                    source.ContentNullEmpty(url);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 扩展方法，生成PageUrl
        /// </summary>
        /// <param name="source"></param>
        /// <param name="actionName">Action</param>
        /// <param name="controlName">Controller</param>
        /// <param name="para">参数</param>
        /// <returns></returns>
        public static string PageUrl(this UrlHelper source, string actionName, string controlName = null, Dictionary<string, object> para = null)
        {
            string url;
            if (string.IsNullOrWhiteSpace(controlName))
            {
                url = source.Action(actionName);
            }
            else
            {
                url = source.Action(actionName, controlName);
            }
            return $"{url}?page={{PageIndex}}{para.ToParam("&")}";
        }

        /// <summary>
        /// 扩展方法，自动生成PageUrl
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string PageUrl(this UrlHelper source)
        {
            return source.WithAllPara("Page", "{PageIndex}");
        }

        /// <summary>
        /// 设置连接参数，自动把key变成变量部分，其他key直接接上
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="actionName"></param>
        /// <param name="controlName"></param>
        /// <param name="exclude">排除掉不要key</param>
        /// <returns></returns>
        public static string WithAllPara(this UrlHelper source, string key, object value,
            string actionName = null, string controlName = null,
            string[] exclude = null)
        {
            if (exclude == null)
            {
                exclude = new string[0];
            }

            var Request = HttpContext.Current.Request;
            Dictionary<string, object> temp = new Dictionary<string, object>();
            string url;
            if (actionName == null)
            {
                url = Request.AppRelativeCurrentExecutionFilePath;
            }
            else if (controlName == null)
            {
                url = source.Action(actionName);
            }
            else
            {
                url = source.Action(actionName, controlName);
            }

            var excludeToLower = exclude.Select(s => s.ToLower()).ToArray();
            foreach (string item in Request.QueryString.Keys)
            {
                var k = item.ToLower();
                if (k != key.ToLower() && !excludeToLower.Any(s => s == k))
                {
                    temp.Add(item, Request[item]);
                }
            }
            temp.Add(key, value);
            return source.ContentNullEmpty($"{url}{temp.ToParam("?")}");

        }

        /// <summary>
        /// 获取地址栏上选中的枚举
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="source"></param>
        /// <param name="key">字段</param>
        /// <param name="defatul">没有时候的默认值</param>
        /// <returns></returns>
        public static string SeletedEnum<T>(this HtmlHelper source, string key, string defatul)
        {
            var t = typeof(T);
            string str = HttpContext.Current.Request[key];

            if (!string.IsNullOrWhiteSpace(str))
            {
                int iTemp;
                if (int.TryParse(str, out iTemp) && Enum.IsDefined(t, iTemp))
                {
                    return ((Enum)Enum.Parse(t, str)).GetDisplayName();
                }
                else if (Enum.IsDefined(t, str))
                {
                    return ((Enum)Enum.Parse(t, str)).GetDisplayName();
                }

            }
            return defatul;
        }

        //public static MvcHtmlString WeChatJsSdkConfig(this HtmlHelper source)
        //{
        //    var config = Newtonsoft.Json.JsonConvert.SerializeObject(new
        //    {
        //        appid = Buy.WeChat.Config.AppID,
        //        signature = Buy.WeChat.Config.JsSign(HttpContext.Current.Request.Url.AbsoluteUri),
        //        timestamp = Buy.WeChat.Config.JsapiTimeStamp,
        //    });
        //    return new MvcHtmlString($"<input type='hidden' id='wechat' data-config='{config}'/>");
        //}

        public static HtmlString TableImg(this HtmlHelper source, string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }
            else
            {
                var img = Comm.ResizeImage(url);
                var tableImg = Comm.ResizeImage(url, h: 23, w: 23, mode: ResizerMode.Crop);
                var tableImgBig = Comm.ResizeImage(url, h: 120);
                return new MvcHtmlString($"<a class='table-img' href='{img}'  target='_blank'><img src='{tableImg}'/><img src='{tableImgBig}'/></a>");
            }
        }




    }

    /// <summary>
    /// 新的分页，和Shared/DisplayTemplates/Page.cshtml 一起用
    /// </summary>
    public class Page
    {
        public Page()
        {

        }

        public Page(IPagedList soucre, string url)
        {
            Url = url;
            HasNext = HasPrev = true;
            HasGo = HasCount = HasFirst = HasLast = false;
            Source = soucre;
            PageCount = 7;
            var iconLeft = "<span>‹</span>";
            var iconRight = "<span>›</span>";
            var iconFirst = "<span>«</span>";
            var iconLast = "<span>»</span>";
            TextPrev = $"{iconLeft}上一页";
            TextNext = $"下一页{iconRight}";
            TextFirst = $"{iconFirst}首页";
            TextLast = $"尾页{iconLast}";
        }

        public bool HasNext { get; set; }

        public bool HasPrev { get; set; }

        public bool HasFirst { get; set; }

        public bool HasLast { get; set; }

        private string _url;
        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                if (!value.Contains("{PageIndex}"))
                {
                    throw new Exception("必须包含{PageIndex}");
                }
                _url = value;
            }
        }

        public bool HasCount { get; set; }

        public bool HasGo { get; set; }

        public string TextNext { get; set; }

        public string TextPrev { get; set; }

        public string TextFirst { get; set; }

        public string TextLast { get; set; }

        public IPagedList Source { get; set; }

        /// <summary>
        /// 分页个数
        /// </summary>
        public int PageCount { get; set; }

    }


}