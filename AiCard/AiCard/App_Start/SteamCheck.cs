using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;

namespace AiCard.SteamCheck
{
    public class Check
    {
        /// <summary>
        /// 判断文件格式
        /// http://www.cnblogs.com/babycool 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsAllowedExtension(Stream stream, string steamCheckFileType)
        {
            stream.Position = 0;
            string fileclass = "";
            try
            {
                var reader = new BinaryReader(stream);
                for (int i = 0; i < 2; i++)
                {
                    fileclass += reader.ReadByte().ToString();
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                stream.Position = 0;
            }
            if (fileclass == steamCheckFileType)
            {
                return true;
            }
            else
            {
                return false;
            }



        }

    }


    public static class FileType
    {
        public const string Jpg = "255216";
        public const string Doc = "208207";
        public const string Xls = "208207";
        public const string Ppt = "208207";
        public const string Wps = "208207";
        public const string Docx = "8075";
        public const string Pptx = "8075";
        public const string Xlsx = "8075";
        public const string Zip = "8075";
        public const string Txt = "5150";
        public const string Rar = "8297";
        public const string Exe = "7790";
        public const string Gif = "7173";
        public const string Png = "13780";
        public const string Bmp = "6677";

        /*文件扩展名说明
        * 255216 jpg
        * 208207 doc xls ppt wps
        * 8075 docx pptx xlsx zip
        * 5150 txt
        * 8297 rar
        * 7790 exe
        * 3780 pdf      
        * 
        * 4946/104116 txt
        * 7173        gif 
        * 255216      jpg
        * 13780       png
        * 6677        bmp
        * 239187      txt,aspx,asp,sql
        * 208207      xls.doc.ppt
        * 6063        xml
        * 6033        htm,html
        * 4742        js
        * 8075        xlsx,zip,pptx,mmap,zip
        * 8297        rar   
        * 01          accdb,mdb
        * 7790        exe,dll
        * 5666        psd 
        * 255254      rdp 
        * 10056       bt种子 
        * 64101       bat 
        * 4059        sgf    
        */

    }
}