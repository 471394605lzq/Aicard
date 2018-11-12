/**
 * that(传页面的this过来，这里需要接收到新消息的时候操作setData改变视图显示出未读消息)
 * app(传配置信息)
 * selToID(对方账号id,列表不设置所以这里传空)
 * callback(登录成功之后的回调) 返回{State:成功Success|错误Error,Error:错误}
*/
function sdkLogin(that, app, callback) {
    if (!callback) {
        callback = () => {

        }
        callback.success = function () { };
        callback.error = function () { }
    }
    if (!callback.success) {
        callback.success = function () { };
    }
    if (!callback.error) {
        callback.error = function () { }
    }
    //this.init({
    //    accountMode: app.data.Config.accountMode
    //    , accountType: app.data.Config.accountType
    //    , sdkAppID: app.data.Config.sdkappid
    //    , selType: webim.SESSION_TYPE.C2C//私聊
    //    , agent_member_id: app.data.userInfo.id
    //    , id: selToID
    //    , name: app.data.userInfo.agent_name
    //    , icon: app.data.userInfo.agent_pic,
    //    that: that
    //});

    //当前用户身份
    var loginInfo = {
        'sdkAppID': app.data.Config.sdkappid, //用户所属应用id,必填
        'appIDAt3rd': app.data.Config.sdkappid, //用户所属应用id，必填
        'accountType': app.data.Config.accountType, //用户所属应用帐号类型，必填
        'identifier': app.data.userInfo.name, //当前用户ID,必须是否字符串类型，选填
        'identifierNick': app.data.userInfo.nick, //当前用户昵称，选填
        'userSig': app.data.userInfo.sign, //当前用户身份凭证，必须是字符串类型，选填
    };



    //监听事件（1V1监听这两个事件就够了）
    var listeners = {
        "onConnNotify": app.data.listeners.onConnNotify, //监听连接状态回调变化事件,必填
        "onMsgNotify": app.data.listeners.onMsgNotify

    };

    //其他对象，选填
    var options = {
        'isAccessFormalEnv': true,//是否访问正式环境，默认访问正式，选填
        'isLogOn': true//是否开启控制台打印日志,默认开启，选填
    };

    //sdk登录(独立模式)
    webim.login(loginInfo, listeners, options, function (resp) {
        callback.success();
    }, function (err) {
        callback.error(err);
    });
}