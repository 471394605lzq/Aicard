﻿using System;
using Newtonsoft.Json.Linq;

namespace AiCard.Common.TxIm
{
    public class ImApi
    {
        /// <summary>
        /// 注册用户（更新用户数据？）到腾讯云（已经注册的用户调用该接口不会报错，也不会重复注册）
        /// </summary>
        /// <param name="name">帐号 长度限制32</param>
        /// <param name="nickname">昵称</param>
        /// <param name="avatar">头像</param>
        public void ImportUser(string name, string nickname, string avatar)
        {
            //使用管理员帐号导入用户（注册）
            string url = $"https://console.tim.qq.com/v4/im_open_login_svc/account_import?usersig={ImConfig.Sign}&identifier={ImConfig.Admin}&sdkappid={ImConfig.SdkAppId}&random={ImConfig.Random}&contenttype=json";
            var api = new CommonApi.BaseApi(url, "POST", new { Identifier = name, Nick = nickname, FaceUrl = avatar });
            var result = api.CreateRequestReturnJson();
            if (result["ErrorCode"].Value<int>() != 0)
            {
                throw new Exception(result["ErrorInfo"].Value<string>());
            }
        }

    }
}