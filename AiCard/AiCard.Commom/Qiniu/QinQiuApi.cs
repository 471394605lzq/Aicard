using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qiniu.Util;
using Qiniu.Common;
using Qiniu.IO.Model;
using Qiniu.IO;
using Qiniu.RS;
using Qiniu.Http;
using System.Configuration;
using System.Diagnostics;
using System.Threading;

namespace AiCard.Common.Qiniu
{
    public class QinQiuApi
    {
        private static string AccessKey = ConfigurationManager.AppSettings["qiniuAccessKey"].ToString(); //"jMrLqQG-vKkJZpUC2aAJcuJPEy-QRNmU0js0rDJ1";

        private static string SecretKey = ConfigurationManager.AppSettings["qiniuSecretKey"].ToString();//"aS5GPLu7_i63sI4ZpBK52rymlVKGFpqCwgMpz8yk";

        private const string Bucket = "image";

        public static string ServerLink = ConfigurationManager.AppSettings["qiniuServerLink"].ToString();// "http://image.dtoao.com/";
        private Mac mac;
        //private Auth auth;

        public QinQiuApi()
        {

            mac = new Mac(AccessKey, SecretKey);

            //auth = new Auth(mac);

            Config.AutoZone(AccessKey, Bucket, false);
            Config.SetZone(ZoneID.CN_South, false);

        }
        //上传文件到七牛云
        public string UploadFile(string path, bool isDeleteAfterUpload = false, bool isCover = false)
        {

            string saveKey = new FileInfo(path).Name;
            string localFile = path;
            // 上传策略，参见 
            // https://developer.qiniu.com/kodo/manual/put-policy
            PutPolicy putPolicy = new PutPolicy();
            // 如果需要设置为"覆盖"上传(如果云端已有同名文件则覆盖)，请使用 SCOPE = "BUCKET:KEY"
            // putPolicy.Scope = bucket + ":" + saveKey;
            putPolicy.Scope = isCover ? $"{Bucket}:{saveKey}" : Bucket;
            // 上传策略有效期(对应于生成的凭证的有效期)          
            putPolicy.SetExpires(3600);
            // 上传到云端多少天后自动删除该文件，如果不设置（即保持默认默认）则不删除
            putPolicy.DeleteAfterDays = null;
            putPolicy.InsertOnly = 0;
            // 生成上传凭证，参见
            // https://developer.qiniu.com/kodo/manual/upload-token            
            string jstr = putPolicy.ToJsonString();
            string token = Auth.CreateUploadToken(mac, jstr);
            UploadManager um = new UploadManager();
            HttpResult result = um.UploadFile(localFile, saveKey, token);
            var jResult = JsonConvert.DeserializeObject<JObject>(result.Text);
            if (jResult.Properties().Any(s => s.Name == "error"))
            {
                throw new Exception(jResult["error"].Value<string>());
            }
            if (isDeleteAfterUpload)
            {
                try
                {
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
                catch (Exception)
                {
                    //删不掉就忽略
                }
            }
            return KeyToLink(jResult["key"].Value<string>());
        }

    /// <summary>
    /// 上传文件并预转格式
    /// </summary>
    /// <param name="key">要转换格式的文件名</param>
    /// <param name="filePath">文件路径</param>
    /// <returns></returns>
    //private string upload(string key, string filePath)
    //{
    //    //设置文件上传后所存储的空间名称
    //    String bucket = "amrtest";

    //    //普通上传,只需要设置上传的空间名就可以了,第二个参数可以设定token过期时间
    //    PutPolicy put = new PutPolicy();

    //    //对转码后的文件进行使用saveas参数自定义命名，也可以不指定,文件会默认命名并保存在当前空间。
    //    string mp3tpname = key.Split('.')[0].ToString() + ".mp3";
    //    String urlbase64 = Qiniu.Util.Base64URLSafe.Encode(bucket + ":" + mp3tpname);

    //    //一般指文件要上传到的目标存储空间（Bucket）。若为“Bucket”，
    //    //表示限定只能传到该Bucket（仅限于新增文件）；若为”Bucket:Key”，表示限定特定的文件，可修改该文件。
    //    put.Scope = bucket + ":" + key;
    //    // 可选。 若非0, 即使Scope为 Bucket:Key 的形式也是insert only.
    //    put.InsertOnly = 0;
    //    // "|"竖线前是你要转换格式的命令；竖线后是转换完成后，文件的命名和存储的空间的名称！
    //    put.PersistentOps = "avthumb/mp3/ab/128k/ar/44100/acodec/libmp3lame|saveas/" + urlbase64;
    //    //规定文件要在那个“工厂”进行改装，也就是队列名称！
    //    put.PersistentPipeline = "LittleBai";
    //    //音视频转码持久化完成后，七牛的服务器会向用户发送处理结果通知。这里指定的url就是用于接收通知的接口。
    //    //设置了`persistentOps`,则需要同时设置此字段
    //    put.PersistentNotifyUrl = "http://***.###.com/***/default.aspx";

    //    //返回数据格式：{"hash":"FvipQyyMwI0gvGc7_NUd8OVBuJ85","key":"55456.amr","persistentId":"z0.57eb86a945a2652644d64308"}
    //    return "";// ret.Response.ToString();
    //}


    public void DeleteFile(string key)
        {
            BucketManager bm = new BucketManager(mac);
            HttpResult result = bm.Delete(Bucket, key);
            if (result.Code != 200)
            {
                var jResult = JsonConvert.DeserializeObject<JObject>(result.Text);
                if (jResult.Properties().Any(s => s.Name == "error"))
                {
                    throw new Exception(jResult["error"].Value<string>());
                }
            }

        }


        public static string LinkToKey(string link)
        {
            return link.Replace(ServerLink, "");
        }

        public static string KeyToLink(string key)
        {
            return $"{ServerLink}{key}";
        }
    }
}