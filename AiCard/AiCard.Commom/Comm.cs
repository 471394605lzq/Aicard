using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Drawing;
using System.Text;
using System.Security.Cryptography;
using AiCard.Common.Enums;
using AiCard.Common.CommModels;
using AiCard;
using System.Diagnostics;
using System.Threading;
using AiCard.Common.Qiniu;

namespace AiCard.Common
{
    public class Comm
    {


        /// <summary>
        /// 升级VIP的会费（元）
        /// </summary>
        /// <returns></returns>
        public static decimal UpGradeAmount()
        {
            return 86m;
        }


        /// <summary>
        /// 注册上级可得佣金（元）
        /// </summary>
        /// <returns></returns>
        public static decimal RegisterAmount()
        {
            return 3m;
        }

        /// <summary>
        /// 升级后上级赚取佣金比例
        /// </summary>
        /// <returns></returns>
        public static decimal ParentCommissionRate()
        {
            return 0.5m;
        }

        /// <summary>
        /// 升级后上上级赚取佣金比例
        /// </summary>
        /// <returns></returns>
        public static decimal GrandfatheredCommissionRate()
        {
            return 0.1m;
        }

        /// <summary>
        /// 统一的请求结果
        /// </summary>
        public struct RequestResult
        {
            /// <summary>
            /// 请求结果 
            /// </summary>
            public ReqResultCode retCode { get; set; }

            /// <summary>
            /// 提示消息
            /// </summary>
            public string retMsg { get; set; }

