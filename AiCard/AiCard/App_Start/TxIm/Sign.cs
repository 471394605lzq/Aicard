using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;

namespace AiCard.TxIm
{
    public class SigCheck
    {
        private const string dllPath = @"G:\AiCard\AiCard\AiCard\bin\sigcheck.dll";

        //private const string dllPath = @"G:\AiCard\AiCard\AiCard\bin\sigcheck.dll";

        [DllImport(dllPath, EntryPoint = "tls_gen_sig", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private extern static int tls_gen_sig(
            UInt32 expire,
            string appid3rd,
            UInt32 sdkappid,
            string identifier,
            UInt32 acctype,
            StringBuilder sig,
            UInt32 sig_buff_len,
            string pri_key,
            UInt32 pri_key_len,
            StringBuilder err_msg,
            UInt32 err_msg_buff_len
        );

        [DllImport(dllPath, EntryPoint = "tls_vri_sig", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private extern static int tls_vri_sig(
            string sig,
            string pub_key,
            UInt32 pub_key_len,
            UInt32 acctype,
            string appid3rd,
            UInt32 sdkappid,
            string identifier,
            StringBuilder err_msg,
            UInt32 err_msg_buff_len
        );

        [DllImport(dllPath, EntryPoint = "tls_gen_sig_ex", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private extern static int tls_gen_sig_ex(
            UInt32 sdkappid,
            string identifier,
            StringBuilder sig,
            UInt32 sig_buff_len,
            string pri_key,
            UInt32 pri_key_len,
            StringBuilder err_msg,
            UInt32 err_msg_buff_len
        );

        [DllImport(dllPath, EntryPoint = "tls_vri_sig_ex", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private extern static int tls_vri_sig_ex(
            string sig,
            string pub_key,
            UInt32 pub_key_len,
            UInt32 sdkappid,
            string identifier,
            ref UInt32 expire_time,
            ref UInt32 init_time,
            StringBuilder err_msg,
            UInt32 err_msg_buff_len
        );

        /// <summary>
        /// 对用户名签名
        /// </summary>
        /// <param name="user">用户名</param>
        /// <returns>签名</returns>
        public static string Sign(string user)
        {
            var f = new FileStream(Config.PrivateKeyPath, FileMode.Open, FileAccess.Read);
            var reader = new BinaryReader(f);
            byte[] b = new byte[f.Length];
            reader.Read(b, 0, b.Length);
            string pri_key = Encoding.Default.GetString(b);

            StringBuilder sig = new StringBuilder(4096);
            StringBuilder err_msg = new StringBuilder(4096);
            int ret = tls_gen_sig_ex(
                Config.SdkAppId,
                user,
                sig,
                4096,
                pri_key,
                (UInt32)pri_key.Length,
                err_msg,
                4096);
            reader.Close();
            f.Close();
            if (0 != ret)
            {
                throw new Exception(err_msg.ToString());
            }
            else
            {
                return sig.ToString();
            }
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="user">用户名</param>
        /// <param name="sign">签名</param>
        /// <returns>是否已经过期</returns>
        public static bool Vir(string user, string sign)
        {
            // 校验 sig
            var f = new FileStream(Config.PublicKeyPath, FileMode.Open, FileAccess.Read);
            var reader = new BinaryReader(f);
            var b = new byte[f.Length];
            reader.Read(b, 0, b.Length);
            string pub_key = Encoding.Default.GetString(b);
            var err_msg = new StringBuilder(4096);
            uint expire_time = 0;
            uint init_time = 0;
            var ret = tls_vri_sig_ex(
                 sign,
                 pub_key,
                 (uint)pub_key.Length,
                 Config.SdkAppId,
                 user,
                 ref expire_time,
                 ref init_time,
                 err_msg,
                 4096);

            if (0 != ret)
            {
                throw new Exception(err_msg.ToString());
            }
            return DateTime.Now < new DateTime(expire_time);
        }
    }

}