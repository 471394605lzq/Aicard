using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace AiCard.Common
{
    public static class QrCode
    {
        public static Image Generate(string data)
        {

            var qrCodeEncoder = new ThoughtWorks.QRCode.Codec.QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 16;
            qrCodeEncoder.QRCodeVersion = 4;
            qrCodeEncoder.QRCodeErrorCorrect = ThoughtWorks.QRCode.Codec.QRCodeEncoder.ERROR_CORRECTION.L;
            Bitmap temp = null;
            Action encode = null; 
            encode = () =>
            {

                try
                {
                    temp = qrCodeEncoder.Encode(data, System.Text.Encoding.UTF8);
                }
                catch (Exception)
                {
                    qrCodeEncoder.QRCodeVersion++;
                    encode();
                }
            };
            encode();

            Image image = new Bitmap(temp.Width + 10, temp.Height + 10);
            Graphics gra = Graphics.FromImage(image);
            gra.Clear(Color.White);
            gra.DrawImage(temp, new Rectangle { Width = temp.Width, Height = temp.Height, X = 10, Y = 10 },
                new Rectangle { Width = image.Width, Height = image.Width }, GraphicsUnit.Pixel);
            gra.Dispose();
            return image;
        }

        public static Image SetLogo(Image image, Image logo)
        {
            //logo缩放
            int proportion = (image.Height * 150) / 500;
            int logow = logo.Width;
            int logoh = logo.Height;
            int bestw = 0;
            int besth = 0;
            if (logoh > proportion || logow > proportion)
            {
                if (logow > proportion)
                {
                    bestw = proportion;
                    besth = (logoh * bestw) / logow;
                }
                else
                {
                    besth = proportion;
                    bestw = (logow * besth) / logoh;
                }
            }
            else
            {
                besth = logoh;
                bestw = logow;
            }
            Image smalllogo = new Bitmap(bestw, besth);
            var gra = Graphics.FromImage(smalllogo);
            gra.DrawImage(logo, new Rectangle(0, 0, bestw, besth), new Rectangle(0, 0, logo.Width, logo.Height), GraphicsUnit.Pixel);
            gra.Dispose();
            logo = smalllogo;
            //重合
            Graphics g = Graphics.FromImage(image);
            g.DrawImage(logo, image.Width / 2 - logo.Width / 2, image.Height / 2 - logo.Height / 2, logo.Width, logo.Height);
            g.Dispose();
            return image;
        }
    }
}