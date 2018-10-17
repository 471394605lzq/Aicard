using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Qiniu.Util;
using Qiniu.Common;
using Qiniu.IO.Model;
using Qiniu.IO;
using Qiniu.Http;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qiniu.RS;

namespace AiCard.Qiniu
{
    public class QinQiuApi
    {
        private const string AccessKey = "jMrLqQG-vKkJZpUC2aAJcuJPEy-QRNmU0js0rDJ1";

        private const string SecretKey = "aS5GPLu7_i63sI4ZpBK52rymlVKGFpqCwgMpz8yk";

        private const string Bucket = "image";

        public const string ServerLink = "http://image.dtoao.com/";
        private Mac mac;
        //private Auth auth;

        public QinQiuApi()
        {

            mac = new Mac(AccessKey, SecretKey);

            //auth = new Auth(mac);

            Config.AutoZone(AccessKey, Bucket, false);
            Config.SetZone(ZoneID.CN_South, false);

        }

        public string UploadFile(string path, bool isDeleteAfterUpload = false)
        {

            string saveKey = new FileInfo(path).Name;
            string localFile = path;
            // 上传策略，参见 
            // https://developer.qiniu.com/kodo/manual/put-policy
            PutPolicy putPolicy = new PutPolicy();
            // 如果需要设置为"覆盖"上传(如果云端已有同名文件则覆盖)，请使用 SCOPE = "BUCKET:KEY"
            // putPolicy.Scope = bucket + ":" + saveKey;
            putPolicy.Scope = Bucket;
            // 上传策略有效期(对应于生成的凭证的有效期)          
            putPolicy.SetExpires(3600);
            // 上传到云端多少天后自动删除该文件，如果不设置（即保持默认默认）则不删除
            putPolicy.DeleteAfterDays = null;
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