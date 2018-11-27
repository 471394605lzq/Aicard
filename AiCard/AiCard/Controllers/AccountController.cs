using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using AiCard.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AiCard.Common.Enums;
using AiCard.DAL.Models;
using AiCard.Common;
using AiCard.Common.WeChat;

namespace AiCard.Controllers
{

    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string afterlogin)
        {
            if (afterlogin == "后台登录")
            {
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 这不会计入到为执行帐户锁定而统计的登录失败次数中
            // 若要在多次输入错误密码的情况下触发帐户锁定，请更改为 shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    {
                        var user = db.Users.FirstOrDefault(s => s.UserName == model.UserName && s.UserType == UserType.Admin);
                        if (user == null)
                        {
                            ModelState.AddModelError("", "帐号不存在或密码错误！");
                            return View(model);
                        }

                        //把重要的用户信息进行加密，存放到cookie
                        this.SetAccountData(new AccountData
                        {
                            EnterpriseID = user.EnterpriseID,
                            UserID = user.Id,
                            UserName = user.UserName,
                            UserType = user.UserType,
                        });
                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return RedirectToLocal(returnUrl);
                        }
                        else
                        {
                            //默认返回地址
                            return RedirectToAction("Index", "EnterpriseManage");
                        }
                    }
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "无效的登录尝试。");
                    return View(model);
            }
        }

        //企业登录
        [AllowAnonymous]
        public ActionResult EnterpriseLogin(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        //企业登录
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> EnterpriseLogin(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // 这不会计入到为执行帐户锁定而统计的登录失败次数中
            // 若要在多次输入错误密码的情况下触发帐户锁定，请更改为 shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    {
                        var user = db.Users.FirstOrDefault(s => s.UserName == model.UserName
                            && s.UserType == UserType.Enterprise);

                        if (user == null)
                        {
                            ModelState.AddModelError("", "用户不存在或者密码错误！");
                            return View(model);
                        }
                        //获取用户类型 Admin：管理员 Enterprise：企业 Personal：个人
                        //存入cookie前给userid加密
                        this.SetAccountData(new AccountData
                        {
                            EnterpriseID = user.EnterpriseID,
                            UserID = user.Id,
                            UserName = user.UserName,
                            UserType = user.UserType,
                        });
                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return RedirectToLocal(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Info", "EnterpriseManage");
                        }
                    }
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "无效的登录尝试。");
                    return View(model);
            }
        }


        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // 要求用户已通过使用用户名/密码或外部登录名登录
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 以下代码可以防范双重身份验证代码遭到暴力破解攻击。
            // 如果用户输入错误代码的次数达到指定的次数，则会将
            // 该用户帐户锁定指定的时间。
            // 可以在 IdentityConfig 中配置帐户锁定设置
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "代码无效。");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                //var result = await UserManager.CreateAsync(user, model.Password);
                //if (result.Succeeded)
                //{
                //    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                //    // 有关如何启用帐户确认和密码重置的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=320771
                //    // 发送包含此链接的电子邮件
                //    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                //    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                //    // await UserManager.SendEmailAsync(user.Id, "确认你的帐户", "请通过单击 <a href=\"" + callbackUrl + "\">這裏</a>来确认你的帐户");

                //    return RedirectToAction("Index", "Home");
                //}
                //AddErrors(result);
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // 请不要显示该用户不存在或者未经确认
                    return View("ForgotPasswordConfirmation");
                }

                // 有关如何启用帐户确认和密码重置的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=320771
                // 发送包含此链接的电子邮件
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "重置密码", "请通过单击 <a href=\"" + callbackUrl + "\">此处</a>来重置你的密码");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // 如果我们进行到这一步时某个地方出错，则重新显示表单
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // 请不要显示该用户不存在
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // 请求重定向到外部登录提供程序
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // 生成令牌并发送该令牌
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // 如果用户已具有登录名，则使用此外部登录提供程序将该用户登录
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // 如果用户没有帐户，则提示该用户创建帐户
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // 从外部登录提供程序获取有关用户的信息
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
                db.Dispose();
            }

            base.Dispose(disposing);
        }



        #region 帮助程序
        // 用于在添加外部登录名时提供 XSRF 保护
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion


        #region 微信对接

        [HttpGet]
        public ActionResult LoginByWeiXinSilence(string state)
        {
            Common.WeChat.IConfig config = new Common.WeChat.WeChatWorkConfig();
            var p = new Dictionary<string, string>();
            p.Add("appid", config.AppID);
            p.Add("redirect_uri", "http://www.dtoao.com/Account/LoginByWeiXin");
            p.Add("response_type", "code");
            p.Add("scope", "snsapi_base");
            p.Add("state", state);
            return Redirect($"https://open.weixin.qq.com/connect/oauth2/authorize{p.ToParam("?")}#wechat_redirect");
        }

        /// <summary>
        /// 微信授权登录
        /// </summary>
        /// <param name="code"></param>
        /// <param name="state">扩展参数</param>
        /// <param name="type">端口类别（Web App Mini）</param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        public ActionResult LoginByWeiXin(string code, string state = null, WeChatAccount type = WeChatAccount.AiCardMini)
        {
            Func<string, string, ActionResult> error = (content, detail) =>
             {
                 if (type != WeChatAccount.PC)
                 {
                     return Json(Comm.ToJsonResult("Error", content, detail));
                 }
                 else
                 {
                     return this.ToError("错误", detail, Url.Action("Login", "Account"));
                 }
             };
            if (string.IsNullOrWhiteSpace(code))
            {
                return error("请求有误", "Code不能为空");
            }
            if (Request.HttpMethod == "GET")
            {
                type = WeChatAccount.PC;
            }
            if (type != WeChatAccount.AiCardMini && type != WeChatAccount.AiCardPersonalMini)
            {

                //非小程序
                switch (type)
                {
                    default:
                    case WeChatAccount.PC:
                        {
                            Common.WeChat.IConfig config = new Common.WeChat.WeChatWorkConfig();
                            Common.WeChat.WeChatApi wechat = new Common.WeChat.WeChatApi(config);
                            Common.WeChat.AccessTokenResult result;
                            try
                            {
                                result = wechat.GetAccessTokenSns(code);
                                var openID = result.OpenID;

                                if (state == "openid")
                                {
                                    Response.Cookies["WeChatOpenID"].Value = openID;
                                    return Json(Comm.ToJsonResult("Success", "成功", new { OpenID = openID }));
                                }
                                config.AccessToken = result.AccessToken;
                                var unionid = result.UnionID;
                                var user = db.Users.FirstOrDefault(s => s.WeChatID == unionid);

                                try
                                {
                                    if (user != null)
                                    {
                                        if (user.UserName == user.NickName)
                                        {
                                            var userInfo = wechat.GetUserInfoSns(openID);
                                            string avart;
                                            try
                                            {
                                                avart = this.Download(userInfo.HeadImgUrl);
                                            }
                                            catch (Exception)
                                            {
                                                avart = "~/Content/Images/404/avatar.png";
                                            }
                                            user.NickName = userInfo.NickName;
                                            user.Avatar = avart;
                                           
                                        }
                                        var option = new Bll.Users.UserOpenID(user);
                                        option.AddOpenID(config.AppID, result.OpenID);
                                        user.LastLoginDateTime = DateTime.Now;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        try
                                        {
                                            var userInfo = wechat.GetUserInfoSns(openID);
                                            user = CreateByWeChat(userInfo);
                                        }
                                        catch (Exception)
                                        {
                                            user = CreateByWeChat(new Common.WeChat.UserInfoResult { UnionID = unionid });
                                        }

                                    }
                                    if (type != WeChatAccount.PC)
                                    {
                                        return Json(Comm.ToJsonResult("Success", "成功", new UserForApiViewModel(user)));
                                    }
                                    SignInManager.SignIn(user, true, true);
                                    switch (state.ToLower())
                                    {
                                        case null:
                                        case "":
                                        case "ticketindex":
                                            return RedirectToAction("Index", "Tickets");
                                        case "qrcode":
                                            return RedirectToAction("Index", "Tickets", new { mode = "qrcode" });
                                        default:
                                            return Redirect(state);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    return error("请求有误", ex.Message);
                                }
                            }
                            catch (Exception ex)
                            {
                                return error("请求有误", ex.Message);
                            }
                        }
                }
            }
            else
            {
                Common.WeChat.IConfig config;
                switch (type)
                {
                    case WeChatAccount.AiCardMini:
                        config = new Common.WeChat.ConfigMini();
                        break;
                    case WeChatAccount.AiCardPersonalMini:
                        config = new Common.WeChat.ConfigMiniPersonal();
                        break;
                    default:
                        return Json(Comm.ToJsonResult("Error", "Type参数有误"));
                }
                //小程序
                WeChatMinApi wechat = new Common.WeChat.WeChatMinApi(config);
                try
                {
                    var result = wechat.Jscode2session(code);
                    ApplicationUser user = null;
                    if (!string.IsNullOrWhiteSpace(result.UnionID))
                    {
                        user = db.Users.FirstOrDefault(s => s.WeChatID == result.UnionID);
                        // 把OpenID存进数据库
                        if (user != null)
                        {
                            Comm.WriteLog("LoginByWeiXin", $"AppID={config.AppID}&OpenID={result.OpenID}", DebugLogLevel.Normal);
                            var option = new Bll.Users.UserOpenID(user);
                            option.AddOpenID(config.AppID, result.OpenID);
                            db.SaveChanges();
                        }
                    }
                    return Json(Comm.ToJsonResult("Success", "成功", new
                    {
                        result.OpenID,
                        result.UnionID,
                        User = user == null ? null : new UserForApiViewModel(user)
                    }));
                }
                catch (Exception ex)
                {
                    return Json(Comm.ToJsonResult("Error", ex.Message));
                }



            }
        }

        [HttpPost]
        [AllowCrossSiteJson]
        public ActionResult RegisterByWeiXin(Common.WeChat.UserInfoResult model)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(model.UnionID))
                {
                    if (string.IsNullOrWhiteSpace(model.OpenID))
                    {
                        return Json(Comm.ToJsonResult("OpenIDNoFound", $"OpenID不能为空"));
                    }
                    //如果用户没关注公众号，获取不了UnionID，从EncryptedData从解密用户数据
                    var session = Common.WeChat.Jscode2sessionResultList.GetSession(model.OpenID);
                    var str = Common.WeChat.Jscode2sessionResultList.AESDecrypt(model.EncryptedData, session, model.IV);
                    try
                    {

                        string debug = JsonConvert.SerializeObject(new
                        {
                            model.EncryptedData,
                            session,
                            model.IV,
                            AES = str
                        });
                        Comm.WriteLog("WeiXin", debug, DebugLogLevel.Normal);
                        var jUser = JsonConvert.DeserializeObject<JObject>(str);
                        var unionID = jUser["unionId"]?.Value<string>();
                        if (unionID == null)
                        {
                            return Json(Comm.ToJsonResult("UnionIDNoFound", "获取不了UnionID"));
                        }
                        model.UnionID = unionID;
                    }
                    catch (Exception)
                    {
                        //如果解密后发现昵称有乱码
                        //如"nickName\":\"涓€鐪兼湜宸?,\"gender\":1,
                        //把乱码部分全部去掉重新解析
                        try
                        {
                            var index = str.IndexOf("\"unionId\"");
                            var newStr = str.Remove(1, index - 1);
                            var jUser = JsonConvert.DeserializeObject<JObject>(newStr);
                            var unionID = jUser["unionId"]?.Value<string>();
                            model.UnionID = unionID;
                        }
                        catch (Exception)
                        {
                            return Json(Comm.ToJsonResult("AESDecryptFail", "解密失败", new { Info = model, Aes = str, Session = session }));
                        }
                    }


                }
                var user = CreateByWeChat(model);
                return Json(Comm.ToJsonResult("Success", "成功", new UserForApiViewModel(user)));
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("Error", "注册失败", ex.Message));
            }
        }

        private ApplicationUser CreateByWeChat(Common.WeChat.UserInfoResult model)
        {

            string username, nickname, avart, unionId = model.UnionID;
            var user = db.Users.FirstOrDefault(s => s.WeChatID == unionId);
            IConfig config = null;
            switch (model.Type)
            {
                case WeChatAccount.AiCardMini:
                    config = new ConfigMini();
                    break;
                case WeChatAccount.AiCardPersonalMini:
                    config = new ConfigMiniPersonal();
                    break;
                default:
                case WeChatAccount.PC:
                    config = new WeChatWorkConfig();
                    break;
            }
            if (user != null)
            {
                string appID = config.AppID;
                var op1 = new Bll.Users.UserOpenID(user);
                op1.AddOpenID(appID, model.OpenID);
                db.SaveChanges();
                return user;
            }
            nickname = model.NickName;

            avart = model.HeadImgUrl;
            if (!string.IsNullOrWhiteSpace(avart))
            {
                try
                {
                    avart = this.Download(avart);
                }
                catch (Exception)
                {
                    avart = "~/Content/Images/404/avatar.png";
                }
            }
            unionId = model.UnionID;

            #region 把图片传到七牛
            var path = Server.MapPath(avart);
            avart = new Common.Qiniu.QinQiuApi().UploadFile(path, true);
            #endregion


            do
            {
                username = $"wc{DateTime.Now:yyyyMMddHHmmss}{Comm.Random.Next(1000, 9999)}";
            } while (db.Users.Any(s => s.UserName == username));
            if (string.IsNullOrWhiteSpace(nickname))
            {
                nickname = username;
            }
            user = new ApplicationUser
            {
                WeChatID = unionId,
                UserName = username,
                NickName = nickname,
                Avatar = avart,
                RegisterDateTime = DateTime.Now,
                LastLoginDateTime = DateTime.Now,
                UserType = UserType.Personal
            };
            var option = new Bll.Users.UserOpenID(user);
            option.AddOpenID(config.AppID, model.OpenID);
            var r = UserManager.Create(user);
            user = db.Users.FirstOrDefault(s => s.WeChatID == unionId);

            db.SaveChanges();
            if (!r.Succeeded)
            {
                throw new Exception(r.Errors.FirstOrDefault());
            }

            return user;
        }

        public ActionResult GetPhoneNumberFromWeChat(string userID, string iv, string encryptedData)
        {
            var user = db.Users.FirstOrDefault(s => s.Id == userID);

            if (user == null)
            {
                return Json(Comm.ToJsonResult("UserNoFound", "用户不存在"));
            }
            if (db.CardPersonals.Any(s => s.UserID == userID))
            {
                return Json(Comm.ToJsonResult("CardPersonalHadCreate", "该用户已经个人名片已存在"), JsonRequestBehavior.AllowGet);
            }
            //把数据中的OpenID取出
            var userOpenIDs = new Bll.Users.UserOpenID(user);
            IConfig config = new ConfigMiniPersonal();
            var openID = userOpenIDs.SearchOpenID(config.AppID);
            if (openID == null)
            {
                return Json(Comm.ToJsonResult("OpenIDIsNull", "OpenID不存在"), JsonRequestBehavior.AllowGet);
            }
            string session = null;
            try
            {
                session = Jscode2sessionResultList.GetSession(openID);
            }
            catch (Exception ex)
            {
                return Json(Comm.ToJsonResult("GetSessionFail", ex.Message), JsonRequestBehavior.AllowGet);
            }

            string mobile = null;
            try
            {
                mobile = Jscode2sessionResultList.AESDecryptPhoneNumber(encryptedData, session, iv);
            }
            catch (Exception)
            {
                Comm.WriteLog("CreateByWeChatPhoneDecrypt", JsonConvert.SerializeObject(new { encryptedData, session, iv }), Common.Enums.DebugLogLevel.Error);
                return Json(Comm.ToJsonResult("DecryptFail", "解密失败，SessionKey过期，需要重新调用登录接口"));
            }
            return Json(Comm.ToJsonResult("Success", "成功", mobile), JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}