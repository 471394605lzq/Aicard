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

    public class ReceivableNotifyWeChatMessage : IWeChatMessageTemp
    {
        private object data;

        public ReceivableNotifyWeChatMessage(decimal amount, string remark, DateTime dateTime)
        {
            data = new
            {
                keyword1 = new { value = amount },
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


    public class NewUserNotifyWeChatMessage : IWeChatMessageTemp
    {
        private object data;

        public NewUserNotifyWeChatMessage(string nickname, DateTime dateTime)
        {
            data = new
            {
                keyword1 = new { value = nickname },
                keyword2 = new { value = dateTime }
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

    public class DefaultNotifyWeChatMessage : IWeChatMessageTemp
    {
        private object data;

        public DefaultNotifyWeChatMessage(string nickname, string content, DateTime datetTime)
        {
            data = new
            {
                keyword1 = new { value = nickname },
                keyword2 = new { value = content },
                keyword3 = new { value = datetTime }
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

}
