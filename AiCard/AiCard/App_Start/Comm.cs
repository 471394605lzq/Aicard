using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Drawing;
using System.Text;
using System.Security.Cryptography;

namespace AiCard
{
    public class Comm
    {
        private static Random _random;
        /// <summary>
        /// 系统唯一随机
        /// </summary>
        public static Random Random
        {
            get
            {
                if (_random == null)
                {
                    _random = new Random();
                }
                return _random;
            }
        }

        /// <summary>
        /// 是否是移动端
        /// </summary>
        public static bool IsMobileDrive
        {
            get
            {
                var request = HttpContext.Current.Request;
                return request.Browser.IsMobileDevice || request.UserAgent.ToLower().Contains("micromessenger");
            }
        }

        /// <summary>
        /// 是否是移动端
        /// </summary>
        public static bool IsWeChat
        {
            get
            {
                var request = HttpContext.Current.Request;
                return request.UserAgent.ToLower().Contains("micromessenger");
            }
        }



        /// <summary>
        /// 设置WebConfig
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public static void SetConfig(string key, string val)
        {
            System.Configuration.ConfigurationManager.AppSettings.Set(key, val);
        }

        /// <summary>
        /// 读取WebConfig
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetConfig<T>(string key)
        {
            return (T)Convert.ChangeType(System.Configuration.ConfigurationManager.AppSettings[key], typeof(T));
        }

        /// <summary>
        /// 写LOG，LOG将按日期分类
        /// </summary>
        /// <param name="type">不同类别保存在不同文件里面</param>
        /// <param name="message">正文</param>
        /// <param name="url">请求地址</param>
        public static void WriteLog(string type, string message, Enums.DebugLogLevel lv, string url = "", Exception ex = null)
        {
            var setting = GetConfig<string>("DebugLog");
            Enums.DebugLog sysDebugLog;
            Enum.TryParse<Enums.DebugLog>(setting, out sysDebugLog);

            Action writeLog = () =>
            {
                var path = HttpContext.Current.Request.MapPath($"~/Logs/{DateTime.Now:yyyy-MM-dd}/{type}.log");
                System.IO.FileInfo info = new System.IO.FileInfo(path);
                if (!info.Directory.Exists)
                {
                    info.Directory.Create();
                }
                System.IO.File.AppendAllText(path, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message} {url} {ex?.Source}\r\n");
            };

            switch (sysDebugLog)
            {
                case Enums.DebugLog.All:
                    writeLog();
                    break;
                default:
                case Enums.DebugLog.No:
                    break;
                case Enums.DebugLog.Warning:
                    if (lv == Enums.DebugLogLevel.Warning || lv == Enums.DebugLogLevel.Error)
                    {
                        writeLog();
                    }
                    break;
                case Enums.DebugLog.Error:
                    if (lv == Enums.DebugLogLevel.Error)
                    {
                        writeLog();
                    }
                    break;
            }


        }


        /// <summary>
        /// ResizeImage图片地址生成
        /// </summary>
        /// <param name="url">图片地址</param>
        /// <param name="w">最大宽度</param>
        /// <param name="h">最大高度</param>
        /// <param name="quality">质量0~100</param>
        /// <param name="image">占位图类别</param>
        /// <returns>地址为空返回null</returns>
        public static string ResizeImage(string url, int? w = null, int? h = null,
            int? quality = null,
            Enums.DummyImage? image = Enums.DummyImage.Default,
            Enums.ResizerMode? mode = null,
            Enums.ReszieScale? scale = null
            )
        {
            var Url = new System.Web.Mvc.UrlHelper(HttpContext.Current.Request.RequestContext);
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }
            else
            {
                if (Url.IsLocalUrl(url))
                {
                    var t = new Uri(HttpContext.Current.Request.Url, Url.Content(url)).AbsoluteUri;
                    Dictionary<string, string> p = new Dictionary<string, string>();
                    if (w.HasValue)
                    {
                        p.Add("w", w.ToString());
                    }
                    if (h.HasValue)
                    {
                        p.Add("h", h.ToString());
                    }
                    if (scale.HasValue)
                    {
                        p.Add("scale", scale.Value.ToString());
                    }
                    if (quality.HasValue)
                    {
                        p.Add("quality", quality.ToString());
                    }
                    if (image.HasValue)
                    {
                        p.Add("404", image.ToString());
                    }
                    if (mode.HasValue)
                    {
                        p.Add("mode", mode.ToString());
                    }
                    return t + p.ToParam("?");
                }
                else if (url.Contains(Qiniu.QinQiuApi.ServerLink))
                {
                    Dictionary<string, string> p = new Dictionary<string, string>();
                    if (w.HasValue || h.HasValue)
                    {

                    }
                    if (w.HasValue)
                    {
                        p.Add("w", w.ToString());
                    }
                    if (h.HasValue)
                    {
                        p.Add("h", h.ToString());
                    }
                    if (scale.HasValue)
                    {
                        p.Add("scale", scale.Value.ToString());
                    }
                    if (quality.HasValue)
                    {
                        p.Add("quality", quality.ToString());
                    }
                    if (mode.HasValue)
                    {
                        p.Add("mode", mode.ToString());
                    }
                    if (p.Count > 0)
                    {
                        //StringBuilder sb = new StringBuilder(url);
                        //foreach (var item in p)
                        //{
                        //    sb.Append($"{}")
                        //}
                        //url = url +
                    }
                    return $"{url}{p.ToParam("?imageMogr2/")}";
                }
                else
                {
                    return url;
                }
            }
        }