            /// <summary>
            /// 返回对象数据
            /// </summary>
            public dynamic objectData { get; set; }
        }
        /// <summary>
        /// 请求结果枚举
        /// </summary>
        public enum ReqResultCode
        {
            /// <summary>
            /// 成功
            /// </summary>
            success = 1,
            /// <summary>
            /// 失败
            /// </summary>
            failed = 0,
            /// <summary>
            /// 异常
            /// </summary>
            excetion = 3

        }

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
        public static void WriteLog(string type, string message, DebugLogLevel lv, string url = "", Exception ex = null)
        {
            var setting = GetConfig<string>("DebugLog");
            DebugLog sysDebugLog;
            Enum.TryParse<DebugLog>(setting, out sysDebugLog);

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
                case DebugLog.All:
                    writeLog();
                    break;
                default:
                case DebugLog.No:
                    break;
                case DebugLog.Warning:
                    if (lv == DebugLogLevel.Warning || lv == DebugLogLevel.Error)
                    {
                        writeLog();
                    }
                    break;
                case DebugLog.Error:
                    if (lv == DebugLogLevel.Error)
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
            DummyImage? image = DummyImage.Default,
            ResizerMode? mode = null,
            ReszieScale? scale = null
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
                    var fileType = System.IO.Path.GetExtension(url);

                    StringBuilder sbUrl = new StringBuilder(url);
                    if (fileType == ".mp4")
                    {
                        sbUrl.Append("?vframe/jpg/offset/1");
                        if (w.HasValue)
                        {
                            sbUrl.Append($"/w/{w}");
                        }
                        if (h.HasValue)
                        {
                            sbUrl.Append($"/h/{h}");
                        }
                        return sbUrl.ToString();
                    }
                    else
                    {
                        sbUrl.Append("?imageView2");
                        switch (mode)
                        {
                            case ResizerMode.Pad:
                            default:
                            case ResizerMode.Crop:
                                sbUrl.Append("/1");
                                break;
                            case ResizerMode.Max:
                                sbUrl.Append("/0");
                                break;
                        }
                        if (w.HasValue)
                        {
                            sbUrl.Append($"/w/{w}");
                        }
                        if (h.HasValue)
                        {
                            sbUrl.Append($"/h/{h}");
                        }
                        quality = quality ?? 100;
                        sbUrl.Append($"/q/{quality}");
                        return sbUrl.ToString();
                    }

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
                    page.HasNextPage,
                    page.TotalItemCount,
                },
                Data = data
            });

        }


        public static Common.Enums.DriveType GetDriveType()
        {
            string userAgent = HttpContext.Current.Request.UserAgent.ToLower();
            if (userAgent.Contains("windows phone"))
            {
                return Common.Enums.DriveType.Windows;
            }
            if (userAgent.Contains("iphone;"))
            {
                return Common.Enums.DriveType.IPhone;
            }
            if (userAgent.Contains("ipad;"))
            {
                return Common.Enums.DriveType.IPad;
            }
            if (userAgent.Contains("android"))
            {
                return Common.Enums.DriveType.Android;
            }
            return Common.Enums.DriveType.Windows;
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

        public static string ConvertToMp3(string pathBefore, string pathLater)
        {
            string bgPath = System.Web.HttpContext.Current.Request.MapPath("~/App_Start/ffmpeg/") + @"ffmpeg.exe -i " + pathBefore + " " + pathLater;
            //string c = Server.MapPath("/ffmpeg/") + @"ffmpeg.exe -i " + pathBefore + " " + pathLater;
            string str = RunCmd(bgPath, pathLater);
            return str;
        }
        /// <summary>
        /// 执行Cmd命令
        /// </summary>
        private static string RunCmd(string c, string filename)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo("cmd.exe");
                info.RedirectStandardOutput = false;
                info.UseShellExecute = false;
                Process p = Process.Start(info);
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.Start();
                p.StandardInput.WriteLine(c);
                p.StandardInput.AutoFlush = true;
                Thread.Sleep(1000);
                p.StandardInput.WriteLine("exit");
                p.WaitForExit();
                string outStr = p.StandardOutput.ReadToEnd();
                p.Close();

                //Comm.WriteLog("RunCmd1", reader, DebugLogLevel.Error);
                return outStr;
            }
            catch (Exception ex)
            {
                return "error" + ex.Message;
            }
        }



        /// <summary>
        /// 生成海报方法
        /// </summary>
        /// <param name="strBg">背景图地址</param>
        /// <param name="model">海报内容model</param>
        /// <returns>返回海报图片地址</returns>
        public static string MergePosterImage(DrawingPictureModel model)
        {
            List<TagModel> taglist = model.TagList;
            string bgPath = System.Web.HttpContext.Current.Request.MapPath("~\\Content\\Images\\bg.png");
            string fname = "微软雅黑";
            Font f = new Font(fname, 18);
            Font f12 = new Font(fname, 12);
            Font font22 = new Font(fname, 22, FontStyle.Bold);
            Font font26 = new Font(fname, 26, FontStyle.Bold);
            Font font20 = new Font(fname, 20, FontStyle.Bold);

            // 初始化背景图片的宽高
            Bitmap bitMap = new Bitmap(933, 1500);
            // 初始化画板
            Graphics g1 = Graphics.FromImage(bitMap);
            ////设置画布背景颜色为白色
            //g1.FillRectangle(Brushes.White, new Rectangle(0, 0, 933, 1500));
            //g1.Clear(Color.White);

            //设置背景图
            FileStream bgfs = new FileStream(bgPath, FileMode.Open, FileAccess.Read);
            Image bgimage = Image.FromStream(bgfs);
            bgfs.Close();
            g1.DrawImage(bgimage, new Rectangle(0, 0, 933, 1500));

            //用setpixel方法绘制图片
            //Bitmap bitMap = new Bitmap(933, 1500);//初始化用来拼图的背景图并设置大小
            //Graphics g1 = Graphics.FromImage(bitMap);//初始化用来拼图的画板
            //Bitmap map = new Bitmap(model.AvatarPath);//读取要画到画板上的图片
            //g1.FillRectangle(Brushes.White, new Rectangle(0, 0, 834, 834));//设置要画的图片大小
            //循环设置好的要画的图片的宽和高来绘画图片
            //for (int i = 0; i < map.Width; i++)
            //{
            //    for (int j = 0; j < map.Height; j++)
            //    {
            //        var temp = map.GetPixel(i, j);//取每个像素和颜色
            //        bitMap.SetPixel(48 + i, 60 + j, temp);//将读取到的图片像素和颜色画到画板上
            //    }
            //}
            //map.Dispose();

            if (!string.IsNullOrWhiteSpace(model.AvatarPath))
            {
                //拼接头像图片
                FileStream avfs = new FileStream(model.AvatarPath, FileMode.Open, FileAccess.Read);
                Image avimage = Image.FromStream(avfs);
                avfs.Close();
                avfs.Dispose();
                g1.DrawImage(avimage, new Rectangle(48, 60, 834, 834));
            }

            if (!string.IsNullOrWhiteSpace(model.QrPath))
            {
                //拼接二维码之前画一个白色的圆底
                Rectangle rect = new Rectangle(670, 1230, 240, 240);
                Brush brush = new SolidBrush(Color.White);
                g1.FillEllipse(brush, rect);
                //描边
                Pen pen = Pens.Black;
                g1.DrawEllipse(pen, rect);

                //拼接二维码图片
                FileStream fs = new FileStream(model.QrPath, FileMode.Open, FileAccess.Read);
                Image image = Image.FromStream(fs);
                fs.Close();
                fs.Dispose();
                g1.DrawImage(image, new Rectangle(670, 1230, 240, 240));
            }

            if (!string.IsNullOrWhiteSpace(model.LogoPath))
            {
                //拼接公司logo图片
                FileStream logofs = new FileStream(model.LogoPath, FileMode.Open, FileAccess.Read);
                Image logoimage = Image.FromStream(logofs);
                logofs.Close();
                logofs.Dispose();
                g1.DrawImage(logoimage, new Rectangle(50, 1230, 96, 96));
            }

            if (taglist.Count > 0)
            {
                for (int i = 0; i < taglist.Count; i++)
                {
                    TagModel tempm = taglist[i];
                    string tempname = tempm.TagName;
                    int taglength = tempname.Length;
                    int tagwidth = taglength <= 5 ? 90 : taglength <= 7 ? 120 : taglength <= 10 ? 160 : taglength <= 13 ? 190 : 220;

                    Color bgc = new Color();
                    Color boc = new Color();
                    switch (tempm.TagStyle)
                    {
                        case CardTabStyle.Orange:
                            {
                                bgc = Color.FromArgb(255, 250, 249);
                                boc = Color.FromArgb(255, 223, 214);
                            }
                            break;
                        case CardTabStyle.Green:
                            {
                                bgc = Color.FromArgb(249, 255, 252);
                                boc = Color.FromArgb(190, 233, 215);
                            }
                            break;
                        case CardTabStyle.Blue:
                            {
                                bgc = Color.FromArgb(249, 255, 255);
                                boc = Color.FromArgb(214, 226, 255);
                            }
                            break;
                        case CardTabStyle.Purple:
                            {
                                bgc = Color.FromArgb(252, 249, 255);
                                boc = Color.FromArgb(234, 214, 255);
                            }
                            break;
                        default:
                            break;
                    }

                    if (i == 0)
                    {
                        DrawingPictures.SetBox(bitMap, g1, tagwidth, 50, Color.FromArgb(255, 223, 214), Color.FromArgb(255, 250, 249), 50, 1130, 2);
                        DrawingPictures.DrawStringWrap(g1, f12, tempname, new Rectangle(50, 1130, tagwidth, 50), 1145, 55, 18);
                    }
                    else
                    {
                        TagModel tempm2 = taglist[i - 1];
                        string tempname2 = tempm.TagName;
                        int taglength2 = tempname2.Length;
                        int tagwidth2 = taglength2 <= 5 ? 90 : taglength2 <= 7 ? 120 : taglength2 <= 10 ? 160 : taglength2 <= 13 ? 190 : 220;
                        DrawingPictures.SetBox(bitMap, g1, tagwidth, 50, Color.FromArgb(190, 233, 215), Color.FromArgb(249, 255, 252), 70 + tagwidth2, 1130, 2);
                        DrawingPictures.DrawStringWrap(g1, f12, tempname, new Rectangle(70 + tagwidth2, 1130, tagwidth, 50), 1145, 75 + tagwidth2, 18);
                    }
                }
                //计算标签内容的字数长度
                //int taglength1 = model.tag1.Length;
                //int taglength2 = model.tag2.Length;
                //int t1width = taglength1 <= 5 ? 90 : taglength1 <= 7 ? 120 : taglength1 <= 10 ? 160 : taglength1 <= 13 ? 190 : 220;
                //int t2width = taglength2 <= 5 ? 90 : taglength2 <= 7 ? 120 : taglength2 <= 10 ? 160 : taglength2 <= 13 ? 190 : 220;

                ////画第一个标签框
                //DrawingPictures.SetBox(bitMap, g1, t1width, 50, Color.FromArgb(255, 223, 214), Color.FromArgb(255, 250, 249), 50, 1130, 2);
                ////画第二个标签框
                //DrawingPictures.SetBox(bitMap, g1, t2width, 50, Color.FromArgb(190, 233, 215), Color.FromArgb(249, 255, 252), 70 + t1width, 1130, 2);
                ////第一个标签框内容
                //DrawingPictures.DrawStringWrap(g1, f12, model.tag1, new Rectangle(50, 1130, t1width, 50), 1145, 55, 18);
                ////第二个标签框内容
                //DrawingPictures.DrawStringWrap(g1, f12, model.tag2, new Rectangle(70 + t1width, 1130, t2width, 50), 1145, 75 + t1width, 18);
            }

            //姓名
            DrawingPictures.DrawStringWrap(g1, font26, model.UserName == null ? "" : model.UserName, new Rectangle(50, 1000, 400, 40), 950, 50, 15);
            //职位
            DrawingPictures.DrawStringWrap(g1, font22, model.Position == null ? "" : model.Position, new Rectangle(50, 1000, 400, 40), 995, 50, 15);
            //签名
            DrawingPictures.DrawStringWrap(g1, f, model.Remark == null ? "" : model.Remark, new Rectangle(50, 1000, 800, 60), 1050, 50, 20);
            //公司名称            
            DrawingPictures.DrawStringWrap(g1, font20, model.CompanyName == null ? "" : model.CompanyName, new Rectangle(50, 1250, 400, 160), 1350, 50, 12);

            // 保存输出到本地
            var path = $"~/Upload/{model.PosterImageName}.jpg";
            string savePath = System.Web.HttpContext.Current.Server.MapPath(path);
            bitMap.Save(savePath);
            //微信小程序的限制，图片放到七牛上无法缓存，然后无法把海报保存到相册
            //QinQiuApi qniu = new QinQiuApi();
            //string resultpath = qniu.UploadFile(savePath, true, true);
            g1.Dispose();
            bitMap.Dispose();
            //生成完成后删除本地缓存文件
            if (File.Exists(model.LogoPath))
            {
                File.Delete(model.LogoPath);
            }
            if (File.Exists(model.AvatarPath))
            {
                File.Delete(model.AvatarPath);
            }
            if (File.Exists(model.QrPath))
            {
                File.Delete(model.QrPath);
            }
            return path;
        }

        /// <summary>
        /// 生成海报方法
        /// </summary>
        /// <param name="model">海报内容model</param>
        /// <returns>返回海报图片地址(本地)</returns>
        public static string MergePosterPersonalImage(DrawingPictureProsonal model)
        {
            int resize = 3;

            // 初始化背景图片的宽高
            Bitmap bitMap = new Bitmap(303 * resize, 452 * resize);
            // 初始化画板
            Graphics g1 = Graphics.FromImage(bitMap);

            ////设置画布背景颜色为白色
            g1.FillRectangle(Brushes.White, new Rectangle(0, 0, bitMap.Width, bitMap.Height));

            //设置背景图
            Image imgBg, imgAvatar, imgQrCode;
            try
            {
                imgBg = DrawingPictures.DownloadImg(model.BgImage);
            }
            catch (Exception ex)
            {
                Comm.WriteLog("MergePosterPersonalImage", "背景图下载失败", DebugLogLevel.Warning, ex: ex);
                throw new Exception("背景图下载失败");
            }
            try
            {
                imgAvatar = DrawingPictures.CutEllipse(DrawingPictures.DownloadImg(model.Avatar));
            }
            catch (Exception ex)
            {
                Comm.WriteLog("MergePosterPersonalImage", "头像图下载失败", DebugLogLevel.Warning, ex: ex);
                throw new Exception("头像图下载失败");
            }
            try
            {
                imgQrCode = DrawingPictures.CutEllipse(DrawingPictures.DownloadImg(model.QrCode));
            }
            catch (Exception ex)
            {
                Comm.WriteLog("MergePosterPersonalImage", "头像图下载失败", DebugLogLevel.Warning, ex: ex);
                throw new Exception("二维码图下载失败");
            }

            g1.DrawImage(imgBg, new Rectangle(0, 0, bitMap.Width, 380 * resize));
            if (!string.IsNullOrWhiteSpace(model.Avatar))
            {
                //拼接头像图片
                int size = 48 * resize;
                imgAvatar = DrawingPictures.CutEllipse(DrawingPictures.DownloadImg(model.Avatar));
                g1.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g1.DrawImage(imgAvatar, 8 * resize, 390 * resize, 48 * resize, 48 * resize);
            }

            if (!string.IsNullOrWhiteSpace(model.QrCode))
            {
                //拼接二维码之前画一个白色的圆底
                var padding = 3 * resize;
                var qrCodeSize = 56 * resize;
                int xQrCode = 243 * resize;
                int yQrCode = 388 * resize;
                Rectangle rect = new Rectangle(xQrCode - padding, yQrCode - padding, qrCodeSize + (padding * 2), qrCodeSize + (padding * 2));
                Brush brush = new SolidBrush(Color.White);
                g1.FillEllipse(brush, rect);

                //拼接二维码图片
                imgQrCode = DrawingPictures.DownloadImg(model.QrCode);

                g1.DrawImage(imgQrCode, new Rectangle(xQrCode, yQrCode, qrCodeSize, qrCodeSize));
            }

            //姓名
            int fontSize = 12 * resize;
            Font fDefault = new Font("微软雅黑", fontSize);
            Font fEmoji = new Font("Segoe UI Emoji", fontSize);
            Color fc = Color.FromArgb(255, 44, 54, 76);
            Brush fb = new SolidBrush(fc);
            int fx = 64 * resize, fy = 405 * resize;
            var emojis = System.Text.RegularExpressions.Regex.Matches(model.Name, Reg.EMOJI);
            for (int i = 0; i < model.Name.Length;)
            {
                bool isFind = false;
                foreach (System.Text.RegularExpressions.Match item in emojis)
                {
                    if (item.Index == i)
                    {
                        g1.DrawString(item.Value, fEmoji, fb, fx, 405 * resize);
                        fx += (int)(Math.Ceiling(g1.MeasureString(item.Value, fEmoji).Width)) - 4 * resize;
                        i += item.Length;
                        isFind = true;
                        break;
                    }
                }
                if (i == model.Name.Length || isFind)
                {
                    continue;
                }
                string txt = model.Name[i].ToString();
                g1.DrawString(txt, fDefault, fb, fx, 405 * resize);
                fx += (int)(Math.Ceiling(g1.MeasureString(txt, fDefault).Width)) - 4 * resize;
                i++;
            }

            // 保存输出到本地
            var path = $"~/Upload/{model.FileName}.png";
            string savePath = System.Web.HttpContext.Current.Server.MapPath(path);
            bitMap = new Bitmap(DrawingPictures.RoundCorners((Image)bitMap, 4 * resize, Color.Transparent));
            bitMap.MakeTransparent();
            bitMap.Save(savePath);
            ////微信小程序的限制，图片放到七牛上无法缓存，然后无法把海报保存到相册
            //QinQiuApi qniu = new QinQiuApi();
            //string resultpath = qniu.UploadFile(savePath, true);
            g1.Dispose();
            bitMap.Dispose();
            return path;
        }
        /// <summary>
        /// 生成指定数量长度的随机字符串
        /// </summary>
        /// <param name="codeCount"></param>
        /// <returns></returns>
        public static string GenerateCheckCodeNum(int codeCount)
        {
            int rep = 0;
            string str = string.Empty;
            long num2 = DateTime.Now.Ticks + rep;
            rep++;
            Random random = new Random(((int)(((ulong)num2) & 0xffffffffL)) | ((int)(num2 >> rep)));
            for (int i = 0; i < codeCount; i++)
            {
                int num = random.Next();
                str = str + ((char)(0x30 + ((ushort)(num % 10)))).ToString();
            }
            return str;
        }
    }

}