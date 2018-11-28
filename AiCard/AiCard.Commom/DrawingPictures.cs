
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace AiCard.Common
{
    public static class DrawingPictures
    {
        /// <summary>
        /// 绘制文本自动换行（超出截断）
        /// </summary>
        /// <param name=\"graphic\">绘图图面</param>
        /// <param name=\"font\">字体</param>
        /// <param name=\"text\">文本</param>
        /// <param name=\"recangle\">绘制范围</param>
        public static void DrawStringWrap(Graphics graphic, Font font, string text, Rectangle recangle, int top, int left, int widthnumber)
        {
            try
            {
                List<string> textRows = GetStringRows(graphic, font, text, recangle.Width, widthnumber);
                int rowHeight = (int)(Math.Ceiling(graphic.MeasureString(text, font).Height));
                int temprowheight = rowHeight == 0 ? 1 : rowHeight;
                int maxRowCount = recangle.Height / temprowheight;
                int drawRowCount = (maxRowCount < textRows.Count) ? maxRowCount <= 1 ? 1 : maxRowCount : textRows.Count;

                //int top = (recangle.Height - rowHeight * drawRowCount) / 2;
                //int top = 0;
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;

                for (int i = 0; i < drawRowCount; i++)
                {
                    Rectangle fontRectanle = new Rectangle(left, top + rowHeight * i, recangle.Width, rowHeight);
                    graphic.DrawString(textRows[i], font, new SolidBrush(Color.Black), fontRectanle, sf);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 将文本分行
        /// </summary>
        /// <param name=\"graphic\">绘图图面</param>
        /// <param name=\"font\">字体</param>
        /// <param name=\"text\">文本</param>
        /// <param name=\"width\">行宽</param>
        /// <returns></returns>
        private static List<string> GetStringRows(Graphics graphic, Font font, string text, int width, int widthnumber)
        {
            //int RowBeginIndex = 0;
            //int rowEndIndex = 0;
            int textLength = text.Length;
            int number = 0;
            //如果一行字数大于文本长度
            if (widthnumber >= textLength)
            {
                number = 1;
            }
            else
            {
                int tempn = textLength / widthnumber;
                int yn = textLength / widthnumber;//取余数
                //如果取余数大于0则行数加1
                if (yn > 0)
                {
                    number = tempn + 1;
                }
                else
                {
                    number = tempn;
                }
            }

            List<string> textRows = new List<string>();
            try
            {

                for (int i = 0; i < number; i++)
                {
                    if (i == 0 && i == number - 1)
                    {
                        textRows.Add(text.Substring(widthnumber * i));
                    }
                    else if (i == 0 && i < number - 1)
                    {
                        textRows.Add(text.Substring(widthnumber * i, widthnumber));
                    }
                    else if (i == number - 1)
                    {
                        int s = widthnumber * i;
                        textRows.Add(text.Substring(s));
                    }
                    else
                    {
                        textRows.Add(text.Substring(widthnumber * i, widthnumber));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return textRows;
        }



        //改变图片大小
        public static System.Drawing.Image ResizeImage(System.Drawing.Image imgToResize, Size size)
        {
            //获取图片宽度
            int sourceWidth = imgToResize.Width;
            //获取图片高度
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            //计算宽度的缩放比例
            nPercentW = ((float)size.Width / (float)sourceWidth);
            //计算高度的缩放比例
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;
            //期望的宽度
            int destWidth = (int)(sourceWidth * nPercent);
            //期望的高度
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //绘制图像
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return (System.Drawing.Image)b;
        }


        //下载图片
        public static string DownloadImg(string strPath, string strName, int width, int height, string dir = "~/Upload")
        {
            WebClient my = new WebClient();
            byte[] mybyte;
            mybyte = my.DownloadData(strPath);
            my.Dispose();
            MemoryStream ms = new MemoryStream(mybyte);
            System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
            System.Drawing.Image tempimg = ResizeImage(img, new Size(width, height));
            var dirPath = System.Web.HttpContext.Current.Server.MapPath(dir);
            if (!System.IO.Directory.Exists(dirPath))
            {
                System.IO.Directory.CreateDirectory(dirPath);
            }
            string filePath = $"{dirPath}/{DateTime.Now:hhmmss}{strName}";//Server.MapPath("../../Content/temofile/"+DateTime.Now.ToString()+ strName);
            tempimg.Save(filePath, ImageFormat.Png);   //保存
            img.Dispose();
            tempimg.Dispose();
            ms.Dispose();
            return filePath;
        }

        public static Image DownloadImg(string strPath)
        {
            using (WebClient my = new WebClient())
            {
                var mybyte = my.DownloadData(strPath);
                using (MemoryStream ms = new MemoryStream(mybyte))
                {
                    System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                    return img;
                }
            }

        }

        //画一个box
        public static void SetBox(Bitmap bitMap, Graphics gh, int width, int height, Color cl, Color bgcl, int x, int y, int bordersize)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    // 将图片画布的点绘制到整体画布的指定位置
                    bitMap.SetPixel(x + i, y + j, bgcl);
                }
            }
            gh.DrawLine(new Pen(cl, bordersize), new Point(x, y), new Point(x, y + height));
            gh.DrawLine(new Pen(cl, bordersize), new Point(x, y), new Point(x + width, y));
            gh.DrawLine(new Pen(cl, bordersize), new Point(x + width, y), new Point(x + width, y + height));
            gh.DrawLine(new Pen(cl, bordersize), new Point(x, y + height), new Point(x + width, y + height));
        }

        public static Image CutEllipse(Image img)
        {
            int x = img.Width / 2;
            int y = img.Height / 2;
            int r = Math.Min(x, y);

            Bitmap tmp = null;
            tmp = new Bitmap(2 * r, 2 * r);
            using (Graphics g = Graphics.FromImage(tmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TranslateTransform(tmp.Width / 2, tmp.Height / 2);
                GraphicsPath gp = new GraphicsPath();
                gp.AddEllipse(0 - r, 0 - r, 2 * r, 2 * r);
                Region rg = new Region(gp);
                g.SetClip(rg, CombineMode.Replace);
                Bitmap bmp = new Bitmap(img);
                g.DrawImage(bmp, new Rectangle(-r, -r, 2 * r, 2 * r), new Rectangle(x - r, y - r, 2 * r, 2 * r), GraphicsUnit.Pixel);

            }
            return tmp;
        }
    }
}