        public static Dictionary<string, object> ToJsonResult(string state, string message, object data = null)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("State", state);
            result.Add("Message", message);
            result.Add("Result", data);
            //if (data != null)
            //{
            //    foreach (var item in data.GetType().GetProperties())
            //    {
            //        result.Add(item.Name, item.GetValue(data));
            //    }
            //}

            return result;
        }

        public static Dictionary<string, object> ToJsonResultForPagedList(PagedList.IPagedList page, object data = null)
        {

            return ToJsonResult("Success", "成功", new
            {
                Page = new
                {
                    page.PageNumber,
                    page.PageCount,
                    page.HasNextPage
                },
                Data = data
            });

        }


        public static Enums.DriveType GetDriveType()
        {
            string userAgent = HttpContext.Current.Request.UserAgent.ToLower();
            if (userAgent.Contains("windows phone"))
            {
                return Enums.DriveType.Windows;
            }
            if (userAgent.Contains("iphone;"))
            {
                return Enums.DriveType.IPhone;
            }
            if (userAgent.Contains("ipad;"))
            {
                return Enums.DriveType.IPad;
            }
            if (userAgent.Contains("android"))
            {
                return Enums.DriveType.Android;
            }
            return Enums.DriveType.Windows;
        }


        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="data">内容</param>
        /// <param name="qrCodepath">二维码地址</param>
        /// <param name="tempPath">二维码地址无LOGO</param>
        /// <param name="logo">LOGO图</param>
        public static void GenerateQRCode(string data, string qrCodepath, string tempPath, Image logo = null)
        {
            try
            {
                Image image = null;
                var tempFilePath = HttpContext.Current.Request.MapPath(tempPath);
                var fileInfo = new FileInfo(tempFilePath);
                if (!fileInfo.Directory.Exists)
                {
                    fileInfo.Directory.Create();
                }
                if (!string.IsNullOrWhiteSpace(tempPath) && fileInfo.Exists)
                {
                    FileStream fs = new FileStream(HttpContext.Current.Request.MapPath(tempPath), FileMode.Open, FileAccess.Read);
                    image = Image.FromStream(fs);
                    fs.Close();
                }
                else
                {
                    image = QrCode.Generate(data);
                    image.Save(HttpContext.Current.Request.MapPath(tempPath));
                }
                if (logo != null)
                {
                    image = QrCode.SetLogo(image, logo);
                }
                //保存
                qrCodepath = HttpContext.Current.Request.MapPath(qrCodepath);

                image.Save(qrCodepath);
                image.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return;
        }

        public static string GetMd5Hash(string input)
        {
            using (System.Security.Cryptography.MD5 md5Hash = System.Security.Cryptography.MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }

        }

        // Verify a hash against a string.
        public static bool VerifyMd5Hash(string input, string hash)
        {
            using (System.Security.Cryptography.MD5 md5Hash = System.Security.Cryptography.MD5.Create())
            {

                // Hash the input.
                string hashOfInput = GetMd5Hash(input);

                // Create a StringComparer an compare the hashes.
                StringComparer comparer = StringComparer.OrdinalIgnoreCase;

                if (0 == comparer.Compare(hashOfInput, hash))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        private static readonly string PasswordHash = "P@@Sw0rd";
        private static readonly string SaltKey = "S@LT&KEY";
        private static readonly string VIKey = "@1B2c3D4e5F6g7H8";

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="plainText">文本</param>
        /// <returns></returns>
        public static string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="encryptedText">加密后文本</param>
        /// <returns></returns>
        public static string Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }
    }
}