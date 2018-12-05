using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCard.Common.WeChat.WeChatMessageTemp
{
    public interface IWeChatMessageTemp
    {
        string ID { get; }

        object Data { get; }


    }

    public class PReceivableNotifyWeChatMessage : IWeChatMessageTemp
    {
        private object data;

        public PReceivableNotifyWeChatMessage(decimal amount, string remark, DateTime dateTime)
        {
            data = new
            {
                keyword1 = new { value = $"{amount}元" },
                keyword2 = new { value = remark },
                keyword3 = new { value = dateTime.ToString("yyyy-MM-dd HH:mm:ss") }
            };
        }

        public object Data
        {
            get
            {
                return data;
            }
        }

        public string ID
        {
            get
            {
                return "sB8aHqZ0CYR6Nx1vY8JXNSAmVPs_8LKhsTDSylvfPWU";
            }
        }
    }


    public class PNewUserNotifyWeChatMessage : IWeChatMessageTemp
    {
        private object data;

        public PNewUserNotifyWeChatMessage(string nickname, DateTime dateTime)
        {
            data = new
            {
                keyword1 = new { value = nickname },
                keyword2 = new { value = dateTime.ToString("yyyy-MM-dd HH:mm:ss") }
            };
        }

        public object Data
        {
            get
            {
                return data;
            }
        }

        public string ID
        {
            get
            {
                return "yRNGFeRZqFmopwfbW6ocHqG41Ef8p2ycW8TJnswx8yc";
            }
        }
    }

    public class PDefaultNotifyWeChatMessage : IWeChatMessageTemp
    {
        private object data;

        public PDefaultNotifyWeChatMessage(string nickname, string content, DateTime datetTime)
        {
            data = new
            {
                keyword1 = new { value = nickname },
                keyword2 = new { value = content },
                keyword3 = new { value = datetTime.ToString("yyyy-MM-dd HH:mm:ss") }
            };
        }

        public object Data
        {
            get
            {
                return data;
            }
        }

        public string ID
        {
            get
            {
                return "W-eZiDuyhEVC_K03XsaaT8m88AMHzm8J3cnxdm28K-I";
            }
        }
    }

    public class EDefaultNotifyWeChatMessage : IWeChatMessageTemp
    {
        private object data;

        public EDefaultNotifyWeChatMessage(string project, string nickname, string content, DateTime datetTime)
        {
            data = new
            {
                keyword1 = new { value = project },
                keyword2 = new { value = nickname },
                keyword3 = new { value = datetTime.ToString("yyyy-MM-dd HH:mm:ss") },
                keyword4 = new { value = content }
            };
        }

        public object Data
        {
            get
            {
                return data;
            }
        }

        public string ID
        {
            get
            {
                return "szJbdS4HgheYYCoDRy4sWwGZltbdSWYARzK5VYrzh1c";
            }
        }
    }
}
