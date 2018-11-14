using AiCard.Common.CommModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiCard.Common
{
    //因为控制器不能使用接口和抽象类只能用实体类
    public class BaseFileUpload
    {
        /// <summary>
        /// 路径
        ///  <para>为空表示使用默认路径</para>
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 取消重命名
        /// </summary>
        public bool IsResetName { get; set; }

        /// <summary>
        /// 图片服务器
        /// </summary>
        public UploadServer Server { get; set; } = UploadServer.Local;
    }
}
