using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AiCard.Common.TxIm
{
    public static class ImConfig
    {

        /// <summary>
        /// SdkAppId 腾讯云里面查看
        /// </summary>
        public static string SdkAppId = ConfigurationManager.AppSettings["tximSdkAppId"].ToString(); //1400157072;
        /// <summary>
        /// 腾讯云里面定于的admin帐号
        /// </summary>
        public static string Admin = ConfigurationManager.AppSettings["tximAdmin"].ToString();//"admin";

        private static string privateKeyPath;
        public static string PrivateKeyPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(privateKeyPath))
                {
                    privateKeyPath = HttpContext.Current.Server.MapPath("~/TxImKey/private_key");
                }
                return privateKeyPath;
            }
        }

        private static string publicKeyPath;

        public static string PublicKeyPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(privateKeyPath))
                {
                    publicKeyPath = HttpContext.Current.Server.MapPath("~/TxImKey/public_key");
                }
                return publicKeyPath;
            }
        }

        
        /// <summary>
        /// 签名 生成日2018-11-7 有效期180天
        /// <para>签名使用cmd 调用tls_licence_tools.exe（没有包含在项目里）</para> 
        /// <para>输入命令行 tls_licence_tools.exe gen private_key sig 1400157072 admin 生成有效期180天</para>
        /// <para>tls_licence_tools.exe gen {私钥路径} {签名路径} {SdkAppId} {管理员名称}</para>
        /// <para>tls_licence_tools verify public_key sig 1400157072 admin 验证签名和查看有效时间</para>
        /// <para>tls_licence_tools verify {公钥路径} {签名路径} {SdkAppId} {管理员名称}</para>
        /// </summary>
        private static string sign;
        public static string Sign
        {
            get
            {
                if (DateTime.Now > new DateTime(2018, 11, 7).AddDays(180))
                {
                    throw new Exception("腾讯云IM管理员的签名已过时)");
                }
                if (string.IsNullOrWhiteSpace(sign))
                {
                    sign = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/TxImKey/sig"));
                }
                return sign;
            }
        }

        /// <summary>
        /// 随机8位数字
        /// </summary>
        public static int Random
        {
            get
            {
                return Comm.Random.Next(10000000, 99999999);
            }

        }
    }


}