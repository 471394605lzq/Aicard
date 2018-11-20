using System;
using System.Collections.Generic;
using System.Web;
using System.IO;

namespace WxPayAPI
{
    public class Log
    {


        /**
         * 向日志写入调试信息
         * @param className 类名
         * @param content 写入内容
         */
        public static void Debug(string className, string content)
        {
            if (WxPayConfig.GetConfig().GetLogLevel() >= 3)
            {
                WriteLog("DEBUG", className, content);
            }
        }

        /**
        * 向日志写入运行时信息
        * @param className 类名
        * @param content 写入内容
        */
        public static void Info(string className, string content)
        {
            if (WxPayConfig.GetConfig().GetLogLevel() >= 2)
            {
                WriteLog("INFO", className, content);
            }
        }

        /**
        * 向日志写入出错信息
        * @param className 类名
        * @param content 写入内容
        */
        public static void Error(string className, string content)
        {
            if (WxPayConfig.GetConfig().GetLogLevel() >= 1)
            {
                WriteLog("ERROR", className, content);
            }
        }

        /**
        * 实际的写日志操作
        * @param type 日志记录类型
        * @param className 类名
        * @param content 写入内容
        */
        protected static void WriteLog(string type, string className, string content)
        {
            //日志内容
            string write_content = type + " " + className + ": " + content;
            //需要用户自定义日志实现形式
            AiCard.Common.Enums.DebugLogLevel logType;
            switch (type)
            {
                default:
                case "DEBUG":
                    logType = AiCard.Common.Enums.DebugLogLevel.Normal;
                    break;
                case "INFO":
                    logType = AiCard.Common.Enums.DebugLogLevel.Normal;
                    break;
                case "ERROR":
                    logType = AiCard.Common.Enums.DebugLogLevel.Error;
                    break;
            }
            AiCard.Common.Comm.WriteLog("WeChatPay", write_content, logType);

        }
    }
}