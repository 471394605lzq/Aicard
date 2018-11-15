using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace AiCard.Common
{
    public class LogBase
    {
        /// <summary>
        /// 类别，没个类别每天一个文件
        /// </summary>
        public virtual string Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Title { get; set; }

        public virtual string Option { get; set; }
        public virtual string Message { get; set; }

        public virtual void Write()
        {
            Func<string, string> FPad = s => s.PadLeft(10, ' ');
            string log = $"{FPad(DateTime.Now.ToString("HH:mm:ss"))}{FPad(Title)}{FPad(Option)}{FPad(Message)}\r\n";
            string path = HttpContext.Current.Request.MapPath($"~/Log/{DateTime.Now:yyyyMMdd}");
            var dirInfo = new DirectoryInfo(path);
            var fileInfo = new FileInfo($"{path}/{Type}.log");
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            File.AppendAllText(fileInfo.FullName, log);
        }
    }


    
}